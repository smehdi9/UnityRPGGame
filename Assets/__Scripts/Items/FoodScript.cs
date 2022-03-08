using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodScript : ItemScript
{
    public GameObject player;       //Reference to player
    public int foodPointValue;
    public int foodHealthValue;

    private PlayerUI _playerUI;

    public bool dynamicallyFindHeroObject = true;        //If we want the script to find the Hero object, we set this to be true.

    // Start is called before the first frame update
    void Start()
    {
        //Find the Hero object
        if (dynamicallyFindHeroObject) player = GameObject.Find("Hero");
        _playerUI = player.GetComponent<PlayerUI>();
    }

    //If picked up, increment the player's health and experience
    public override void PickupAction()
    {
        _playerUI.Experience = _playerUI.Experience + foodPointValue;
        _playerUI.Health = _playerUI.Health + foodHealthValue;
    }

    //Only allow pick up if player is damaged
    public override bool AllowPickup()
    {
        return !_playerUI.HasFullHP;
    }
}
