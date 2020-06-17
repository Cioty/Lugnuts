using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part_Manager : MonoBehaviour
{
    public Part_Generation partGenScript;
    public Part_Creator partCreator;

    //variables from partGenScript
    [HideInInspector]
    public List<Node> masterBlueprint;
    private List<Node> subBlueprint;
    [HideInInspector]
    public List<Node> playerBlueprint;

    //public class PseudoNode
    //{
    //    public PseudoNode[] children;
    //    public PseudoNode parent;
    //
    //    //shape is the number at the end of prefabPart
    //    public Shape shape;
    //    //color is currently arbitrary
    //    public int color;
    //    public bool flipped;
    //}


    public void CompareBlueprints()
    {

    }

    public void CreatePlayerBlueprint()
    {
        playerBlueprint = new List<Node>() { masterBlueprint[0] };
    }

    public void AddPlayerPart(Shape passedShape, int passedColor, GameObject socketObject, GameObject newObject)
    {
        #region add to playerBlueprint
        //parent vars
        var parentObject = socketObject.transform.parent.gameObject;
        var parentObjectScript = parentObject.GetComponent<Local_Part_Manager>();
        var parentSocketList = parentObjectScript.socketList;
        var parentNode = parentObjectScript.thisNode;
        bool flipped = parentObjectScript.flipped;

        //newObject vars
        var newObjectScript = newObject.GetComponent<Local_Part_Manager>();

        //new node definition
        var newNode = new Node()
        {
            shape = passedShape,
            color = passedColor,
            children = new Node[newObjectScript.socketList.Count],
            parent = parentNode
        };

        //newNode added to player blueprint
        playerBlueprint.Add(newNode);

        //add thisNode to local manager on newObject
        newObjectScript.thisNode = newNode;

        //Add to parentNode children
        for (int i = 0; i < parentSocketList.Count; i++)
        {
            if(parentSocketList[i] = socketObject)
            {
                if(flipped)
                {
                    parentNode.children[parentSocketList.Count - i] = newNode;
                } else
                {
                    parentNode.children[i] = newNode;
                }
            }
        }
        #endregion

        #region update Part_Creator

            //var masterBlueprintReference = newObjectScript.masterBlueprintReference;
            //partCreator.UpdateAllowedParts(new List<Node> { masterBlueprintReference });

        //edgeList takes in a node reference from playerBlueprint
        partCreator.UpdateEdgeList(newNode, true);
        //futureNodes takes in a node reference from masterBlueprint
        partCreator.UpdateFutureNodes(newObjectScript.masterBlueprintReference, true);

        #endregion
    }

    public void RemovePlayerPart(GameObject removedObject)
    {
        
        var removedObjectScript = removedObject.GetComponent<Local_Part_Manager>();

        #region update Part_Creator

        //edgeList takes in a node reference from playerBlueprint
        partCreator.UpdateEdgeList(removedObjectScript.thisNode, false);
        //futureNodes takes in a node reference from masterBlueprint
        partCreator.UpdateFutureNodes(removedObjectScript.masterBlueprintReference, false);

        #endregion
    }

    //private void EdgeListRecursiveRemove(Node searchNode)
    //{
    //    foreach (var child in searchNode.children)
    //    {
    //        if (child != null)
    //        {
    //            if (edgeList.Contains(child))
    //            {
    //                edgeList.Remove(child);
    //            }
    //            else
    //            {
    //                EdgeListRecursiveRemove(child);
    //            }
    //        }
    //    }
    //}
}
