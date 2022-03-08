using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DynamicObjectOrderScript : MonoBehaviour
{

    //Private objects
    private SpriteRenderer _sprite;

    void Start()
    {
        //Get the sprite object
        _sprite = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        //Manage sorting order to perform depth in game. Dyanamically done as objects change positions
        _sprite.sortingOrder = (int)((transform.position.y - _sprite.bounds.size.y /2) * -100);
    }
}
