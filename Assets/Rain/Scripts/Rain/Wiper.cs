/*-------------------------------------------------
Created by: Daniel Peribáñez Sepúlveda
January 2013
peribanez.daniel@gmail.com

/**/

using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using GlassUtils;

/// <summary>
/// A Single windscreen
/// </summary>
public class Wiper: MonoBehaviour {

    /// <summary>
    /// Windscreen Mesh
    /// </summary>
    public GameObject Windscreen;

    /// <summary>
    /// Pivot where Windscreen turns
    /// </summary>
    public GameObject Pivot;

    /// <summary>
    /// Windscreen Ratio from [0,1]
    /// </summary>
    public float Ratio = 0.3f;

    /// <summary>
    /// Windscreen Position, XZ from [0,1]. Y not used. 
    /// </summary>
    public Vector3 Position = new Vector3(0.5f,0, 0.5f);
   
    /// <summary>
    /// Reference to the CheeseManager to control the Wipers
    /// </summary>
    CheeseManager _cheeseManager;

    /// <summary>
    /// Windscreen movement speed 
    /// </summary>
    public float _MovementSpeed = 1;

    /// <summary>
    /// Windscreen movement degrees
    /// </summary>
    public float _AngleDegrees = 120f;

    /// <summary>
    /// Time the windscreen remains stopped
    /// </summary>
    public float _StopTime = 1;


    /// <summary>
    /// Time the windscreen remains in movement
    /// </summary>
    public float _CycleTime = 0.5f;

    /// <summary>
    /// Private value to control the Windscreen angle
    /// </summary>
    public float _Percent = 0f;
    public float _accumTime = 0;

  

    /// <summary>
    /// Current Direction.
    /// </summary>
    public WindshieldDirection _CurrentDir = WindshieldDirection.STOPPED;
  
    //Angle in Radians.
    float AngleRadians { get { return _AngleDegrees * Mathf.Deg2Rad; } }

    /// <summary>
    /// Flag to indicate if Update() is called by an external object.
    /// Useful to manage various Windscreen simultaneously.
    /// </summary>
    public bool ExternalManagement = false;

    //Windscreen Area
    CheesePortion _wiper = null;
    PoolFactory<CheesePortion> _wipersPool = new PoolFactory<CheesePortion>();

    public int D_wipersPool;

    // Use this for initialization
    void Start()
    {
       
    }

    void OnEnable()
    {
        _cheeseManager = GetComponent<CheeseManager>();
        _cheeseManager.OnRemoveArea += new CheeseManager.RemoveAreaEventHandler(_cheeseManager_OnRemoveArea);
        _wipersPool.Initialize(5);
    }    

    void OnDisable()
    {
        _cheeseManager.OnRemoveArea -= new CheeseManager.RemoveAreaEventHandler(_cheeseManager_OnRemoveArea);
        _wipersPool.Deinitialize();
        if (_wiper != null)
            _wiper.Auto = true;        
    }

    // Update is called once per frame
    void Update()
    {
        D_wipersPool = _wipersPool.CountUsed;
        if (!ExternalManagement)
            Update(Time.deltaTime);
    }
  
    public void Update(float deltaTime)
    {
        float angle = 0;
        float previousAngle = 0;
        if (_MovementSpeed == 0)
            _CurrentDir = WindshieldDirection.STOPPED;
        if (_CurrentDir == WindshieldDirection.FORWARD)
        {
            _Percent = UpdatePercent();
            IniWiper(0,0);
            previousAngle = _wiper.EndAngle;           
            _wiper.Ratio = Ratio;
            _wiper.EndAngle = _Percent * AngleRadians;
            _cheeseManager.UpdateArea(_wiper);
            _wiper.EndAngle = _wiper.EndAngle > AngleRadians ? AngleRadians : _wiper.EndAngle;
            angle = previousAngle - _wiper.EndAngle;
            if (_wiper.EndAngle >= AngleRadians)
            {
                _CurrentDir = WindshieldDirection.REVERSE;
                _Percent = 0;
                DestroyWiper();
                _accumTime = 0;
            }
        }
        else if (_CurrentDir == WindshieldDirection.REVERSE)
        {
            _Percent = UpdatePercent();
            IniWiper(AngleRadians, AngleRadians);
            previousAngle = _wiper.StartAngle;
            _wiper.Ratio = Ratio;
            _wiper.StartAngle = AngleRadians - (_Percent * AngleRadians);
            _cheeseManager.UpdateArea(_wiper);
            _wiper.StartAngle = _wiper.StartAngle < 0 ? 0 : _wiper.StartAngle;
            angle = previousAngle - _wiper.StartAngle;
            if (_Percent >= 1)            
            {
                _CurrentDir = WindshieldDirection.STOPPED;
                _Percent = 0;
                DestroyWiper();
                _accumTime = 0;
            }
        }
        else if (_CurrentDir == WindshieldDirection.STOPPED)
        {
            _accumTime += Time.deltaTime * _MovementSpeed;
            if (_accumTime >= _StopTime)
            {
                _CurrentDir = WindshieldDirection.FORWARD;
                _accumTime = 0;
                _Percent = 0;
            }
        }

        if (Windscreen && Pivot)
            Windscreen.transform.RotateAround(Pivot.transform.position, Pivot.transform.up, angle * Mathf.Rad2Deg);
    }

    private void IniWiper(float startAngle, float endAngle)
    {
        if (_wiper == null)
        {
            _wiper = _wipersPool.Get();
            //Debug.Log("Wiper:" + _wiper.GetHashCode());
            //_wiper = new CheesePortion(Ratio, Position, startAngle, endAngle);
            _wiper.Ratio = Ratio;
            _wiper.Point = Position;
            _wiper.StartAngle = startAngle;
            _wiper.EndAngle = endAngle;
            _wiper.ResetTime(0);
            //_wiper.SetTime(0);
            _wiper.Auto = false;
            //_cheeseManager.AddArea(_wiper);
            _cheeseManager.AddAreaAndRemove(_wiper);
        }
    }

    private void DestroyWiper()
    {
        _wiper.Auto = true;
        //_wipersPool.Release(_wiper);
        _wiper = null;
    }

    void _cheeseManager_OnRemoveArea(CheesePortion area)
    {
        _wipersPool.Release(area);
    }   


    float UpdatePercent()
    {
        _accumTime += Time.deltaTime * _MovementSpeed;
        return _accumTime / (_CycleTime * 0.5f);
    }
}
