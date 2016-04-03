/*-------------------------------------------------
Created by: Daniel Perib��ez Sep�lveda
January 2013
peribanez.daniel@gmail.com

/**/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Manage the Areas on the surface. Has the MAGIC. Avoid the normals in the area 
/// Every Update() serialize the Areas internal list into a Texture which will be
/// deserializated into the VertexShader, using the areas info to set the normals to
/// (0,1,0).
/// </summary>
public class CheeseManager : MonoBehaviour {

    public static string AREA_TEXTURE_PARAMETER_NAME = "_TexMem";
    public static string DECIMALS = "_Decimals";
    public static string TEXTURE_WIDTH = "_TextureSize";
    public static string STRUCT_SIZE_PX = "_NumParameters";

    public delegate void RemoveAreaEventHandler(CheesePortion area);
    public event RemoveAreaEventHandler OnRemoveArea;

    /// <summary>
    /// It is public just for Debug Info
    /// </summary>
    public Texture2D Memory_Texture;

    /// <summary>
    /// Max Areas at the same time.
    /// </summary>
    public int MaxCheeseAreas = 10;

    /// <summary>
    /// Flag to indicate if It has to manage the Area.
    /// </summary>
    public bool ManageAreas = true;

    /// <summary>
    /// When an Area reach a 1f Percent, the it is removed
    /// </summary>
    public float _MaxPercentToRemove = 1f;

    /// <summary>
    /// Rain recover factor
    /// </summary>
    public float _DeltaSpeed = 1f;
   

    /// <summary>
    /// Area Ratio
    /// </summary>
    public float _AreaRatio = 0.5f;

    /// <summary>
    /// Rain distortion
    /// </summary>
    public float _Distortion = 10f;

    /// <summary>
    /// Static Rain distortion
    /// </summary>
    public float _BumpMapDistortion = 100.0f;

  
    /// <summary>
    /// Oscilating between [0,1]
    /// </summary>
    public float _DrawNormals = 0f;

    /// <summary>
    /// Function to describe the Wave Propagation
    /// </summary>
    public AnimationCurve WaveExpansion;

    int _textureWidth = 16;
    int _textureHeight = 2;
    List<CheesePortion> _cheeseAreas = new List<CheesePortion>();//Cheese areas managing

    void Awake()
    {
        //We plus 1 because we use the first pixel to set the number of Areas
        _textureWidth = CheesePortion.BiggerPow2((MaxCheeseAreas/2 * CheesePortion.NUM_PARAMETERS) + 1);
        //Calculate the MaxAreas allowed.
        MaxCheeseAreas = (_textureWidth - 1) / CheesePortion.NUM_PARAMETERS * 2;

        if (Memory_Texture == null)
        {            
            Memory_Texture = new Texture2D(_textureWidth, _textureHeight, TextureFormat.ARGB32, false);
            Memory_Texture.filterMode = FilterMode.Point;
            Memory_Texture.wrapMode = TextureWrapMode.Clamp;
        }
        else
        {
            _textureWidth = Memory_Texture.width;
            _textureHeight = Memory_Texture.height;
        }

        Color32[] colors = Memory_Texture.GetPixels32(0);
        CleanBytes(ref colors);
        SetNumOfAreas(ref colors, 0);
        Memory_Texture.SetPixels32(colors,0);
        Memory_Texture.Apply();

        this.GetComponent<Renderer>().material.SetFloat(STRUCT_SIZE_PX, CheesePortion.NUM_PARAMETERS);
        this.GetComponent<Renderer>().material.SetFloat(TEXTURE_WIDTH, Memory_Texture.width);
        this.GetComponent<Renderer>().material.SetFloat(DECIMALS, CheesePortion.DECIMALS);
        this.GetComponent<Renderer>().material.SetTexture(AREA_TEXTURE_PARAMETER_NAME, Memory_Texture);
    }

    // Use this for initialization
    void Start()
    {
      
        
    }

