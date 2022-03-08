using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSpawner : MonoBehaviour
{
    public int distance = 3;
    public GameObject playerPrefab;
    public GameObject uiPrefab;

    //Call initially, to spawn the player in the world
    void Awake()
    {
        //Find the player object, and set their location to where the level is
        GameObject player = GameObject.Find("Hero");
        GameObject ui = GameObject.Find("UI");

        //If no UI is found, create the UI.
        if (ui == null)
        {
            ui = Instantiate(uiPrefab) as GameObject;
        }

        //If no player is found, create a player
        if (player == null)
        {
            player = Instantiate(playerPrefab) as GameObject;

            GameObject mainCamera = GameObject.Find("MainCamera");
            mainCamera.GetComponent<CameraScript>().target = player;
        }
        player.transform.position = new Vector2(transform.position.x, transform.position.y - distance);
    }
}