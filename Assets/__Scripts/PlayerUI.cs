using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerUI : MonoBehaviour
{
    //Public variables
    public int startingHealth = 100;
    public int maximumHealthCap = 300;      //maximum health can reach this maximum value
    public int expPerHealthGain = 20;       //EXP needed to gain ONE extra HP

    public int startingExp = 0;
    public Animator playerAnim;     //Player's animator
    public GameObject gameOver;     //Game over UI

    //Text UI game objects from Unity
    public Text health;
    public Text experience;
    public Text currentScene;

    //The internal values of player's health and experience
    private int _fullHealthVal;
    private int _healthCounter;

    private int _experienceCounter;
    private int _prevExperience;        //If experience changes, we use this variable

    private bool _isDead = false;
    private bool _hasDoorKey = false;

    //Initialize UI variables
    void Start()
    {
        //Find UI obects
        if (gameOver == null) gameOver = GameObject.Find("GameOver");
        if (health == null) gameOver = GameObject.Find("Health");
        if (experience == null) gameOver = GameObject.Find("Experience");
        if (currentScene == null) gameOver = GameObject.Find("LevelName");

        //Set values
        IsDead = false;
        HasKey = false;

        FullHealthValue = startingHealth;

        //Initialize the health and experience, as well as the Text game objects
        Health = FullHealthValue;
        Experience = startingExp;
        SetExperienceText();
        SetHealthText();
        SetCurrentScene();

        DontDestroyOnLoad(gameObject);      //Ensure that the player cannot be destroyed

        gameOver.SetActive(false);  //Game over text
    }

    // Update is called once per frame
    void Update()
    {
        //If the experience increases, so should health
        if(_prevExperience != Experience)
        {
            FullHealthValue = startingHealth + Experience / expPerHealthGain;       //Increase by rate given above
            _prevExperience = Experience;
        }

        //If the player dies, perform this action once.
        if (Health <= 0 && !IsDead)
        {
            IsDead = true;
            //Fix collider on death
            this.GetComponent<BoxCollider2D>().offset = new Vector2(0, 0);
            playerAnim.SetBool("Dead", true);               //Set the animation
            Health = 0;                                     //Do not let the health go below 0
            gameOver.SetActive(true);                       //Display game over message
            StartCoroutine(ReloadLevelOne(5));              //Reload scene after 5 seconds
        }

        //Consistently update the canvas values
        SetExperienceText();
        SetHealthText();
        SetCurrentScene();
    }

    //If the game is over, wait for a given perior before reloading the scene
    IEnumerator ReloadLevelOne(float time)
    {
        yield return new WaitForSeconds(time);

        //Destroy all objects that are set to dont destroy on load currently in active
        GameObject[] allDDOLObject = gameObject.scene.GetRootGameObjects();

        GameObject selfObject = null;
        GameObject cameraObject = null;

        foreach (GameObject go in allDDOLObject)
        {
            if(go.tag.Equals("Player"))
            {
                selfObject = go;
                continue;
            }
            else if(go.tag.Equals("MainCamera"))
            {
                cameraObject = go;
                continue;
            }

            if (go.activeInHierarchy)
            {
                Destroy(go);
            }
        }
            
        //Load new scene
        SceneManager.LoadScene("Level1", LoadSceneMode.Single);
        Destroy(selfObject);
    }

    //Set the Text object's content based on the current health and exp values --------
    void SetHealthText()
    {
        health.text = "Health: " + Health.ToString() + "/" + FullHealthValue.ToString();
    }

    void SetExperienceText()
    {
        experience.text = "EXP: " + Experience.ToString();
    }

    void SetCurrentScene()
    {
        currentScene.text = SceneManager.GetActiveScene().name;
    }

    //Properties ------------------------------
    //Health property. Only set the value of the health counter if the value passed is between maximum health value and 0. ONLY CHANGE IF THE CHARACTER IS NOT DEAD.
    public int Health
    {
        get
        {
            return _healthCounter;
        }
        set
        {
            if(!IsDead)
            {
                if (value < 0) _healthCounter = 0;
                else if (value > FullHealthValue) _healthCounter = FullHealthValue;
                else _healthCounter = value;
            }
        }
    }

    //Experience property. Only set the value of the experience counter if the value passed is above or equal to 0. ONLY CHANGE IF THE CHARACTER IS NOT DEAD.
    public int Experience
    {
        get
        {
            return _experienceCounter;
        }
        set
        {
            if (!IsDead)
            {
                if (value >= 0) _experienceCounter = value;
                else _experienceCounter = 0;
            }
        }
    }

    //Is dead property. If the player is already dead, they must not be brought back to life. (Cannot set back to false)
    public bool IsDead
    {
        get
        {
            return _isDead;
        }
        set
        {
            _isDead = value;
        }
    }

    //Get whether or not the player has the key
    public bool HasKey
    {
        get { return _hasDoorKey; }
        set
        {
            if(!_hasDoorKey) _hasDoorKey = value;     //Only allow change if the value is currently false
        }
    }

    //Has full HP
    public bool HasFullHP
    {
        get { return Health == FullHealthValue; }
    }

    //Get the full HP value
    public int FullHealthValue
    {
        get { return _fullHealthVal; }
        set
        {
            if(value >= startingExp && value <= maximumHealthCap)       //Bounds for full hp starting HP < fullHP < hp cap
            {
                int increase = value - _fullHealthVal;          //This can be negative
                _fullHealthVal = value;
                Health += increase;                             //Increase the health at the same time as full HP increase, but don't max out the HP
            }
        }
    }
}
