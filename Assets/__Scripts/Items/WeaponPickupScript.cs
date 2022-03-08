using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponPickupScript : ItemScript
{
    public GameObject player;          //Reference to player
    public string type;
    public int damageValue;
    public int minimumExperienceRequired = 0;           //Minimum required combat value

    public bool dynamicallyFindHeroObject = true;       //If we want the script to find the Hero object, we set this to be true.

    // Start is called before the first frame update
    void Start()
    {
        //Find the Hero object
        if (dynamicallyFindHeroObject) player = GameObject.Find("Hero");
    }

    //Add the current item to the inventory of the player
    public override void PickupAction()
    {
        player.GetComponent<PlayerMovementScript>().CreateObjectFromPickup(this.gameObject, type, damageValue);
    }

    //If the player has enough experience, allow pick up
    public override bool AllowPickup()
    {
        return player.GetComponent<PlayerUI>().Experience >= minimumExperienceRequired;
    }
}
