using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Doll_Parent : MonoBehaviour
{
    public GameObject gameManagerObject;
    private Part_Manager partManager;

    // Start is called before the first frame update
    void Awake()
    {
        partManager = gameManagerObject.GetComponent<Part_Manager>();
    }

    public void StartDollDemo()
    {
        StartCoroutine(partManager.DollDemonstration());
    }

    public void DestroyDoll()
    {
        partManager.DollRemove();
    }
}
