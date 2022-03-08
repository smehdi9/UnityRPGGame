using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1BossScript : EnemyScript
{
    //Demon's damage value
    public int bossDamageValue = 30;
    public int bossRegenHealthAmount = 5;       //How many HP should the boss regenerate
    public float bossRegenHealthRate = 2.0f;    //How often should the boss regenerate

    public GameObject keyPrefab;
    private Vector2 _keySpawnLocation;
    private bool _spawnedKey = false;

    private NPCScript _npcScript;
    private DialogControllerScript _diagController;
    private bool _bossEnabled = false;

    public GameObject musicObject;

    //Start performs last, hence is the most applicable
    void Start()
    {
        maxDist = 0;
        _npcScript = this.GetComponent<NPCScript>();
        _diagController = GameObject.Find("DialogController").GetComponent<DialogControllerScript>();

        _keySpawnLocation = transform.position;
        StartCoroutine(RegenBossHealth());
        _npcScript = this.GetComponent<NPCScript>();
    }

    //Late update to avoid race conditions
    void LateUpdate()
    {
        if(!_npcScript.triggerDialog && !_diagController.IsOpen && !_bossEnabled)
        {
            maxDist = 100.0f;   //Make max distance really high
            musicObject.GetComponent<Level1MusicControllerScript>().PlayBossMusic();
            _bossEnabled = true;
        }

        //If the demon boss dies, drop the key
        if(demonAnim.GetBool("Dead") && !_spawnedKey)
        {
            GameObject key = Instantiate(keyPrefab) as GameObject;
            key.transform.position = _keySpawnLocation;
            _spawnedKey = true;
        }
    }

    //Regenerate the boss's health on the given duration (every second)
    IEnumerator RegenBossHealth()
    {
        while(true)
        {
            yield return new WaitForSeconds(bossRegenHealthRate);
            this.Health += bossRegenHealthAmount;
        }
    }


    //What to do on attack
    public override void OnAttack()
    {
        //Damage the player
        PlayerUserInferface.Health = PlayerUserInferface.Health - bossDamageValue;
    }
}
