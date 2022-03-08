using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCScript : MonoBehaviour
{
    public GameObject player;
    public bool dynamicallyFindHeroObject = true;       //If we want the script to find the Hero object, we set this to be true.

    public bool triggerDialog = true;

    public float triggerDistance = 2f;
    public Dialog dialog;

    // Awake is called before the first frame update
    void Awake()
    {
        if (dynamicallyFindHeroObject) player = GameObject.Find("Hero");
    }

    //Physics update
    void LateUpdate()
    {
        //If player gets close
        if (Vector2.Distance(player.transform.position, transform.position) <= triggerDistance)
        {
            if(dialog.listOfSentences.Length > 0 && triggerDialog)
            {
                TriggerDialog();
                triggerDialog = false;
            }
        }
    }

    //Play the dialogue sequence
    public void TriggerDialog()
    {
        //Get the Dialog controller object, and run the dialog sequence.
        GameObject.Find("DialogController").GetComponent<DialogControllerScript>().StartDialog(dialog);
    }
}
