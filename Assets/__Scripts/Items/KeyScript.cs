using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyScript : ItemScript
{
    //When player picks it up, set the player's has key value to true. This function overwrites the original ItemScript function for OnTrigger. But we utilize the other functions it provides
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag.Equals("Player"))
        {
            print("You have picked up the key");
            other.gameObject.GetComponent<PlayerUI>().HasKey = true;    //Set has key to true
            Destroy(gameObject);                                        //Destroy self
        }
    }
}
