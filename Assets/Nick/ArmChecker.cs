using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArmChecker : MonoBehaviour
{

    public NickTestScript parentScript;
    private Local_Part_Manager localManager;

    //the position where other pieces move to
    public Transform connectionPosition;

    //[HideInInspector]
    public bool connected = false;


    // Start is called before the first frame update
    void Start()
    {
        localManager = GetComponentInParent<Local_Part_Manager>();
    }

   
    public void OnTriggerEnter(Collider other)
    {
        //Checks if the player is not holding the object and the tag of the entered collier
        if (other.tag == "Finish" && parentScript.holdingPiece == false && connected == false /*&& other.GetComponentInChildren<ArmChecker>().connected == true*/)
        {
            AddPiece(other.gameObject);
           // parentScript.connected = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Finish")
        {
            RemovePiece();
        }
    }

    public void AddPiece(GameObject socketObject)
    {
        //nick's code
        parentScript.pieceRigidBody.isKinematic = true;
        parentScript.pieceRigidBody.useGravity = false;
        parentScript.pieceRigidBody.rotation = socketObject.GetComponent<ArmChecker>().connectionPosition.transform.rotation;
        parentScript.pieceRigidBody.position = socketObject.GetComponent<ArmChecker>().connectionPosition.transform.position;
        Audio_Manager.Play("Connection");

        localManager.AddPieceToGlobalManager(socketObject);
    }

    public void RemovePiece()
    {

        parentScript.pieceRigidBody.useGravity = true;
        Audio_Manager.Play("Disconnect");

        // parentScript.connected = false;
        localManager.RemovePieceFromGlobalManager();

    }
}
