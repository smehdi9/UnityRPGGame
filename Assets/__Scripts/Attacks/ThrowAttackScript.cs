using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowAttackScript : AttackScript
{ 
    private Vector2 _movement;
    private bool _rotate;

    public Rigidbody2D throwRB;               // Reference to rigidbody
    public float speed = 15;
    public float rotationSpeed = 300;

    public void SetSpeed(float x, float y, bool r)  // Set projectile speed values
    {
        _movement = new Vector2(x, y);
        _rotate = r;
        _movement.Normalize();
    }

    private void FixedUpdate()
    {
        //Update position
        throwRB.MovePosition(throwRB.position + _movement * speed * Time.fixedDeltaTime);

        if (_rotate) transform.Rotate(0f, 0f, rotationSpeed * Time.fixedDeltaTime);
    }
}
