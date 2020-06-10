using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmChecker : MonoBehaviour
{

    private NickTestScript parentScript;
    public Transform connectionPosition;
    //public bool connectingSocket;

    //public int connectionNumber;

    // Start is called before the first frame update
    void Start()
    {
        parentScript = GetComponentInParent<NickTestScript>();
        //connectionPosition = parentScript.connectionArea;
        //connectionPosition = parentScript.connectionPoints[connectionNumber].transform;

        //if (connectingSocket == false)
        //{
        //    connectionPosition = null;
        //}
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Finish" && parentScript.holdingPiece == false /*&& connectingSocket == false*/)
        {
            parentScript.piece.isKinematic = true;
            parentScript.piece.useGravity = false;
            parentScript.piece.rotation = other.gameObject.GetComponent<ArmChecker>().connectionPosition.transform.rotation;
            parentScript.piece.position = other.gameObject.GetComponent<ArmChecker>().connectionPosition.transform.position;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Finish" /*&& parentScript.holdingPiece == true*/)
        {
            parentScript.piece.useGravity = true;
        }
    }
}
