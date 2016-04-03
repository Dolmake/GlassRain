/*-------------------------------------------------
Created by: Daniel Peribáñez Sepúlveda
January 2013
peribanez.daniel@gmail.com

/**/

using UnityEngine;
using System.Collections;
using GlassUtils;

namespace GlassGUI
{
    //[RequireComponent(typeof(IGlassManager))]
    public class GUIGlass : MonoBehaviour
    {

        public float RainPower = 1f;
        public float VehicleSpeed = 0f;
        public float WindshieldSpeed = 0f;

        public GlassManager _glassManager = null;

        // Use this for initialization
        void Start()
        {
            //_glassManager = GetComponent(typeof(GlassManager)) as GlassManager;

        }

        // Update is called once per frame
        void Update()
        {

        }

        void OnGUI()
        {
            bool mouseOver = false;
            GUI.Label(GUIUtils.GetRectangle_MOUSE_ORBIT(25, 15, 100, 30, ref mouseOver), "Rain Power:" + (int)_glassManager.RainPower);
            _glassManager.RainPower = GUI.HorizontalSlider(GUIUtils.GetRectangle_MOUSE_ORBIT(25, 25, 100, 30, ref mouseOver), _glassManager.RainPower, 0.0f, 3.0f);

            GUI.Label(GUIUtils.GetRectangle_MOUSE_ORBIT(25, 45, 150, 30, ref mouseOver), "Drop Direction:" + (int)_glassManager.VehicleSpeed);
            _glassManager.VehicleSpeed = GUI.HorizontalSlider(GUIUtils.GetRectangle_MOUSE_ORBIT(25, 55, 100, 30, ref mouseOver), _glassManager.VehicleSpeed, -140.0f, 140.0f);

            //GUI.Label(GUIUtils.GetRectangle_MOUSE_ORBIT(25, 45, 150, 30, ref mouseOver), "Vehicle Speed:" + (int)_glassManager.VehicleSpeed);
            //_glassManager.VehicleSpeed = GUI.HorizontalSlider(GUIUtils.GetRectangle_MOUSE_ORBIT(25, 55, 100, 30, ref mouseOver), _glassManager.VehicleSpeed, 0.0f, 140.0f);


            //GUI.Label(GUIUtils.GetRectangle_MOUSE_ORBIT(25, 75, 150, 30, ref mouseOver), "Windshield:" + (int)_glassManager.WindShieldSpeed);
            //_glassManager.WindShieldSpeed = (WindshieldSpeed)(int)GUI.HorizontalSlider(GUIUtils.GetRectangle_MOUSE_ORBIT(25, 85, 100, 30, ref mouseOver), (int)_glassManager.WindShieldSpeed, 0, 3);


            MouseOrbit.ALLOW_MOUSE_ROTATION = !mouseOver;
        }
    }
}
