using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LizardAllyScript : NPCScript
{
    //Public numerators
    public float speed = 6.5f;

    public float standByDist = 3;
    public float maxTargetDistance = 8;
    public float minTargetDistance = 3;

    public int allyDamageValue = 5;

    public Rigidbody2D npcRB;
    public Animator npcAnim;

    public GameObject enemiesList;    //The parent object that holds all enemies (Using find objects by tag requires getting the list each time, we can simply access the list in this variable)
    private GameObject _target;       //Currently chasing target

    private Vector2 _movement;
    private bool _isAttacking = false;

    //The current action of the ally, names are self explanatory
    public enum AllyState { Idle, StandBy, Follow, Chase, Attack };
    private AllyState _state;

    void Start()
    {
        //Ensure that min is less than max
        if (minTargetDistance >= maxTargetDistance) minTargetDistance = maxTargetDistance / 5.0f;

        //Initialize the npc facing south
        npcAnim.SetFloat("IdleVertical", -1);

        _state = AllyState.Idle;

        if (enemiesList == null) enemiesList = GameObject.Find("Enemies");    //If null, try to find the list dynamically
    }

    // Update is called once per frame
    void Update()
    {
        //Activate
        if (_state == AllyState.Idle)
        {
            if (!triggerDialog) _state = AllyState.Follow;
            else return;
        }

        //Standby or follow
        if (Vector2.Distance(player.transform.position, transform.position) <= standByDist) _state = AllyState.StandBy;
        else if (Vector2.Distance(player.transform.position, transform.position) >= standByDist * 1.5) _state = AllyState.Follow;   //Multiplying to add a slight delay

        float distTarget = 0;

        //Ally has a target to attack
        if (_target != null)
        {
            distTarget = Vector2.Distance(transform.position, _target.transform.position);  //Distance to target

            //If the target has gotten too far away, stop chasing
            if (distTarget > maxTargetDistance)
            {
                _target = null;
            }
            //Otherwise, attack/attack
            else
            {
                if (distTarget >= minTargetDistance) _state = AllyState.Chase;
                else _state = AllyState.Attack;
            }
        }
        //If the ally has no target to attack, it looks for one
        else
        {
            if (enemiesList != null)
            {
                //Traverse list of enemies
                for(int i = 0; i < enemiesList.transform.childCount; i++)
                {

                    if (Vector2.Distance(transform.position, enemiesList.transform.GetChild(i).transform.position) <= maxTargetDistance)
                    {
                        _target = enemiesList.transform.GetChild(i).gameObject; //Found enemy that's close enough
                        _state = AllyState.Chase;
                        break;
                    }
                }
            }
        }


        //Apply ally state 
        switch (_state)
        {
            //Idling
            case AllyState.Idle:
                _movement.x = 0;
                _movement.y = 0;
                break;
            //Chasing the player. Set the demon's direction to be moving toward the player
            case AllyState.Follow:
                _movement.y = player.transform.position.y - transform.position.y;
                _movement.x = player.transform.position.x - transform.position.x;
                _movement.Normalize();          //Normalize the movement vector to be a unit vector
                break;
            case AllyState.Chase:
                if (_target != null)
                {
                    _movement.y = _target.transform.position.y - transform.position.y;
                    _movement.x = _target.transform.position.x - transform.position.x;
                    _movement.Normalize();          //Normalize the movement vector to be a unit vector
                }
                else
                {
                    _movement.x = 0;
                    _movement.y = 0;
                }
                break;
            case AllyState.Attack:
                _movement.x = 0;
                _movement.y = 0;
                break;
            case AllyState.StandBy:
                _movement.x = 0;
                _movement.y = 0;
                break;
            default:
                _movement.x = 0;
                _movement.y = 0;
                break;
        }

        //Manage ally animation
        if (_state == AllyState.Attack && _target != null)
        {
            npcAnim.SetFloat("Attack", 1);
        }
        else npcAnim.SetFloat("Attack", 0);

        npcAnim.SetFloat("Horizontal", _movement.x);
        npcAnim.SetFloat("Vertical", _movement.y);
        npcAnim.SetFloat("Speed", _movement.sqrMagnitude);

        //If the ally starts moving, set their facing direction
        if (_movement.sqrMagnitude > 0)
        {
            npcAnim.SetFloat("IdleHorizontal", _movement.x);
            npcAnim.SetFloat("IdleVertical", _movement.y);
        }

        if(_target != null)
        {
            //If in attack state and near target. Cause damage
            if (npcAnim.GetCurrentAnimatorStateInfo(0).IsName("AttackState") && distTarget <= minTargetDistance && !_isAttacking && !(npcAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1))
            {
                //Set the direction of the mob
                npcAnim.SetFloat("IdleHorizontal", _target.transform.position.x - transform.position.x);
                npcAnim.SetFloat("IdleVertical", _target.transform.position.y - transform.position.y);

                OnAttack(); //Perform attack action
                _isAttacking = true;
            }

            //Check if the animation has ended
            else if (npcAnim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1 && npcAnim.GetCurrentAnimatorStateInfo(0).IsName("AttackState") && _isAttacking)
            {
                npcAnim.SetFloat("Attack", 0);
                _isAttacking = false;
            }
        }
    }

    //Fixed update for physics
    void FixedUpdate()
    {
        //Move the rigid body
        npcRB.MovePosition(npcRB.position + _movement * speed * Time.fixedDeltaTime);
    }

    //On attack, perform this function
    void OnAttack()
    {
        MobScript mobController = _target.GetComponent<MobScript>();
        mobController.Health = mobController.Health - allyDamageValue;
    }
}
