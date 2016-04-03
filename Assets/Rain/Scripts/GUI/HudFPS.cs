/*-------------------------------------------------
Created by: Daniel Peribáñez Sepúlveda
October 2011
peribanez.daniel@gmail.com

/**/

using UnityEngine;
using System.Collections;

public class HudFPS : MonoBehaviour {

    string _fpsLabel = "None";
    int _fpsCounter = 0;
    float _accumTime = 0;
    float MAX_TIME = 1.0f;
    public Rect RectLabel = new Rect(0,0, 128,128);
    public GUIStyle Style = new GUIStyle();

	// Use this for initialization
	void Start () {
        
//#if !UNITY_EDITOR
//        this.enabled = false;
//#endif
	
	}
	
	// Update is called once per frame
	void Update () {

        _fpsCounter++;
        _accumTime += Time.deltaTime;
        if (_accumTime > MAX_TIME)
        {
            _fpsLabel = _fpsCounter.ToString();
            _accumTime = 0;
            _fpsCounter = 0;
        }
	
	}
    void OnGUI()
    {
        GUI.Label(RectLabel, _fpsLabel, Style);
        
        
    }
}
