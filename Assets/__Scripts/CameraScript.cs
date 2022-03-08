using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraScript : MonoBehaviour
{
    //Public values
    public float maxOrthoSize = 20;
    public float minOrthoSize = 5;

    public bool dynamicallyFindHeroObject = true;       //If we want the script to find the Hero object, we set this to be true.

    //Target that the camera is following
    public GameObject target;

    //Private value. This is an intermediate value to set the camera's current positon to
    private Vector3 _position;

    void Start()
    {
        //Find the Hero object
        if (dynamicallyFindHeroObject) target = GameObject.Find("Hero");
    }

    // Update is called once per frame
    void Update()
    {
        // If the scrollwheel is used, increment or decriment the orthogonal size
        if (Input.GetKey(KeyCode.KeypadMinus) && GetComponent<Camera>().orthographicSize < maxOrthoSize) GetComponent<Camera>().orthographicSize += 0.5f;
        if (Input.GetKey(KeyCode.KeypadPlus) && GetComponent<Camera>().orthographicSize > minOrthoSize) GetComponent<Camera>().orthographicSize -= 0.5f;

        //Move the camera with the player
        _position.x = target.transform.position.x;
        _position.y = target.transform.position.y;
        _position.z = transform.position.z;
        transform.position = _position;        //Set the camera's position to the new position
    }
}
