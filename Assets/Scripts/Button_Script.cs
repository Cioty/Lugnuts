using UnityEngine.Animations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.EventSystems;

public class Button_Script : MonoBehaviour
{
    public Part_Creator partCreator;
    public Animator chuteDoorAnimator;
    public Animator[] crateAnimators;
    public Animator crateConveyerAnimator;
    public Animator thisAnimator;
    public bool buttonDone = true;

    // Start is called before the first frame update
    void Start()
    {
        thisAnimator = gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        //if(Input.GetKeyDown(KeyCode.H))
       // {
       //     ButtonPress();
       // } 
    }

    public void ButtonPress()
    {
        if (buttonDone) //how interact with apk
        {
            Audio_Manager.Play("Button");
            thisAnimator.SetTrigger("Press");
            buttonDone = false;
           partCreator.CreateParts();
        } //else play unsuccessful press sound
    }

    public void StartConveyer()
    {
        
    }

    public void StopConveyer()
    {

    }

    public void ResetAnimatorBool(Animator animator, int fuckyou)
    {
        
    }
}
