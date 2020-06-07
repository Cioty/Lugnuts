using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NickTestScript : MonoBehaviour
{
    public Transform myHand;
    public Transform player;
    public GameObject mePiece;
    public Rigidbody piece;
    private bool holdingPiece = false;
    public bool lookedAt = false;
    public SphereCollider entrancePeg;
    public int pieceValue;
    public Text pieceDisplay; 

    void Start()
    {
        piece = this.gameObject.GetComponent<Rigidbody>();
    }

    public void pieceInteract()
    {
        if (holdingPiece == false && this.gameObject.tag == "Respawn")
        {
            piece.transform.parent = myHand.transform;
            holdingPiece = true;
            piece.isKinematic = true;
           mePiece.transform.rotation = Quaternion.LookRotation(myHand.position);

        }
        else if (holdingPiece == true)
        {
            piece.transform.parent = null;
            piece.isKinematic = false;
            holdingPiece = false;

        }
    }

    public void ActiveLook()
    {
        this.gameObject.tag = "Respawn";
    }
    public void DeActiveLook()
    {
        this.gameObject.tag = "Untagged";
    }

    public void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Finish" && holdingPiece == false)
        {
            piece.isKinematic = true;
            piece.useGravity = false;
            pieceDisplay.text = pieceValue.ToString();
        }
    }
    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Finish" && holdingPiece == true)
        {
            piece.useGravity = true;
            pieceDisplay.text = "0";
        }
    }
}
