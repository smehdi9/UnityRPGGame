using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackScript : MonoBehaviour
{
    private int _damageValue = 0;       //protected variable. We only set this value in run-time. Convention for protected variables is Capital first letter camelcase
    private List<GameObject> _listDamagedObjects = new List<GameObject>();

    //Attack trigger
    void OnTriggerEnter2D(Collider2D other)
    {
        foreach (GameObject go in _listDamagedObjects) if (go == other.gameObject) return;  //Do not damage the same object twice

        //Only if the collider is of tag mob
        if (other.tag.Equals("Mob"))
        {
            _listDamagedObjects.Add(other.gameObject);
            MobScript mobController = other.GetComponent<MobScript>();
            mobController.Health = mobController.Health - DamageValue;
        }
        if (!other.tag.Equals("Player") && !other.tag.Equals("Item"))
        {
            Destroy(this.gameObject);
        }
    }

    //Property. Used to ensure damage value is not set to below 0
    public int DamageValue
    {
        get { return _damageValue; }
        set
        {
            if (value >= 0) _damageValue = value;
            else _damageValue = 0;
        }
    }
}
