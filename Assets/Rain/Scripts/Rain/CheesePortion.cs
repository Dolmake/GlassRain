/*-------------------------------------------------
Created by: Daniel Peribáñez Sepúlveda
January 2013
peribanez.daniel@gmail.com

/**/

using UnityEngine;
using System.Collections;

public class CheesePortion  {

    /// <summary>
    /// Decimals precision to Encode/Decode a float from a Color32
    /// </summary>
    public static float DECIMALS = 100000f;

    /// <summary>
    /// Number of serialized parameters
    /// </summary>
    public static int NUM_PARAMETERS = 6;

    /// <summary>
    /// Empty Cheese portion
    /// </summary>
    public static CheesePortion EMPTY = new CheesePortion();

    /// <summary>
    /// Ratio of the wave (Serializable)
    /// </summary>    
    public float Ratio;

    /// <summary>
    /// Current Percent time of the wave (Serializable)
    /// </summary>
    public float Percent;

    /// <summary>
    /// Area point (Serializable)
    /// </summary>
    public Vector3 Point;

    /// <summary>
    /// Accumulated time
    /// </summary>
    public float AccumTime = 0.0f;

    /// <summary>
    /// StartAngle
    /// </summary>
    public float StartAngle = 0;

    /// <summary>
    /// EndAngle
    /// </summary>
    public float EndAngle = 30;

    /// <summary>
    /// Indicate if CheesePortion is manage by RainManager
    /// </summary>
    public bool Auto = true;

    public CheesePortion():
        this(0.5f, new Vector3())
    {
    }

    public CheesePortion(float ratio, Vector3 point) :
        this(ratio, point, 0 ,0)
    {
    }
     public CheesePortion(float ratio, Vector3 point, float startAngle, float endAngle)         
    {
        Ratio = ratio;
        Point = point;
        StartAngle = startAngle;
        EndAngle = endAngle;
    }

    /// <summary>
    /// Size in Bytes
    /// </summary>
    public static int Size
    {
        get
        {
            return NUM_PARAMETERS * 4; //Num Parameters to serialize By 4 bytes per float
        }
    }

    public void SetTime(float time)
    {
        AccumTime += time;
        Percent = AccumTime;// / 1.0f;
    }
    public void ResetTime(float time)
    {
        AccumTime = time;
        SetTime(0);
    }

    /// <summary>
    /// Encode a float value into a Color32 which it will be written in a Texture
    /// </summary>
    /// <param name="f">Float Value</param>
    /// <returns></returns>
    public static Color32 EncodeFloatToColor32(float f)
    {
        int integer = (int)(f * DECIMALS);
        Color32 c = new Color32();
        c.r = (byte)(integer >> 24);
        c.g = (byte)(integer >> 16);
        c.b = (byte)(integer >> 8);
        c.a = (byte)(integer);        
        return c;
    }

    /// <summary>
    /// Serialize THIS into a Color32[] from an Index
    /// </summary>
    /// <param name="colors"></param>
    /// <param name="index"></param>
    public void SerializeToColorArray(ref Color32[] colors, ref int index)
    {
        colors[index++] = CheesePortion.EncodeFloatToColor32(Ratio);
        
        colors[index++] = CheesePortion.EncodeFloatToColor32(Point.x);
        //colors[index++] = CheesePortion.EncodeFloatToColor32(Point.y);
        colors[index++] = CheesePortion.EncodeFloatToColor32(Point.z);

        colors[index++] = CheesePortion.EncodeFloatToColor32(Percent);
        colors[index++] = CheesePortion.EncodeFloatToColor32(StartAngle);
        colors[index++] = CheesePortion.EncodeFloatToColor32(EndAngle);       
    }

    /// <summary>
    /// Aux function to get the closest Pow2 to a value.
    /// Useful to calculate optimal Texture sizes
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static int BiggerPow2(int value)
    {
        int v = 1;
        while (v <= value) v = v << 1;
        return v;
    }
}
