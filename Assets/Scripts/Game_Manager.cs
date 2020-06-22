using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_Manager : MonoBehaviour
{
    //script references
    public Part_Manager partManager;
    public Part_Generation partGeneration;
    public Part_Creator partCreator;

    //this gets incremented when you send off an incorrect model
    public int failureAmount = 0;
    public int failureMax = 3;

    //this is the index for subsetBlueprint
    public int modelNumber = 0;
    public int modelNumberPlus = 2;

    public void Start()
    {
        NewLine();
    }

    public void Success()
    {
        modelNumber++;
    }

    public void Failure()
    {
        failureAmount++;

        if(failureAmount == failureMax)
        {
            //reset then NewLine()

            NewLine();
        }
    }

    public void NewLine()
    {
        partGeneration.GenerateMasterBlueprint();
        partManager.CreatePlayerBlueprint();
        NewModel();
        partCreator.NewLine();
    }

    public void NewModel()
    {
        partManager.SubsetBlueprint(partManager.masterBlueprint.Count-1); /*modelNumber+modelNumberPlus*/
    }
}