    // Update is called once per frame
    void Update()
    {
        this.GetComponent<Renderer>().material.SetFloat("_AreasCounter", MaxCheeseAreas);// _cheeseAreas.Count);
        this.GetComponent<Renderer>().material.SetFloat("_DrawNormals", _DrawNormals);
        this.GetComponent<Renderer>().material.SetFloat("_AreaRatio", _AreaRatio);
        this.GetComponent<Renderer>().material.SetFloat("_Distorsion", _Distortion);
        this.GetComponent<Renderer>().material.SetFloat("_BumpMapDistortion", _BumpMapDistortion);

         //We manage only the areas with Auto flag enabled
        if (ManageAreas)
        {
            //while (_cheeseAreas.Count > MaxCheeseAreas)
            //    RemoveArea(0);
            for (int i = _cheeseAreas.Count -1; i >= 0; i--)
            {
                if (_cheeseAreas[i].Auto)
                {
                    UpdateArea(_cheeseAreas[i]);

                    if (_cheeseAreas[i].AccumTime > 1f || _cheeseAreas[i].Percent > _MaxPercentToRemove)
                        RemoveArea(i);
                }
            }
        }

        //Update texture
        Color32[] colors = Memory_Texture.GetPixels32(0);
        SerializeAreasToTexture(ref colors, _cheeseAreas);
        Memory_Texture.SetPixels32(colors, 0);        
        Memory_Texture.Apply();
    }

    public void UpdateArea(CheesePortion area)
    {
        area.AccumTime += Time.deltaTime * _DeltaSpeed;
        area.Percent = WaveExpansion.Evaluate(area.AccumTime);  
    }

    public int DebugAreasCounter = 0;
    private void SerializeAreasToTexture(ref Color32[] colors, List<CheesePortion> cheeseAreas)
    {
        int index = SetNumOfAreas(ref colors, cheeseAreas.Count);
        for (int i = 0;(index + 4) < colors.Length && i < cheeseAreas.Count; ++i)
        {
            cheeseAreas[i].SerializeToColorArray(ref colors, ref index);
            //float r = CheesePortion.Decode(colors[index-4]);
            //float g = CheesePortion.Decode(colors[index-3]);
            //float b = CheesePortion.Decode(colors[index-2]);
            //float a = CheesePortion.Decode(colors[index-1]);
        }
        for (; index < colors.Length - 1; index++)        
            colors[index].r = colors[index].g = colors[index].b = colors[index].a = 0;
        
        //Debug.Log("CheeseManager-> Index:" + index);
        DebugAreasCounter = cheeseAreas.Count;
    }

    private int SetNumOfAreas(ref Color32[] colors, int counter)
    {       
        int index = 0;
        colors[index++] = CheesePortion.EncodeFloatToColor32(counter);
        return index;
    }

    private void CleanBytes(ref Color32[] colors)
    {
        for (int i = 0; i < colors.Length; ++i)        
            colors[i].a = colors[i].r = colors[i].g = colors[i].b = 0;        
    }

    /// <summary>
    /// Add an Area
    /// </summary>
    /// <param name="CheesePortion"></param>
    public bool AddArea(CheesePortion area)
    {
        if (_cheeseAreas.Count < MaxCheeseAreas)
        {
            SendMessage("mOnAreaAdded", area, SendMessageOptions.DontRequireReceiver);
            _cheeseAreas.Add(area);
            return true;
        }
        else return false;
    }

    public bool AddAreaAndRemove(CheesePortion area)
    {
        while (!AddArea(area))
        {
            RemoveArea(0);
        }
        return true;

    }

    /// <summary>
    /// Removes an Area
    /// </summary>
    /// <param name="CheesePortion"></param>
    /// <returns></returns>
    public bool RemoveArea(CheesePortion area)
    {  
        int i =0;
        for (; _cheeseAreas[i] != area && i < _cheeseAreas.Count; ++i) ;

        return i < _cheeseAreas.Count ? RemoveArea(i) : false;
    }

    /// <summary>
    /// Removes an Area by Index
    /// </summary>
    /// <param name="index"></param>
    /// <returns></returns>
    public bool RemoveArea(int index)
    {
        bool result = true;
        if (index >= 0 && index < _cheeseAreas.Count)
        {
            CheesePortion area = _cheeseAreas[index];
            //Debug.Log("CheeseManager-> Removing Area");
            _cheeseAreas.RemoveAt(index);
            if (OnRemoveArea != null)
                OnRemoveArea(area);
            
        }
        else result = false;
        return result;
    }


   
}
