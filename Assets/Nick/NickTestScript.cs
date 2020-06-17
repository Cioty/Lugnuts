using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NickTestScript : MonoBehaviour
{
    //Transform of the reticle/player gaze
    public Transform myHand;

    //Player's transform
    public Transform player;

    //The piece's gameobject
    public GameObject thisGameObject;

    //The rigidbody on the piece
    public Rigidbody pieceRigidBody;

    //bool that checks if the player is holding a piece
    [HideInInspector]
    public bool holdingPiece = false;

    //bool that checks if a piece is connected to another piece
    [HideInInspector]
    public bool connected = false;

    //Materials need to be changed to shaders later
    //Materials/Shaders that change if player is looking at object
    public Material hoverMaterial;
    public Material baseMaterial;
   

    void Start()
    {
        //finds the piece's rigidbody
        pieceRigidBody = this.gameObject.GetComponent<Rigidbody>();
    }


    //occurs when reticle is pointing over object and player presses button
    public void pieceInteract()
    {
        //tag names need to be changed

        //if player is not holding piece and the piece is being looked at
        if (holdingPiece == false && this.gameObject.tag == "Respawn")
        {
            this.gameObject.transform.rotation = new Quaternion(0, 0, 0, 0);
            //parents the hand to the object so that it follows where the player looks
            pieceRigidBody.transform.parent = myHand.transform;
            holdingPiece = true;
            pieceRigidBody.isKinematic = true;
            //changes the position and rotation of the object to be the same distance/rotation each time
            this.gameObject.transform.position = myHand.transform.position;
            

        }
        //drops the piece if it's being held, and it becomes subject to gravity
        else if (holdingPiece == true)
        {
            pieceRigidBody.transform.parent = null;
            pieceRigidBody.isKinematic = false;
            holdingPiece = false;   
        }
    }


    //sets game object currently being looked at
    //occurs when player looks at object
    public void ActiveLook()
    {
        //tag names need to be changed
        this.gameObject.tag = "Respawn";
        this.gameObject.GetComponent<Renderer>().material.color = hoverMaterial.color;
    }
    //occurs when player stops looking at object
    public void DeActiveLook()
    {
        this.gameObject.tag = "Untagged";
        this.gameObject.GetComponent<Renderer>().material.color = baseMaterial.color;

    }
    
}
