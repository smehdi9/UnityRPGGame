using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasSingletonScript : MonoBehaviour
{
    static public CanvasSingletonScript S;  //Singleton

    void Awake()
    {
        //Singleton
        if (S == null)
        {
            S = this;
        }
        else
        {
            print("Trying to create another Singlton instacne");
        }

        Vector3 hPos = CanvasSingletonScript.S.transform.position;

        DontDestroyOnLoad(gameObject);      //Ensure that the canvas cannot be destroyed
    }

}
