using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonSkeletonScript : EnemyScript
{
    public GameObject arrowPrefab;
    public int arrowTimeToLive = 3;

    //What to do on attack
    public override void OnAttack()
    {
        //Spawn the arrow moving in the direction of the player
        GameObject arrow = Instantiate(arrowPrefab) as GameObject;

        //Set object values
        arrow.GetComponent<DamageObjectScript>().Movement = player.transform.position - transform.position;
        arrow.GetComponent<DamageObjectScript>().Movement.Normalize();
        arrow.GetComponent<DamageObjectScript>().timeToLive = arrowTimeToLive;

        //Set the angle
        int dir = arrow.GetComponent<DamageObjectScript>().Movement.x < 0 ? 1 : 0;
        arrow.transform.eulerAngles = new Vector3(0, 0, (float)System.Math.Atan(arrow.GetComponent<DamageObjectScript>().Movement.y / arrow.GetComponent<DamageObjectScript>().Movement.x) * 180/3.14159f + 180 * dir);

        //Set the location
        arrow.transform.position = transform.position;
    }
}
