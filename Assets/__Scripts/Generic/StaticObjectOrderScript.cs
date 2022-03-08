using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticObjectOrderScript : MonoBehaviour
{
    //Set the sorting order at the start of the scene
    void Start()
    {
        //Manage sorting order to perform depth in game. Done once at the beginning of the scene
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        sprite.sortingOrder = (int)((transform.position.y - sprite.bounds.size.y / 2) * -100);
    }
}
