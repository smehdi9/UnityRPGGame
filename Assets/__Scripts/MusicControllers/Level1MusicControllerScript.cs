using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Level1MusicControllerScript : MonoBehaviour
{
    public AudioSource villageMusic;
    public AudioSource nightTimeMusic;
    public AudioSource bossBattleMusic;

    void Start()
    {
        PlayVillageMusic();
    }

    //Play village music
    public void PlayVillageMusic()
    {
        nightTimeMusic.Stop();
        bossBattleMusic.Stop();
        villageMusic.Play();
    }

    //Play night music
    public void PlayNightMusic()
    {
        villageMusic.Stop();
        bossBattleMusic.Stop();
        nightTimeMusic.Play();
    }

    //Play boss music
    public void PlayBossMusic()
    {
        villageMusic.Stop();
        nightTimeMusic.Stop();
        bossBattleMusic.Play();
    }

    //Stop all music
    public void StopAllMusic()
    {
        villageMusic.Stop();
        nightTimeMusic.Stop();
        bossBattleMusic.Stop();
    }
}
