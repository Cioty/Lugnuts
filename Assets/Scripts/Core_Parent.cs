using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Core_Parent : MonoBehaviour
{
    public Game_Manager gameManager;
    private Part_Manager partManager;
    public Lever leverScript;

    public void Awake()
    {
        partManager = gameManager.partManager;
        leverScript = gameManager.leverScript;
    }

    public void CoreOut()
    {
        partManager.CompareBlueprints();
    }

    public void CoreIn()
    {
        leverScript.leverDone = true;
    }
}
