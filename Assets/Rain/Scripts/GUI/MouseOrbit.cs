using UnityEngine;
using System.Collections;


public class MouseOrbit : MonoBehaviour
{

    public static bool ALLOW_MOUSE_ROTATION = true;

    public Transform target;
    public float distance = 5.0f;
    public float xSpeed = 120.0f;
    public float ySpeed = 120.0f;

    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    public float distanceMin = .5f;
    public float distanceMax = 15f;

    float x = 0.0f;
    float y = 0.0f;

    // Use this for initialization
    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;

        // Make the rigid body not change rotation
        if (GetComponent<Rigidbody>())
            GetComponent<Rigidbody>().freezeRotation = true;
    }

    bool _mousePressed = false;   
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            _mousePressed = true;
            //Debug.Log("MOuse pressed:" + _mousePressed);
        }
        else if (Input.GetMouseButtonUp(0))
        {
            _mousePressed = false;
            //Debug.Log("MOuse pressed:" + _mousePressed);
        }
       
       
    }

    void LateUpdate()
    {
        if (target)
        {
            if (_mousePressed && ALLOW_MOUSE_ROTATION)
            {
                x += Input.GetAxis("Mouse X") * xSpeed * distance * 0.02f;
                y -= Input.GetAxis("Mouse Y") * ySpeed * 0.02f;
            }

            y = ClampAngle(y, yMinLimit, yMaxLimit);

            Quaternion rotation = Quaternion.Euler(y, x, 0);

            distance = Mathf.Clamp(distance - Input.GetAxis("Mouse ScrollWheel") * 5, distanceMin, distanceMax);

            //RaycastHit hit;
            bool hitted = false;

            //////Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);            
            //////if (Physics.Raycast(ray, out hit, 100))          
            //////{              
            //////     hitted = true;
            //////     RaycastHit intermediate;
            //////     if (Physics.Linecast(target.position, transform.position, out intermediate))
            //////     {
            //////         distance -= intermediate.distance;
            //////     }
            //////}
            hitted = true;

            Vector3 negDistance = new Vector3(0.0f, 0.0f, -distance);
            Vector3 position = rotation * negDistance + target.position;
            if (hitted)
            {
                transform.rotation = rotation;
                transform.position = position;
            }

        }

    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }


}