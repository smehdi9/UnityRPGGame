using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogControllerScript : MonoBehaviour
{
    //Public variables
    public Text nameUI;
    public Text dialogUI;

    public Animator uiAnim;         //Animation

    private Queue<string> _listOfDialog;

    // Awake is called before the first frame update
    void Awake()
    {
        _listOfDialog = new Queue<string>();
    }

    //Play the passed sequence of dialog
    public void StartDialog(Dialog dialog)
    {
        _listOfDialog.Clear();  //Empty the dialog set
        nameUI.text = dialog.speakerName;

        uiAnim.SetBool("Open", true);

        //Fill the list of dialogue
        foreach (string sentence in dialog.listOfSentences)
        {
            _listOfDialog.Enqueue(sentence);
        }

        //Display the first sentence
        DisplayNextDialog();
    }

    //Display the next dialogue
    public void DisplayNextDialog()
    {
        //Check if there are more sentences to display
        if(_listOfDialog.Count == 0)
        {
            EndDialog();
        }
        else
        {
            string currentDialog = _listOfDialog.Dequeue();
            dialogUI.text = currentDialog;
        }
    }

    
    //What to do on end dialogue
    void EndDialog()
    {
        uiAnim.SetBool("Open", false);
    }

    //Properties ----
    //Get if the dialog box is currently open
    public bool IsOpen
    {
        get { return uiAnim.GetBool("Open"); }
    }
}
