using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovementScript : MonoBehaviour
{
    //Constants
    public float playerSpeed = 8f;         //Constant value to be modified using Unity editor (hence not set as a const)
    public float leapSpeed   = 12f;
    public float leapDuration = 1f;
    public float leapCoolDown = 5f;

    static public PlayerMovementScript S;   //Singleton

    //Public unity game objects
    public Rigidbody2D playerRb;
    public Animator playerAnim;

    //Movement direction vector
    private Vector2 _movement;
    private Vector2 _leapMovement;
    private bool    _canLeap = true;
    private PlayerUI _playerUI;     //To get the player's health

    //List of inventory items that the player holds [Pseudo-inventory]
    private List<GameObject> _inventory;
    private GameObject _currentItem;            //The Game object storing the currently selected game object
    private int _selectedInventoryItem;         //A value of -1 will be allowed. -1 will imply that no item from the inventory is selected

    //Melee collider 
    public GameObject meleePrefab;
    private GameObject _dynamicMelee;           //This value is private because we will generate and destroy the melee collider in run-time

    //Throwable collider
    public GameObject throwPrefab;
    public float throwCoolDown = 2f;
    private bool _canThrow = true;

    //AOE collider
    public GameObject aoePrefab;
    private GameObject _dynamicAOE;
    public float aoeCoolDown = 5f;
    public float aoeDuration = 0.5f;
    private bool _canAOE = true;

    //Private because we want to find it dynamically
    private DialogControllerScript dialogScript;       //Dialog box to ensure user inputs are not allowed when dialog is being shown

    //Singleton to ensure there is only one player
    void Awake()
    {
        if (S == null)
        {
            S = this;
            print("Set Singlton to:" + this.tag);
        }
        else
        {
            print("Trying to create another Singlton instacne");
        }

        Vector3 hPos = PlayerMovementScript.S.transform.position;
        print("This is: " + this.tag + " Hero is set to: " + PlayerMovementScript.S.tag);

        dialogScript = GameObject.Find("DialogController").GetComponent< DialogControllerScript>();
    }

    void Start()
    {
        //Set inventory
        _inventory = new List<GameObject>();
        _selectedInventoryItem = -1;

        //Initialize the player to face south
        playerAnim.SetFloat("IdleVertical", -1);
        _playerUI = this.GetComponent<PlayerUI>();
    }

    void Update()
    {
        //Only allow any updates on the player object if the player is not dead
        if (_playerUI.IsDead)
        {
            //If the player is dead, remove current item
            Destroy(_currentItem);
            return;
        }

        //Directional controls (WASD and arrow keys) to move the character (Only allow when dialog isn't shown)
        int u = (Input.GetKey("up") || Input.GetKey(KeyCode.W)) && !dialogScript.IsOpen ? 1 : 0;
        int d = (Input.GetKey("down") || Input.GetKey(KeyCode.S)) && !dialogScript.IsOpen ? 1 : 0;
        int l = (Input.GetKey("left") || Input.GetKey(KeyCode.A)) && !dialogScript.IsOpen ? 1 : 0;
        int r = (Input.GetKey("right") || Input.GetKey(KeyCode.D)) && !dialogScript.IsOpen ? 1 : 0;

        //Set the movement vector which will be used to modify the player position
        _movement.y = u - d;
        _movement.x = r - l;
        _movement.Normalize();          //Normalize the movement vector to be a unit vector

        //Leap mechanics --- RIGHT CLICK
        //If the player is not in cooldown, and not mid leap, allow leap
        if(!dialogScript.IsOpen && Input.GetMouseButtonDown(1) && !playerAnim.GetBool("Leap") && _canLeap){
            _leapMovement = _movement;
            StartCoroutine(LeapDuration(leapDuration));
            StartCoroutine(LeapCoolDown(leapCoolDown));
        }


        //Manage player attacks --- LEFT CLICK
        bool initFrame = false;
        if (playerAnim.GetBool("Attack")) playerAnim.SetBool("Attack", false);
        if (!dialogScript.IsOpen && !playerAnim.GetCurrentAnimatorStateInfo(0).IsName("AttackState") && !playerAnim.GetCurrentAnimatorStateInfo(0).IsName("AOEState") && Input.GetMouseButtonDown(0) && _currentItem != null)     //If an item is selected and the mouse is pressed
        {
            //Melee weapons
            if (_currentItem.tag == "Melee" && _dynamicMelee == null)
            {
                //Start attack animation
                playerAnim.SetBool("Attack", true);

                //Create the melee collider based on the direction of the player
                _dynamicMelee = Instantiate(meleePrefab) as GameObject;
                _dynamicMelee.GetComponent<AttackScript>().DamageValue = System.Int32.Parse(_currentItem.name.Split('(')[0]);      //Set the damage value (If any instance of this object has the (Clone) string in it's name, exclude that)
                initFrame = true;

                //Locate based on direction facing
                if (System.Math.Abs(playerAnim.GetFloat("IdleVertical")) > System.Math.Abs(playerAnim.GetFloat("IdleHorizontal")))
                {
                    Vector2 posDM;
                    posDM.x = transform.position.x;
                    if (playerAnim.GetFloat("IdleVertical") > 0)
                    {
                        posDM.y = transform.position.y + 1;
                    }
                    else
                    {
                        posDM.y = transform.position.y - 2;
                        _dynamicMelee.transform.Rotate(0, 0, 180);
                    }
                    _dynamicMelee.transform.position = posDM;
                }
                else
                {
                    Vector2 posDM;
                    posDM.y = transform.position.y - 0.5f;
                    if (playerAnim.GetFloat("IdleHorizontal") > 0)
                    {
                        posDM.x = transform.position.x + 2;
                        _dynamicMelee.transform.Rotate(0, 0, 270);
                    }
                    else
                    {
                        posDM.x = transform.position.x - 2;
                    _dynamicMelee.transform.Rotate(0, 0, 90);
                    }
                    _dynamicMelee.transform.position = posDM;
                }

                //Remove the current item sprite
                Destroy(_currentItem);
                _currentItem = null;

            }
            //Throwable weapons (Only if not on cooldown)
            else if (_currentItem.tag == "Throw" && _canThrow)
            {
                //Start attack animation
                playerAnim.SetBool("Attack", true);

                StartCoroutine(ThrowCoolDown(throwCoolDown));           //Start cooldown for throw

                //Spawn the object
                GameObject throwObj = Instantiate(throwPrefab) as GameObject;
                throwObj.transform.position = transform.position;
                throwObj.GetComponent<SpriteRenderer>().sprite = _currentItem.GetComponent<SpriteRenderer>().sprite;

                throwObj.GetComponent<AttackScript>().DamageValue = System.Int32.Parse(_currentItem.name.Split('(')[0]);      //Set the damage value (If any instance of this object has the (Clone) string in it's name, exclude that)
                Vector2 posTO = new Vector2(0, 0);

                if (System.Math.Abs(playerAnim.GetFloat("IdleVertical")) > System.Math.Abs(playerAnim.GetFloat("IdleHorizontal")))
                {
                    if (playerAnim.GetFloat("IdleVertical") > 0) posTO.y = 1;
                    else posTO.y = -1;                    
                }
                else
                {
                    if (playerAnim.GetFloat("IdleHorizontal") > 0) posTO.x = 1;
                    else posTO.x = -1;
                }

                //Set the speed of the object
                throwObj.GetComponent<ThrowAttackScript>().SetSpeed(posTO.x, posTO.y, true);
            }
            //Area of effect items (Only if not on cooldown)
            else if (_currentItem.tag == "AreaOfEffect" && _canAOE)
            {
                //Spawn the object
                _dynamicAOE = Instantiate(aoePrefab) as GameObject;
                _dynamicAOE.transform.position = transform.position;
                _dynamicAOE.GetComponent<AttackScript>().DamageValue = System.Int32.Parse(_currentItem.name.Split('(')[0]);      //Set the damage value (If any instance of this object has the (Clone) string in it's name, exclude that)

                initFrame = true;
                Destroy(_currentItem);
                _currentItem = null;

                StartCoroutine(AOECoolDown(aoeCoolDown));           //Start cooldown for throw
                StartCoroutine(AOEDuration(aoeDuration));
            }
        }
        //Ensure the player isn't moving while in attack mode
        if (playerAnim.GetCurrentAnimatorStateInfo(0).IsName("AttackState") || playerAnim.GetCurrentAnimatorStateInfo(0).IsName("AOEState")) 
        {
            _movement.x = 0; _movement.y = 0;
        }
        //Destroy the dynamic melee object when completed attack
        else if (!initFrame)
        {
            //Once animation is done, remove the collider and return the item sprite
            if (_dynamicMelee != null)
            {
                Destroy(_dynamicMelee);
                _dynamicMelee = null;
            }
            if(_dynamicAOE != null)
            {
                Destroy(_dynamicAOE);
                _dynamicAOE = null;
            }

            //Reset current item
            if(_currentItem == null && SelectedItem >= 0)
            {
                _currentItem = Instantiate(_inventory[SelectedItem]) as GameObject;
                _currentItem.transform.position = transform.position;
                _currentItem.transform.parent = transform;
            }
        }


        //Manage player inventory ---
        //Scroll up
        if(Input.GetAxis("Mouse ScrollWheel") > 0)
        {
            SelectedItem = SelectedItem + 1;
            if (_currentItem != null) Destroy(_currentItem);  //Remove current item
            if (SelectedItem == -1) _currentItem = null;
            else
            {
                //Instantiate new item
                _currentItem = Instantiate(_inventory[SelectedItem]) as GameObject;
                _currentItem.transform.position = transform.position;
                _currentItem.transform.parent = transform;
            }
        }
        //Scroll down
        else if (Input.GetAxis("Mouse ScrollWheel") < 0)
        {
            SelectedItem = SelectedItem - 1;
            if (_currentItem != null) Destroy(_currentItem);  //Remove current item
            if (SelectedItem == -1) _currentItem = null;
            else
            {
                //Instantiate new item
                _currentItem = Instantiate(_inventory[SelectedItem]) as GameObject;
                _currentItem.transform.position = transform.position;
                _currentItem.transform.parent = transform;
            }
        }

        //Set the orientation of the selected item object
        if (_currentItem != null)
        {
            //Sorting Order
            SpriteRenderer itemSpr = _currentItem.GetComponent<SpriteRenderer>();
            if (playerAnim.GetFloat("IdleVertical") > System.Math.Abs(playerAnim.GetFloat("IdleHorizontal"))) itemSpr.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder + 1;
            else itemSpr.sortingOrder = GetComponent<SpriteRenderer>().sortingOrder - 1;

            float xOfs = 0;
            //Rotation
            if (System.Math.Abs(playerAnim.GetFloat("IdleVertical")) > System.Math.Abs(playerAnim.GetFloat("IdleHorizontal")))
            {
                Vector3 rotCI = new Vector3(0, 0, 180);
                if (playerAnim.GetFloat("IdleVertical") < 0) rotCI.y = 0;
                else rotCI.y = 180;
                _currentItem.transform.eulerAngles = rotCI;
            }
            else
            {
                Vector3 rotCI = new Vector3(0, 0, 180);
                if (playerAnim.GetFloat("IdleHorizontal") < 0) xOfs = -itemSpr.bounds.size.x / 2.5f;
                else xOfs = itemSpr.bounds.size.x / 2.5f;
                rotCI.y = 45;
                _currentItem.transform.eulerAngles = rotCI;
            }

            //Transform
            Vector2 posCI;
            posCI.x = transform.position.x - xOfs; posCI.y = transform.position.y;
            _currentItem.transform.position = posCI;
        }


        //Manage player animation ---
        playerAnim.SetFloat("Horizontal", _movement.x);
        playerAnim.SetFloat("Vertical", _movement.y);
        playerAnim.SetFloat("Speed", _movement.sqrMagnitude);

        //If the player starts moving, set their facing direction
        if (_movement.sqrMagnitude > 0)
        {
            playerAnim.SetFloat("IdleHorizontal", _movement.x);
            playerAnim.SetFloat("IdleVertical", _movement.y);
        }
    }

    //Fixed update for physics
    void FixedUpdate()
    {
        //If the player is dead, they must not move
        if(_playerUI.IsDead)
        {
            _movement.x = 0;
            _movement.y = 0;
        }

        //Move the rigid body (Not in leap)
        if (!playerAnim.GetBool("Leap")) playerRb.MovePosition(playerRb.position + _movement * playerSpeed * Time.fixedDeltaTime);
        
        //If in leap
        else
        {
            //If not moving character, leap in the direction that the character is facing
            if(_leapMovement.magnitude == 0)
            {
                if (System.Math.Abs(playerAnim.GetFloat("IdleVertical")) > System.Math.Abs(playerAnim.GetFloat("IdleHorizontal")))
                {
                    if (playerAnim.GetFloat("IdleVertical") > 0) _leapMovement.y = 1;
                    else _leapMovement.y = -1;
                }
                else
                {
                    if (playerAnim.GetFloat("IdleHorizontal") > 0) _leapMovement.x = 1;
                    else _leapMovement.x = -1;
                }
            }

            //Leap in the direction of movement
            playerRb.MovePosition(playerRb.position + _leapMovement * leapSpeed * Time.fixedDeltaTime);
        }
    }

    //Coroutine for when the player is mid leap
    IEnumerator LeapDuration(float time){
        playerAnim.SetBool("Leap",true);
        yield return new WaitForSeconds(time);
        playerAnim.SetBool("Leap",false);
    }

    //Coroutine to ensure that player waits the cooldown before next leap
    IEnumerator LeapCoolDown(float time){
        _canLeap = false;
        yield return new WaitForSeconds(time);
        _canLeap = true;
    }

    //Coroutine for when the player is mid leap
    IEnumerator AOEDuration(float time)
    {
        playerAnim.SetBool("AOE", true);
        yield return new WaitForSeconds(time);
        //Restore the current item
        _currentItem = Instantiate(_inventory[SelectedItem]) as GameObject;
        _currentItem.transform.position = transform.position;
        _currentItem.transform.parent = transform;

        playerAnim.SetBool("AOE", false);
    }

    //Coroutine to ensure that player waits the cooldown before next leap
    IEnumerator AOECoolDown(float time)
    {
        _canAOE = false;
        yield return new WaitForSeconds(time);
        _canAOE = true;
    }

    //Coroutine to ensure that player waits the cooldown before next throw
    IEnumerator ThrowCoolDown(float time)
    {
        _canThrow = false;
        yield return new WaitForSeconds(time);
        _canThrow = true;
    }

    //If a new item is picked up, add to inventory. Accessed by other scripts
    public void CreateObjectFromPickup(GameObject item, string tag, int damage)
    {
        //Create a clone of the original item
        GameObject newItem = new GameObject();

        DontDestroyOnLoad(newItem);      //Ensure that the new item cannot be destroyed

        //Copy only the transform and sprite renderer components of the original item
        UnityEditorInternal.ComponentUtility.CopyComponent(item.GetComponent<Transform>());
        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(newItem); 
        UnityEditorInternal.ComponentUtility.CopyComponent(item.GetComponent<SpriteRenderer>());
        UnityEditorInternal.ComponentUtility.PasteComponentAsNew(newItem);

        newItem.transform.localScale = item.transform.localScale;
        newItem.transform.position = new Vector3(0, 0, -20);

        //Set new item tag and add to inventory
        newItem.tag = tag;
        newItem.name = damage.ToString();       //Set the damage value as the name
        _inventory.Add(newItem);
    }

    //Properties
    //Selected inventory item. (Only set to an index that is between -1 and max inventory entries)
    public int SelectedItem
    {
        get { return _selectedInventoryItem; }
        set
        {
            if(value >= -1 && value < _inventory.Count)
            {
                _selectedInventoryItem = value;
            }
        }
    }
}
