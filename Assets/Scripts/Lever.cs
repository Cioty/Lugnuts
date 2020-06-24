using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lever : MonoBehaviour
{
    private Animator thisAnimator;

    private void Awake()
    {
        thisAnimator = gameObject.GetComponent<Animator>();
    }

    public void LeverStop()
    {
        thisAnimator.SetBool("pull", false);
    }
}
