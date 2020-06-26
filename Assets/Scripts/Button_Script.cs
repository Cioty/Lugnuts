using UnityEngine.Animations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.EventSystems;

public class Button_Script : MonoBehaviour
{
    public Game_Manager gameManager;
    private Part_Manager partManager;
    private Part_Creator partCreator;
    private Crate_Manager crateManager;
    //public Animator chuteDoorAnimator;
    //public Animator[] crateAnimators;
    //public Animator crateConveyerAnimator;
    private Animator thisAnimator;
    [HideInInspector]
    public bool buttonDone = false;

    //part parent gets reset to this position
    private GameObject partParentLocation;
    //part Parent
    private GameObject partParent;

    // Start is called before the first frame update
    void Awake()
    {
        thisAnimator = gameObject.GetComponent<Animator>();
        partManager = gameManager.partManager;
        partCreator = gameManager.partCreator;
        crateManager = gameManager.crateManager;
        partParentLocation = gameManager.partParentLocation;
        partParent = gameManager.partCreator.partParent;
    }

    public void ButtonPress()
    {
        if (buttonDone) //how interact with apk
        {
            Audio_Manager.Play("Button");
            thisAnimator.SetBool("Pressed", true);
            buttonDone = false;

            //maybe add a delay?
            partManager.NonCoreRemovePart();
            partParent.transform.position = partParentLocation.transform.position;

            StartCoroutine(crateManager.CrateCycle());
            //create parts is in crate manager as it manages the transition
        } //else play unsuccessful press sound
    }

    public void ButtonPressEnd()
    {
        thisAnimator.SetBool("Pressed", false);
    }
}
