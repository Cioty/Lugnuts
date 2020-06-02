using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.EventSystems;

public class NickTestScript : MonoBehaviour
{
    public Transform myHand;
    public GameObject mePiece;
    public Rigidbody piece;
    private bool holdingPiece = false;

    void Start()
    {
        piece = this.gameObject.GetComponent<Rigidbody>();
    }

    public void pieceInteract()
    {
        if (holdingPiece == false)
        {
            piece.transform.parent = myHand.transform;
            holdingPiece = true;
        }
       else if (holdingPiece == true)
        {
            piece.transform.parent = null;
            holdingPiece = false;
        }
    }
  
 /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Respawn")
        {
            piece.useGravity = false;
        }
        else
        {
            piece.useGravity = true;
        }
    }
    */
}
