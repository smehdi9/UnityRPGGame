using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TownsMenScript : NPCScript
{
    public Animator anim;   //Animator object

    void Update()
    {
        //Change the direction based on the player's location
        Vector2 direction = new Vector2(player.transform.position.x - transform.position.x, player.transform.position.y - transform.position.y);
        direction.Normalize();

        //Set animation
        anim.SetFloat("Horizontal", direction.x);
        anim.SetFloat("Vertical", direction.y);
    }
}
