using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemonExplodeScript : EnemyScript
{
    public float maxFuseValue = 300f;         //Fuse value required to explode
    public float fuseIncrement = 30f;         //Increment per frame
    public float fuseDecrement = 10f;         //Decrement per frame
    public float flashDuration = 0.001f;      //Duration per flash color

    private float _fuse = 0;                    //The current fuse count. If it reaches fuseDelay, enemy explodes
    private bool _colorMode = true;             //True = Yellow, False = None. Used to implement flashing when in attack mode
    private bool _inFlashCoroutine = false;     //To avoid starting more coroutines if we are currently in one

    public GameObject explosionPrefab;          //Spawn object on explode

    //Start is called first
    void Start()
    {
        FreezeAttack = true;

        //Ensure that min is less than max
        if (fuseDecrement >= fuseIncrement) fuseDecrement = fuseIncrement / 5.0f;
    }

    //Late update to avoid race conditions
    void LateUpdate()
    {
        //If close to player
        if(Vector2.Distance(transform.position, player.transform.position) <= minDist) {
            OnAttack();
        }
        else CurrentFuse -= fuseDecrement * Time.fixedDeltaTime;

        //If the enemy's fuse is greater than 0, keep him in attack until fuse is depleted
        if (CurrentFuse > 0)
        {
            demonAnim.SetFloat("Attack", 1);
            if (this.CurrentState != DemonState.Attack && this.Health > 0)
            {
                this.CurrentState = DemonState.Attack;
            }

            //Flash explosion
            if (!_inFlashCoroutine) StartCoroutine(FlashVisual(flashDuration));
        }

        //Once the max fuse is reached, explode
        if (CurrentFuse >= maxFuseValue) Explode();
    }

    //Explosion
    void Explode()
    {
        //Spawn explosion object
        GameObject explosion = Instantiate(explosionPrefab) as GameObject;
        explosion.transform.position = transform.position;

        DestroyHealthBar();         //Destroy healthbar
        Destroy(this.gameObject);
    }

    //Implemented function
    public override void OnAttack()
    {
        //While attacking, set the fuse value higher
        CurrentFuse += fuseIncrement * Time.fixedDeltaTime;
    }

    //Flash the color of the demon
    IEnumerator FlashVisual(float time)
    {
        _inFlashCoroutine = true;

        //While the fuse is not 0
        while(CurrentFuse > 0)
        {
            //Increase rate as fuse gets higher
            float newTime = time * (maxFuseValue - CurrentFuse);
            if (newTime <= 0) newTime = 0.001f;

            yield return new WaitForSeconds(newTime);
            if(_colorMode) this.GetComponent<SpriteRenderer>().color = new Color(1, 0.568f, 0, 1);
            else this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
            _colorMode = !_colorMode;   //Swap color mode
        }

        //Reset
        _inFlashCoroutine = false;
        _colorMode = false;
        this.GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 1);
    }

    //Properties
    //Fuse time
    public float CurrentFuse
    {
        get { return _fuse; }
        set
        {
            //Clamp value above 0
            if (value <= 0) _fuse = 0;
            else _fuse = value;
        }
    }
}
