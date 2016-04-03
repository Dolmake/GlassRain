/*-------------------------------------------------
Created by: Daniel Peribáñez Sepúlveda
January 2013
peribanez.daniel@gmail.com

/**/


using UnityEngine;
using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Large drops that fall through the screen
/// </summary>
public class ContinousDrop : MonoBehaviour {

    /// <summary>
    /// Drop Prefab
    /// </summary>
    public GameObject DropPrefab;

    /// <summary>
    /// Rain Power percent
    /// </summary>
    public float _RainPercent = 0;

    /// <summary>
    /// Max drops running at the same time
    /// </summary>
    public int MaxDrops = 5;

    /// <summary>
    /// Speed percent
    /// </summary>
    public float _SpeedPercent = 0;

    /// <summary>
    /// Min Speed
    /// </summary>
    public float SpeedMin = 0.001f;

    /// <summary>
    /// Max Speed
    /// </summary>
    public float SpeedMax = 0.01f;

    /// <summary>
    /// Drops direction, -1 down, 1 up.
    /// </summary>
    public float _Direction = -1f;

    /// <summary>
    /// Time to invoke a new Drop
    /// </summary>
    public float InvokeDropTime = 1f;

    //Factory of GameObjects
    GameObjectFactory _factory = new GameObjectFactory();  

    /// <summary>
    /// Bounds to describe the area where the drops falls
    /// </summary>
    public Bounds Bounds;

    //Scale modificators
    Vector3 _scaleBackup;
    Vector3 _scale;

	// Use this for initialization
	void Start () {
        _factory.Initialize(this.transform, DropPrefab, 5); //Initialize the factory
        _scale = this.DropPrefab.transform.localScale;      //Backup the original scale
        _scaleBackup = _scale;
	}
	
	// Update is called once per frame
	void Update () {
        if (NeedAddDrop())
            AddDrop();
        UpdateDropsDirection();
        UpdateDropsScale();
        DestroyUselessDrops();
	}

    private void UpdateDropsScale()
    {
        _scale.x = _scaleBackup.x * _RainPercent;
        _scale.y = _scaleBackup.y * _RainPercent;
        for (int i = 0; i < _factory.UsedObjects.Count; ++i)
            _factory.UsedObjects[i].transform.localScale = _scale;
    }

    private void DestroyUselessDrops()
    {
        for (int i = _factory.UsedObjects.Count - 1;i >= 0 ; --i)
        {
            Vector3 pos = _factory.UsedObjects[i].transform.localPosition;
            if (!Bounds.Contains(pos))
                _factory.Release(_factory.UsedObjects[i]);
        }
    }

    private void UpdateDropsDirection()
    {
        float speed = _SpeedPercent;
        for (int i = 0; i < _factory.UsedObjects.Count; ++i)        
            _factory.UsedObjects[i].transform.localPosition += new Vector3(0, (speed * (SpeedMax - SpeedMin) + SpeedMin) * _Direction, 0);        
    }

   
    private void AddDrop()
    {
        GameObject drop = _factory.Get();
        drop.transform.localPosition = new Vector3(
            Random.Range(Bounds.min.x, Bounds.max.x),
            Random.Range(Bounds.min.y, Bounds.max.y),
            Bounds.center.z);
        //Debug.Log("Continuos Drop-> Creating Drop at " + drop.transform.localPosition);
    }

    float _accum = 0;
    private bool NeedAddDrop()
    {
        //We invoke a drop every InvokeDropTime
        bool result = false;
        _accum += Time.deltaTime;
        if (_accum > InvokeDropTime )
        {
            _accum = 0;
            if (_factory.UsedObjects.Count < MaxDrops)// && SpeedPercent > 0.6 || SpeedPercent < 0.4)
                result = true;
        }
        return result;
    }
}
