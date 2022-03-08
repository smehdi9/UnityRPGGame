using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class WorldDoorScript : MonoBehaviour
{
    private bool _canEnter = true;          //If the user can enter (Disable after one use)

    //When the player walks into the door
    void OnTriggerEnter2D(Collider2D other)
    {
        if(other.tag.Equals("Player") && _canEnter)
        {
            //Only if the player has the key
            if(other.gameObject.GetComponent<PlayerUI>().HasKey)
            {
                SceneManager.LoadScene("Level2", LoadSceneMode.Single);
                _canEnter = false;
            }
        }
    }
}
