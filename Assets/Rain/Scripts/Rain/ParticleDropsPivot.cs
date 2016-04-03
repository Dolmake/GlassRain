using UnityEngine;
using System.Collections;

public class ParticleDropsPivot : MonoBehaviour {
    
    public Transform Pivot;
    public float AngleDegrees;
    float _currentAngleDegrees;


    public float DistanceToPivot { get; private set; }

	// Use this for initialization
	void Start () {
        if (!Pivot)
            Debug.LogError("ParticleDropsPivot-> No Pivot Founded");
        
        _currentAngleDegrees = Pivot.eulerAngles.y * Mathf.Rad2Deg;
        DistanceToPivot = (this.transform.position - Pivot.position).magnitude;
	}
	
	// Update is called once per frame
	void Update () {
        transform.RotateAround(Pivot.position, Vector3.right, _currentAngleDegrees - AngleDegrees);
        _currentAngleDegrees = AngleDegrees;
	}
}
