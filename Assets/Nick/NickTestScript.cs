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
    [HideInInspector]
    public bool holdingPiece = false;
    public bool lookedAt = false;
    public Material hoverMaterial;
    public Material baseMaterial;
    //public Transform connectionArea;
    //public GameObject[] connectionPoints;

    void Start()
    {
        piece = this.gameObject.GetComponent<Rigidbody>();
        /*
        for(int i = 0; i <= connectionPoints.Length; i++)
        {
            connectionPoints[i].GetComponent<ArmChecker>().connectionNumber = i;
        }
        */
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
        this.gameObject.GetComponent<Renderer>().material.color = hoverMaterial.color;
    }
    public void DeActiveLook()
    {
        this.gameObject.tag = "Untagged";
        this.gameObject.GetComponent<Renderer>().material.color = baseMaterial.color;

    }
    
}
