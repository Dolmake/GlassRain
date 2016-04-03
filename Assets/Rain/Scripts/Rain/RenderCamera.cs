/*-------------------------------------------------
Created by: Daniel Peribáñez Sepúlveda
January 2013
peribanez.daniel@gmail.com

/**/

using UnityEngine;
using System.Collections;


/// <summary>
/// Interface to manage RenderCamera Prefab
/// </summary>
public class RenderCamera : MonoBehaviour
{
    public ParticleSystem ShowerParticle;
    public ParticleSystem RainParticle;
    public ParticleDropsPivot ShowerParticlesPivot; //Pivot which Shower turns around it
    public ContinousDrop ContinousDrop;             //Large drops

    public float _RainPercent = 0f;                  //Rain power percent
    public float ShowerMaxParticles = 1000f;        //Max # of particles in Shower Particle system
    public float _ShowerAngleDegrees = 0f;           //Angle in Shower Particle system      
    public float _ShowerStartSize = 0.5f;            //Particle Size in Shower Particle system
    public float _ShowerEmissionRate = 0;            //Emission rate in Shower Particle system   
    public float _VehicleSpeedPercent = 0;           //VehicleSpeed

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        ShowerParticle.startSize = _ShowerStartSize;
        ShowerParticle.emissionRate = _ShowerEmissionRate;
        ShowerParticle.startLifetime = ShowerMaxParticles / ShowerParticle.emissionRate;
        ShowerParticle.startSpeed = ShowerParticlesPivot.DistanceToPivot * 1.5f / ShowerParticle.startLifetime;
        //ShowerParticle.gravityModifier = -Percent(-0.01f, 0.3f, _VehicleSpeedPercent);

        ShowerParticlesPivot.AngleDegrees = _ShowerAngleDegrees;

        ContinousDrop._SpeedPercent = _VehicleSpeedPercent;
        //ContinousDrop._Direction = _VehicleSpeedPercent > 0.5 ? 1 : -1;
        ContinousDrop._RainPercent = _RainPercent;


        RainParticle.gravityModifier = -Percent(-0.01f, 0.3f, _VehicleSpeedPercent);
        RainParticle.emissionRate = Percent(0,500,_RainPercent);
        RainParticle.startLifetime = Percent(0.5f, 1f, 1f - _RainPercent);
        

    }

    float Percent(float min, float max, float percent)
    {
        return (max - min) * percent + min;
    }
}
