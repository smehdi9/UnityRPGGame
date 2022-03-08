using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PortalScript : MonoBehaviour
{
    public GameObject otherPortal;      //Portal to spawn to
    public float spawnOffset = 2.0f;

    private Vector2 _spawnLocation;

    void Start()
    {
        //Ensure that the other object is also a portal. Otherwise, self destruct
        if (otherPortal == null || !otherPortal.tag.Equals("Portal")) Destroy(this.gameObject);

        //Get the spawn location
        _spawnLocation = new Vector2(otherPortal.transform.position.x, otherPortal.transform.position.y + spawnOffset);
    }

    //On trigger, teleport the triggering collider
    void OnTriggerEnter2D(Collider2D other)
    {
        //Only teleport player
        if (other.tag.Equals("Player")) other.transform.position = _spawnLocation;
    }
}
