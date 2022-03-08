using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossActivationScript : MonoBehaviour
{
    public GameObject player;
    public bool dynamicallyFindHeroObject = true;       //If we want the script to find the Hero object, we set this to be true.

    public int experienceRequired = 300;
    public bool isEnabled = true;

    private bool _actionCompleted = false;

    private PlayerUI _playerUI;

    // Start is called before the first frame update
    void Awake()
    {
        if (dynamicallyFindHeroObject) player = GameObject.Find("Hero");
        _playerUI = player.GetComponent<PlayerUI>();    //Set player UI
    }

    //Update
    void Update()
    {
        //Check if the player has reached the minimum required experience. If they do, remove self
        if ((_playerUI.Experience >= experienceRequired) && !_actionCompleted)
        {
            ActionCompleted = true;
            PerformAction();
        }
    }

    //virtual function that may need to be performed if the user reaches required exp.
    public virtual void PerformAction()
    {
        //Implemented by children
    }

    public bool ActionCompleted
    {
        get { return _actionCompleted; }
        set
        {
            if (!_actionCompleted) _actionCompleted = value;
        }
    }
}
