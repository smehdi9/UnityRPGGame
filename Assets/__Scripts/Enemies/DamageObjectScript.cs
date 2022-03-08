using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageObjectScript : MonoBehaviour
{
    //Public variables
    public int objectDamageValue;
    public Rigidbody2D rb;
    public int speed = 10;
    public bool destroyOnContact = true;

    public bool dynamicallyFindHeroObject = true;
    public GameObject player;

    private PlayerUI _playerUI;             //Player UI object
    private Vector2 _movement;              //Movement vector
    private bool _damagedPlayer = false;    //To avoid damaging player twice

    public float timeToLive = 0.5f;       //Seconds before the object will self destruct

    void Start()
    {
        //Find the Hero object
        if (dynamicallyFindHeroObject) player = GameObject.Find("Hero");
        _playerUI = player.GetComponent<PlayerUI>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //Update position
        rb.MovePosition(rb.position + _movement * speed * Time.fixedDeltaTime);

        //Set the time to live for the object if exists
        if (timeToLive > 0)
        {
            StartCoroutine(SetTTL(timeToLive));
            timeToLive = 0;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        //Ensure that this function only operates if the collider is the player
        //Only perform this section if the player is allowed to pick the item up
        if (other.tag.Equals("Player") && !_damagedPlayer)
        {
            //Damage player
            _playerUI.Health = _playerUI.Health - objectDamageValue;
            _damagedPlayer = true;
        }

        if(!other.tag.Equals("Mob") && !other.tag.Equals("Item") && destroyOnContact) Destroy(gameObject);        //Destroy on impact with any collider (except other mobs)
    }

    IEnumerator SetTTL(float ttl)
    {
        if (ttl < 0) ttl = 0;
        yield return new WaitForSeconds(ttl);
        Destroy(gameObject);
    }

    public Vector2 Movement
    {
        get { return _movement; }
        set
        {
            _movement = value;
        }
    }

}
