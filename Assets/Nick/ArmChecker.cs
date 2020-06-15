using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmChecker : MonoBehaviour
{

    private NickTestScript parentScript;

    //the position where other pieces move to
    public Transform connectionPosition;


    // Start is called before the first frame update
    void Start()
    {
        parentScript = GetComponentInParent<NickTestScript>();  
    }

   
    public void OnTriggerEnter(Collider other)
    {
        //Checks if the player is not holding the object and the tag of the entered collier
        if (other.tag == "Finish" && parentScript.holdingPiece == false /*&& other.gameObject.GetComponent<NickTestScript>().connected == false*/)
        {
            parentScript.pieceRigidBody.isKinematic = true;
            parentScript.pieceRigidBody.useGravity = false;
            parentScript.pieceRigidBody.rotation = other.gameObject.GetComponent<ArmChecker>().connectionPosition.transform.rotation;
            parentScript.pieceRigidBody.position = other.gameObject.GetComponent<ArmChecker>().connectionPosition.transform.position;
           // parentScript.connected = true;
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Finish")
        {
            parentScript.pieceRigidBody.useGravity = true;
           // parentScript.connected = false;
        }
    }
}
