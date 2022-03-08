using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MobScript : MonoBehaviour
{
    public int maxHealth = 100;
    public float colorChangeDislayDuration = 0.02f;
    public float deathDelay = 0;

    private int _healthCounter;     //Health of the mob

    //To avoid race conditions, we use Awake
    void Awake()
    {
        _healthCounter = maxHealth;     //Initialize health to max
    }

    //Check if demon is dead
    public bool IsDead()
    {
        return Health <= 0;
    }

    //Virtual method for what to do on mob death
    public virtual void OnDeath()
    {
        //Implemented by children
    }

    //Change the color of the sprite on damage, and then return it to default after given time
    IEnumerator DamageDisplay(float time)
    {
        this.GetComponent<SpriteRenderer>().color = new Color(1, 0, 0, 1);
        yield return new WaitForSeconds(time);
        this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    //Change the color of the sprite, and then return it to default after given time
    IEnumerator RegenDisplay(float time)
    {
        this.GetComponent<SpriteRenderer>().color = new Color(0, 1, 0, 1);
        yield return new WaitForSeconds(time);
        this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    //Coroutine for death
    IEnumerator MobDeath(float time)
    {
        OnDeath();
        yield return new WaitForSeconds(time);
        Destroy(this.gameObject);
    }

    //Property for health. Ensure no illegal values for health are passed
    public int Health
    {
        get { return _healthCounter; }
        set
        {
            if (!IsDead())
            {
                int newVal = value;
                if (value <= 0)
                {
                    StartCoroutine(MobDeath(deathDelay));                               //If the enemy's health goes below 100, our enemy will be destroyed
                    newVal = 0;
                }

                if (newVal <= maxHealth)         //If the value does change, display the change
                {
                    if (newVal < _healthCounter) StartCoroutine(DamageDisplay(colorChangeDislayDuration));       //Taking damage
                    if (newVal > _healthCounter) StartCoroutine(RegenDisplay(colorChangeDislayDuration));        //Regenerating health
                    _healthCounter = newVal;
                }
            }
        }
    }
}
