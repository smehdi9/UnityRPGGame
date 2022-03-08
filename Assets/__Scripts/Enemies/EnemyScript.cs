using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyScript : MobScript
{
    //Public numerators
    public float speed = 5;
    public float maxDist = 10;
    public float minDist = 2;
    public int enemyExperience = 10;

    public bool dynamicallyFindHeroObject = true;       //If we want the script to find the Hero object, we set this to be true.

    public Rigidbody2D demonRb;
    public Animator demonAnim;
    public GameObject player;                        //Reference to player object

    public GameObject healthBarPrefab;               //Health bar of the mob

    public bool showHealthBar = true;                //Whether or not to show the healthbar associated with the mob
    public int healthBarOffset = 3;                  //How far above to display health
    private GameObject _healthBar;

    private Vector2 _movement;                      //Motion of the Demon

    private bool _isAttacking = false;
    private bool _freezeAttack = false;

    protected PlayerUI PlayerUserInferface;           //Protected variable to be accessed in child classes. The naming convention for protected variables is CamelCase with first character uppercase.

    //The current action of the demon, names are self explanatory
    public enum DemonState { Idle, Roam, Chase, Attack };
    private DemonState _state;

    //Using on enable to establish order between executions of Awake OnEnable and Start. Race conditions
    void OnEnable()
    {
        //Ensure that min is less than max
        if (minDist >= maxDist) minDist = maxDist / 5.0f;

        //Spawn healthbar
        if (showHealthBar)
        {
            _healthBar = Instantiate(healthBarPrefab) as GameObject;
            _healthBar.GetComponent<SpriteRenderer>().color = new Color(((float)this.maxHealth - this.Health) / this.maxHealth, ((float)this.Health) / this.maxHealth, 0, 1);
            _healthBar.transform.localScale = new Vector2((float)this.Health / this.maxHealth, 1);
            _healthBar.transform.position = new Vector2(transform.position.x, transform.position.y + healthBarOffset);
        }

        //Find the Hero object
        if (dynamicallyFindHeroObject) player = GameObject.Find("Hero");
        PlayerUserInferface = player.GetComponent<PlayerUI>();

        //Initialize the demon facing south and idling
        demonAnim.SetFloat("IdleVertical", -1);
        _state = DemonState.Idle;
    }

    void Update()
    {
        if (IsDead()) return;   //If demon is dead, do nothing.

        //Health bar updates
        if(showHealthBar)
        {
            _healthBar.GetComponent<SpriteRenderer>().color = new Color(((float)this.maxHealth - this.Health) / this.maxHealth, ((float)this.Health) / this.maxHealth, 0, 1);
            _healthBar.transform.localScale = new Vector2((float)this.Health / this.maxHealth, 1);
            _healthBar.transform.position = new Vector2(transform.position.x, transform.position.y + healthBarOffset);
        }

        float distPlayer = Vector2.Distance(player.transform.position, transform.position);
        //Manage demon state
        if (distPlayer > maxDist) _state = DemonState.Idle;
        else if (distPlayer <= maxDist && distPlayer >= minDist) _state = DemonState.Chase;
        else _state = DemonState.Attack;

        //Apply demon state 
        switch (_state)
        {
            //Idling
            case DemonState.Idle:
                _movement.x = 0;
                _movement.y = 0;
                break;
            //Chasing the player. Set the demon's direction to be moving toward the player
            case DemonState.Chase:
                _movement.y = player.transform.position.y - transform.position.y;
                _movement.x = player.transform.position.x - transform.position.x;
                _movement.Normalize();          //Normalize the movement vector to be a unit vector
                break;
            case DemonState.Attack:
                _movement.x = 0;
                _movement.y = 0;
                break;
            default:
                _movement.x = 0;
                _movement.y = 0;
                break;
        }

        //Manage enemy animation
        if (_state == DemonState.Attack)
        {
            //Only attack if the player is alive
            if (PlayerUserInferface.IsDead) _state = DemonState.Idle;
            else demonAnim.SetFloat("Attack", 1);
        }
        else demonAnim.SetFloat("Attack", 0);
        demonAnim.SetFloat("Horizontal", _movement.x);
        demonAnim.SetFloat("Vertical", _movement.y);
        demonAnim.SetFloat("Speed", _movement.sqrMagnitude);
        //If the demon starts moving, set their facing direction
        if (_movement.sqrMagnitude > 0)
        {
            demonAnim.SetFloat("IdleHorizontal", _movement.x);
            demonAnim.SetFloat("IdleVertical", _movement.y);
        }

        //If in attack state and near player. Cause damage
        if (distPlayer <= minDist && (_freezeAttack || (demonAnim.GetCurrentAnimatorStateInfo(0).IsName("AttackState") && !_isAttacking && !AnimationHasEnded())))
        {
            //Set the direction of the mob
            demonAnim.SetFloat("IdleHorizontal", player.transform.position.x - transform.position.x);
            demonAnim.SetFloat("IdleVertical", player.transform.position.y - transform.position.y);

            OnAttack(); //Perform attack action
            _isAttacking = true;
        }

        //Check if the animation has ended
        else if (!_freezeAttack && AnimationHasEnded() && demonAnim.GetCurrentAnimatorStateInfo(0).IsName("AttackState") && _isAttacking)
        {
            demonAnim.SetFloat("Attack", 0);
            _isAttacking = false;
        }
    }

    //Fixed update for physics
    void FixedUpdate()
    {
        if (IsDead()) _movement = new Vector2(0, 0);   //If demon is dead, do nothing.

        //Update position
        demonRb.MovePosition(demonRb.position + _movement * speed * Time.fixedDeltaTime);
    }

    //Get if the current animation is playing
    bool AnimationHasEnded()
    {
        return demonAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1;
    }

    //Destroy healthbar function
    public void DestroyHealthBar()
    {
        Destroy(this._healthBar);
    }

    //Inhereted function
    public override void OnDeath()
    {
        showHealthBar = false;
        DestroyHealthBar();    //Destroy the health object

        // Gain experience on monster death
        PlayerUserInferface.Experience = PlayerUserInferface.Experience + enemyExperience;

        //Fix collider on death        
        this.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
        //Set animation
        demonAnim.SetBool("Dead", true);
    }

    //Inherited functionality, what action to perform on attack
    public virtual void OnAttack()
    {
        //To be implemented by children
    }

    //Property to get state. No setting
    public DemonState CurrentState
    {
        get { return _state; }
        set
        {
            _state = value;
        }
    }

    //Property to get if the attack must freeze or not
    public bool FreezeAttack
    {
        get { return _freezeAttack; }
        set
        {
            _freezeAttack = value;
        }
    }
}
