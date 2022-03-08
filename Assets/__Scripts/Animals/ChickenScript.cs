using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChickenScript : MobScript
{
    //Constants
    public float speed = 3f;         //Constant value to be modified using Unity editor (hence not set as a const)
    public Rigidbody2D chickenRB;
    public Animator chickenAnim;
    public bool dynamicallyFindHeroObject = true;       //If we want the script to find the Hero object, we set this to be true.
    public GameObject player;           //Reference to player object
    public int healthGain = 10;
    
    private PlayerUI _playerUI;         //Player UI object
    private Vector2 _movement;

    //The direction of chicken's movement. This is a work in progress.
    private float _angle = 0f;

    void Start()
    {
        //Find the Hero object
        if (dynamicallyFindHeroObject) player = GameObject.Find("Hero");
        _playerUI = player.GetComponent<PlayerUI>();

        //Initialize the chicken facing south
        chickenAnim.SetFloat("IdleVertical", -1);
    }

    // Update is called once per frame
    void Update()
    {
        //Sample motion to demonstrate the movement of the chicken
        _angle += 0.01f;
        _movement.x = (float) System.Math.Cos(_angle);
        _movement.y = (float)System.Math.Sin(_angle);

        //Manage player animation
        chickenAnim.SetFloat("Horizontal", _movement.x);
        chickenAnim.SetFloat("Vertical", _movement.y);
        chickenAnim.SetFloat("Speed", _movement.sqrMagnitude);
        //If the chicken starts moving, set their facing direction
        if (_movement.sqrMagnitude > 0)
        {
            chickenAnim.SetFloat("IdleHorizontal", _movement.x);
            chickenAnim.SetFloat("IdleVertical", _movement.y);
        }
    }

    //Fixed update for physics
    void FixedUpdate()
    {
        //Move the rigid body
        chickenRB.MovePosition(chickenRB.position + _movement * speed * Time.fixedDeltaTime);
    }

    //Inhereted function
    public override void OnDeath()
    {
        // Gain experience on monster death
        _playerUI.Health = _playerUI.Health + healthGain;
    }
}
