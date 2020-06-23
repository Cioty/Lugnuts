using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part_Manager : MonoBehaviour
{
    public Part_Generation partGenScript;
    public Part_Creator partCreator;
    public Game_Manager gameManager;

    //variables from partGenScript
    [HideInInspector]
    public List<Node> masterBlueprint;
    [HideInInspector]
    public List<Node> subBlueprint;
    [HideInInspector]
    public List<Node> playerBlueprint;

    //follow me vars
    private Queue<bool> followQueue;
    private Queue<bool> followerQueue;

    //DollDemonstration Values
    public float demoWaitTime = 1; //time in seconds between GlowFade starting on one part and it activating on the next one.
    [HideInInspector]
    public Animator dollParentAnimator;
    private int startedDemos = 0;
    [HideInInspector]
    public int finishedDemos = 0;

    public void CompareBlueprints()
    {
        //reset follow queues
        followQueue     =   new Queue<bool>();
        followerQueue   =   new Queue<bool>();

        //get queues and follow queues
        var subBlueprintQueue     = BFSTree(subBlueprint[0], followQueue);
        var playerBlueprintQueue  = BFSTree(playerBlueprint[0], followerQueue);

        //check follow queues are equal in length
        if(followQueue.Count != followerQueue.Count)
        {
            CompareFailed();
            return;
        }

        // check follow queues are equal in quality 
        // This checks for socket-position inconsistencies in tree size
        for (int i = 0; i < followQueue.Count; i++)
        {
            if(followQueue.Dequeue() != followerQueue.Dequeue())
            {
                CompareFailed();
                return;
            }
        }

        for (int i = 0; i < subBlueprint.Count; i++)
        {
            Node sub    = subBlueprintQueue.Dequeue();
            Node player = playerBlueprintQueue.Dequeue();
            if
            (
            player.shape == sub.shape &&
            player.color == sub.color &&
            player.flipped == sub.flipped
            )
            continue;
            CompareFailed();
        }
    }

    public void CompareFailed()
    {
        gameManager.Failure();
    }

    public void CompareSuccess()
    {
        gameManager.Success();
    }

    private Queue<Node> BFSTree(Node inputNode, Queue<bool> followQueueIndex)
    {
        //Normal functions
        Queue<Node> retQueue = new Queue<Node>();
        retQueue.Enqueue(inputNode);

        //follow me checks

        // Children
        for (int i = 0; i < inputNode.children.Length; i++)
        {
            if(inputNode.childrenValid[i] == true)
            {
                BFSTree(inputNode.children[i], followQueueIndex);
                FollowLog(followQueueIndex, true);
            } else
            {
                FollowLog(followQueueIndex, false);
            }
        }

        return new Queue<Node>();
    }

    private void FollowLog(Queue<bool> toFollowQueue, bool data)
    {
        toFollowQueue.Enqueue(data);
    }

    public IEnumerator DollDemonstration()
    {
        startedDemos = 0;
        finishedDemos = 0;

        for (int i = 1; i < subBlueprint.Count; i++)
        {
            StartCoroutine(subBlueprint[i].demonstrationObject.GetComponent<Local_Part_Manager>().GlowFade());
            startedDemos++;
            yield return new WaitForSeconds(demoWaitTime); 
        }

        //this should pause until the coroutines on each part are done
        while(startedDemos != finishedDemos)
        {
            yield return null;
        }

        dollParentAnimator.SetBool("Enter", false);
        partCreator.NewLine2();
    }

    public void CreatePlayerBlueprint()
    {
        playerBlueprint = new List<Node>() { masterBlueprint[0] };
    }

    public void SubsetBlueprint(int index)
    {
        var nullCount = 0;
        foreach (var item in masterBlueprint)
        {
            foreach (var child in item.children)
            {
                if(child == null)
                {
                    nullCount++;
                }
            }
        }
        Debug.Log($"{"nullcount ="} {nullCount} {", Blueprint count ="} {masterBlueprint.Count}");

        List<Node> returnList = new List<Node>();
        // gets all the nodes with an id at or below index
        foreach (Node item in masterBlueprint)
        {
            if (item.id <= index)
            {
                item.childrenValid = new bool[item.children.Length];
                for (int i = 0; i < item.children.Length; i++)
                {
                    if (item.children[i] != null)
                    {
                        if (item.children[i].id <= index)
                        {
                            // This true indicates that the index on the child here is not too high. It is false otherwise.
                            item.childrenValid[i] = true;
                        }
                    }
                } 
                // adds sanitised nodes to the return array
                returnList.Add(item);

            }
        }

        subBlueprint = returnList;
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
                    parentNode.childrenValid[i] = true;
                } else
                {
                    parentNode.children[i] = newNode;
                    parentNode.childrenValid[i] = true;
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
