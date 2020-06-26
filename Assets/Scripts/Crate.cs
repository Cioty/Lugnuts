using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crate : MonoBehaviour
{
    public Game_Manager gameManager;
    private Crate_Manager crateManager;
    private GameObject partParent;

    private void Awake()
    {
        crateManager = gameManager.crateManager;
        partParent = gameManager.partCreator.partParent;
    }

    //different method since this is called by an animation event
    public void DisableColliders()
    {
        CrateCollider(false);
        crateManager.EnableOtherCrateColliders(gameObject); 
    }

    //disables/enables colliders for the crate.
    public void CrateCollider(bool enable)
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            transform.GetChild(i).gameObject.SetActive(enable);
        }
    }

    public void DisableChute()
    {
        transform.GetChild(0).gameObject.SetActive(false);
    }

    public void ParentTo()
    {
        partParent.transform.SetParent(transform);
    }

    public void ParentAway()
    {
        partParent.transform.parent = null;

        //this is called at the end so I reset buttonDone on buttonScript here too
        gameManager.buttonScript.buttonDone = true;
    }
}
