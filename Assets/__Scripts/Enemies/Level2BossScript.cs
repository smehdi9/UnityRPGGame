using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level2BossScript : EnemyScript
{
    //Demon's damage value
    public int bossDamageValue = 60;

    //What to do on attack
    public override void OnAttack()
    {
        //Damage the player
        PlayerUserInferface.Health = PlayerUserInferface.Health - bossDamageValue;
    }
}
