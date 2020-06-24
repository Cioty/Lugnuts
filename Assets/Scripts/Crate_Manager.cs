using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate_Manager : MonoBehaviour
{
    public GameObject crate1;
    public GameObject crate2;
    private Animator crate1Animator;
    private Animator crate2Animator;
    private bool crate1Bool;
    private bool crate2Bool;

    private void Awake()
    {
        crate1Animator = crate1.GetComponent<Animator>();
        crate2Animator = crate2.GetComponent<Animator>();
        CrateSet();
    }

    private void CrateSet()
    {
        crate1Animator.SetBool("Entry", true);
        crate1Bool = true;
        crate2Animator.SetBool("Entry", false);
    }

    public void CrateCycle()
    {
        crate1Animator.SetBool("Entry", !crate1Bool);
        crate1Bool = !crate1Bool;
        crate2Animator.SetBool("Entry", !crate2Bool);
        crate2Bool = !crate2Bool;
    }
}
