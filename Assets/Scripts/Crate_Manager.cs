using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate_Manager : MonoBehaviour
{
    private Game_Manager gameManager;
    private Part_Creator partCreator;

    //crate vars
    public GameObject crate1;
    public GameObject crate2;
    private Animator crate1Animator;
    private Animator crate2Animator;
    private Crate crate1Script;
    private Crate crate2Script;
    private bool crate1Bool;
    private bool crate2Bool;

    public bool createPartsFinished;

    private void Awake()
    {
        gameManager = gameObject.GetComponent<Game_Manager>();
        partCreator = gameObject.GetComponent<Part_Creator>();

        //create stuff
        crate1Animator = crate1.GetComponent<Animator>();
        crate2Animator = crate2.GetComponent<Animator>();
        crate1Script = crate1.GetComponent<Crate>();
        crate2Script = crate2.GetComponent<Crate>();
        CrateSet();
    }

    private void CrateSet()
    {
        crate1Animator.SetBool("Entry", false);
        crate1Bool = false;
        crate2Animator.SetBool("Entry", false);
        crate2Bool = false;

        crate1Script.CrateCollider(true);
        crate2Script.CrateCollider(false);
        StartCoroutine(FirstCrate());
    }

    //called during the animation event that turns the other crate's colliders off.
    public void EnableOtherCrateColliders(GameObject crate)
    {
        if(crate == crate1)
        {
            crate2Script.CrateCollider(true);
        } else
        {
            crate1Script.CrateCollider(true);
        }

        //since this enables the colliders on the receiving crate we may as well spawn parts here
        
        partCreator.CreateParts();

    }

    public IEnumerator FirstCrate()
    {
        createPartsFinished = false;
        while (!createPartsFinished)
        {
            yield return null;
        }
        crate1Animator.SetBool("Entry", true);
        crate1Bool = true;
    }

    //cycles the crates. The other crate is delayed from moving out until the last part's speed is low.
    public IEnumerator CrateCycle()
    {
        createPartsFinished = false;
        if (crate1Bool)
        {
            crate1Animator.SetBool("Entry", !crate1Bool);
            crate1Bool = !crate1Bool;
            while (!createPartsFinished)
            {
                yield return null;
            }
            crate2Animator.SetBool("Entry", !crate2Bool);
            crate2Bool = !crate2Bool;
        } else
        {
            crate2Animator.SetBool("Entry", !crate2Bool);
            crate2Bool = !crate2Bool;
            
            while (!createPartsFinished)
            {
                yield return null;
            }
            crate1Animator.SetBool("Entry", !crate1Bool);
            crate1Bool = !crate1Bool;
        }
        
    }
}
