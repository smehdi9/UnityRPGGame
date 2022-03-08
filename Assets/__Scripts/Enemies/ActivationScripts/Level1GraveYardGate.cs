using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1GraveYardGate : BossActivationScript
{
    //Enable night sky
    public GameObject nightSky;
    public float nightSkyLoadIncrement;

    private float _waitTime = 0.001f;

    public GameObject levelBoss;
    private bool _bossKilled = false;

    public GameObject musicObject;

    private float _moveDistance;

    private NPCScript _npcScript;       //Get dialog stuff

    void Start()
    {
        _npcScript = this.GetComponent<NPCScript>();
        _moveDistance = this.GetComponent<SpriteRenderer>().bounds.size.x * nightSkyLoadIncrement;
    }

    //Late update to avoid race conditions
    void LateUpdate()
    {
        if(!isEnabled)
        {
            //If the player is inside the grave yard
            if(player.transform.position.y > this.transform.position.y && 
                player.transform.position.x >= this.transform.position.x && 
                player.transform.position.x <= this.transform.position.x + this.GetComponent<SpriteRenderer>().bounds.size.x * 2)
            {
                isEnabled = true;
                StartCoroutine(CloseDoor());
            }
        }

        //If level boss dies
        else
        {
            if(levelBoss == null && !_bossKilled)
            {
                musicObject.GetComponent<Level1MusicControllerScript>().StopAllMusic();
                StartCoroutine(NightSkyActivation());
                _bossKilled = true;
            }
        }
    }

    //perform action required 
    public override void PerformAction()
    {
        _npcScript.triggerDialog = false;
        musicObject.GetComponent<Level1MusicControllerScript>().StopAllMusic();
        StartCoroutine(NightSkyActivation());
    }


    //Slowly fade into night time or out
    IEnumerator NightSkyActivation()
    {
        Color color = nightSky.GetComponent<SpriteRenderer>().color;
        int inverter = color.a == 0 ? 1 : -1;
        while (inverter * nightSky.GetComponent<SpriteRenderer>().color.a < (1.0f + inverter)/2.0f)
        {
            nightSky.GetComponent<SpriteRenderer>().color = new Color(color.r, color.b, color.g, nightSky.GetComponent<SpriteRenderer>().color.a + inverter * nightSkyLoadIncrement * Time.deltaTime);
            this.transform.position = new Vector2(this.transform.position.x - _moveDistance * Time.deltaTime, this.transform.position.y);
            yield return new WaitForSeconds(_waitTime);
        }

        //If the boss is not dead
        if (levelBoss != null)
        {
            musicObject.GetComponent<Level1MusicControllerScript>().PlayNightMusic();
            isEnabled = false;
        }
        else musicObject.GetComponent<Level1MusicControllerScript>().PlayVillageMusic();
    }


    //Close door (faster)
    IEnumerator CloseDoor()
    {
        float currX = this.transform.position.x;
        float totalDist = this.GetComponent<SpriteRenderer>().bounds.size.x;
        while(this.transform.position.x < currX + totalDist)
        {
            this.transform.position = new Vector2(this.transform.position.x + _moveDistance * 3 * Time.deltaTime, this.transform.position.y);
            yield return new WaitForSeconds(_waitTime);
        }

        isEnabled = true;
    }
}
