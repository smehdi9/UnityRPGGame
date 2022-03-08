using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemScript : MonoBehaviour
{
    //Public values
    public float rotationSpeed = 30f;
    public bool destroyOnPickup = true;

    public static float ANGLE;       //Current angle of rotation. Static to maintain constance throughout code

    // Update is called once per frame
    void FixedUpdate()
    {
        //Constantly rotate a collectable item on the y-axis
        ANGLE += rotationSpeed * Time.fixedDeltaTime;
        if (ANGLE > 360) ANGLE -= 360;

        transform.eulerAngles = new Vector3(0f, ANGLE, 0f);
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Ensure that this function only operates if the collider is the player
        //Only perform this section if the player is allowed to pick the item up
        if (other.tag.Equals("Player") && AllowPickup())
        {
            PickupAction();
            if(destroyOnPickup) Destroy(gameObject);          
        }
    }

    //Virtual methods
    //Perform an action if the object is picked up. Implemented by child classes.
    public virtual void PickupAction()
    {
        //implemented by child classes
    }

    //Check if pick up is allowed. This function is implemented by the child classes as they determine when a pick up is allowed.
    public virtual bool AllowPickup()
    {
        //implemented by child classes
        return true;
    }

}
