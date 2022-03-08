using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RelicScript : ItemScript
{
    public GameObject player;       //Reference to player
    public int relicPointValue;

    private PlayerUI _playerUI;

    public bool dynamicallyFindHeroObject = true;        //If we want the script to find the Hero object, we set this to be true.

    // Start is called before the first frame update
    void Start()
    {
        //Find the Hero object
        if (dynamicallyFindHeroObject) player = GameObject.Find("Hero");
        _playerUI = player.GetComponent<PlayerUI>();
    }

    //The player's experience is increased by the relic's value
    public override void PickupAction()
    {
        _playerUI.Experience = _playerUI.Experience + relicPointValue;
    }

    //We do shall always allow the player to pick up relics
    public override bool AllowPickup()
    {
        return true;
    }
}
