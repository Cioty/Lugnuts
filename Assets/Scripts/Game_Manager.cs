using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    //script references
    public Part_Manager partManager;
    public Part_Generation partGeneration;
    public Part_Creator partCreator;
    public Socket_Laser socketLaser;
    public Crate_Manager crateManager;
    public Button_Script buttonScript;
    public Lever leverScript;

    //part Parent location reference
    public GameObject partParentLocation;

    //this gets incremented when you send off an incorrect model
    public int failureAmount = 0;
    public int failureMax = 3;

    //this is the index for subsetBlueprint
    public int modelNumber = 0;
    public int modelNumberPlus = 2;

    //animator reference
    [HideInInspector]
    public Animator coreParentAnimator;

    //hand reference
    public Transform myHand;

    //part rotate speed
    public float rotateSpeed = 400f;
    public float moveSpeed = 1f;

    public void Awake()
    {
        socketLaser.partManager = partManager;
        crateManager = gameObject.GetComponent<Crate_Manager>();
    }

    public void Start()
    {
        NewLine();

        //maybe add feedback here

        //call new model

    }

    public void Success()
    {
        modelNumber++;
        partManager.PartRemove();

        //maybe add feedback here

        NewModel();

    }

    public void Failure()
    {
        failureAmount++;
        partManager.PartRemove();

        if(failureAmount == failureMax)
        {
            //reset then NewLine()

            NewLine();
        } else
        {
            NewModel();
            partCreator.NewModel2();
        }
        
    }

    public void RemoveCore()
    {
        coreParentAnimator.SetBool("Enter", false);
    }

    public void NewLine()
    {
        partGeneration.GenerateMasterBlueprint();
        //create player blueprint moved from here to new line
        NewModel();
        partCreator.NewLine();
    }

    public void NewModel()
    {
        partManager.CreatePlayerBlueprint();
        partManager.SubsetBlueprint(partManager.masterBlueprint.Count - 1); /*partManager.masterBlueprint.Count- 1*/ /*modelNumber + modelNumberPlus*/
    }
}
