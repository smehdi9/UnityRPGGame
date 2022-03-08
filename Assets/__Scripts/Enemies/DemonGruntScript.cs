using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonGruntScript : EnemyScript
{
    //Demon's damage value
    public int gruntDamageValue = 10;

    //What to do on attack
    public override void OnAttack()
    {
        //Damage the player
        PlayerUserInferface.Health = PlayerUserInferface.Health - gruntDamageValue;
    }
}
