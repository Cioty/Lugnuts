using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    private Animator thisAnimator;
    public Game_Manager gameManager;
    private Part_Manager partManager;
    //private Part_Creator partCreator;
    private GameObject coreParent;
    private Animator coreAnimator;

    public bool leverDone = false;

    private void Awake()
    {
        thisAnimator = gameObject.GetComponent<Animator>();
        partManager = gameManager.partManager;
        //partCreator = gameManager.partCreator;
        coreParent = gameManager.partCreator.coreParent;
        coreAnimator = coreParent.GetComponent<Animator>();
    }

    //public void LeverStop()
    //{
    //    
    //}

    public void LeverPull()
    {
        if (leverDone)
        {
            thisAnimator.SetBool("Pull", true);
            leverDone = false;
        }
    }

    //called at the end of the pull animation
    public void Pulled()
    {
        thisAnimator.SetBool("Pull", false);
        coreAnimator.SetBool("Enter", false);
    }
}
