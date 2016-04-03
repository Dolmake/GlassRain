/*-------------------------------------------------
Created by: Daniel Peribáñez Sepúlveda
January 2013
peribanez.daniel@gmail.com

/**/

using UnityEngine;
using System.Collections;
using GlassUtils;

/// <summary>
/// Manage the Rain using RainPower y Car Velocity.
/// </summary>
[RequireComponent(typeof(CheeseManager))]
public class GlassManager : MonoBehaviour { 
    public static float MAX_SPEED = 140f;
    public static float MAX_RAIN_POWER = 3f;
    public static float MAX_DISTORTION = 300.0f;
    public static float EMISSION_FACTOR = 5.0f;
    public static float MAX_PARTICLE_ANGLE_DEGREES = 160;
    public static float MAX_PARTICLE_SIZE = 0.5f;
   
    public float RainPower {get;set;}
    public float VehicleSpeed {get;set;}
    public WindshieldSpeed WindShieldSpeed { get; set; }

    public RenderCamera RenderCamera;

    float _maxSpeed_inverse = 1f / MAX_SPEED;
    float _maxRainPower_inverse = 1f / MAX_RAIN_POWER;
    CheeseManager _rainManager = null;
  
    Wiper[] _windscreens = null;

	// Use this for initialization
	void Start () {
     
	}

    void OnEnable()
    {
        _rainManager = GetComponent<CheeseManager>();        
        _windscreens = GetComponents<Wiper>() as Wiper[];
        foreach (Wiper w in _windscreens)
            w.ExternalManagement = true;
    }

   

	// Update is called once per frame
	void Update () {
       
        _rainManager._DeltaSpeed = RainPower * _maxRainPower_inverse;
        _rainManager._Distortion = RainPower < 1 ? MAX_DISTORTION * RainPower : MAX_DISTORTION;
        _rainManager._BumpMapDistortion = RainPower < 1 ? 0 : Lerp(1.0f, MAX_RAIN_POWER, RainPower);
        //Debug.Log("_BumpMapDistortion:" + _rainManager._BumpMapDistortion);
        //Debug.Log("RainPower:" + RainPower);

        RenderCamera._ShowerStartSize = MAX_PARTICLE_SIZE * (RainPower * _maxRainPower_inverse);
        RenderCamera._ShowerEmissionRate = EMISSION_FACTOR * (RainPower + 1);        
        RenderCamera._ShowerAngleDegrees = -MAX_PARTICLE_ANGLE_DEGREES * VehicleSpeed * _maxSpeed_inverse;
        RenderCamera._VehicleSpeedPercent = VehicleSpeed * _maxSpeed_inverse;
        RenderCamera._RainPercent = RainPower * _maxRainPower_inverse;

        float deltaTime = Time.deltaTime;

        //We change the Windscreen Speed only if are stopped
        bool allWindScreenAreStopped = false;
        foreach (Wiper w in _windscreens)
        {
            allWindScreenAreStopped |= w._CurrentDir == WindshieldDirection.STOPPED;
            //Updating the Windscreen movement
            w.Update(deltaTime);
        }
        if (allWindScreenAreStopped)
            UpdateWindShieldSpeed(WindShieldSpeed);
        
	}

    /// <summary>
    /// Update the Speed of Windscreen
    /// </summary>
    /// <param name="speed"></param>
    private void UpdateWindShieldSpeed(WindshieldSpeed speed)
    {
        switch (speed)
        {
            case WindshieldSpeed.NO:
                foreach (Wiper w in _windscreens)
                {
                    w._accumTime = 0;
                    w._MovementSpeed = 0f;
                    w._CycleTime = 1f;
                    w._StopTime = 1f;
                }
                break;
            case WindshieldSpeed.SLOW:
                foreach (Wiper w in _windscreens)
                {
                    w._MovementSpeed = 1f;
                    w._StopTime = 1f;
                    w._CycleTime = 2f;
                }
                break;
            case WindshieldSpeed.NORMAL:
                foreach (Wiper w in _windscreens)
                {
                    w._MovementSpeed = 1f;
                    w._StopTime = 0.5f;
                    w._CycleTime = 1f;
                }
                break;
            case WindshieldSpeed.FAST:
                foreach (Wiper w in _windscreens)
                {
                    w._MovementSpeed = 1f;
                    w._StopTime = 0f;
                    w._CycleTime = 0.75f;
                }
                break;
        }
    }

    float Lerp(float from, float to, float value)
    {
        return (value - from) / (to - from);
    }

}
