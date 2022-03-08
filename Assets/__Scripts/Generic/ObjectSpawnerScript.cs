using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectSpawnerScript : MonoBehaviour
{
    //Public variables
    public float spawnRateInTime = 5f;       //How often will our function be called
    public int maxAmount = 2;                //How many maximum enemies will we spawn. Counter for each individual spawner

    public GameObject spawnObject;          //Object to spawn
    public GameObject parentObject;         //Parent of the object being spawned. Will be set as parent once object is spawned.

    private List<GameObject> _listEnemies;      //List of enemies
    SpriteRenderer _sprite;

    void Start()
    {
        _listEnemies = new List<GameObject>();
        _sprite = GetComponent<SpriteRenderer>();
        InvokeRepeating("TrySpawnObject", 0, spawnRateInTime);
    }

    //Function will try to spawn an enemy each few durations.
    void TrySpawnObject()
    {
        for(int i = 0; i < _listEnemies.Count; i++)
        {
            if (_listEnemies[i] == null) _listEnemies.RemoveAt(i);
        }
        if(_listEnemies.Count < maxAmount)
        {
            GameObject obj = Instantiate(spawnObject) as GameObject;
            obj.transform.parent = parentObject.transform;  //Set parent
            //Set the position (Random location within the bounds of the spawn region)
            Vector2 pos;
            pos.x = Random.Range(transform.position.x - _sprite.bounds.size.x / 2, transform.position.x + _sprite.bounds.size.x / 2);
            pos.y = Random.Range(transform.position.y - _sprite.bounds.size.y / 2, transform.position.y + _sprite.bounds.size.y / 2);
            obj.transform.position = pos;
            _listEnemies.Add(obj);
        }
    }
}
