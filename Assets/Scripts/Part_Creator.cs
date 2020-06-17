using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part_Creator : MonoBehaviour
{
    [HideInInspector]
    public List<Node> masterBlueprint;
    public Part_Manager partManager;

    public List<Node> futureNodes;
    public List<Node> allowedParts;
    private int allowedPartsMax;
    private int allowedPartsMaxExtra = 2;
    private List<Node> edgeList;

    public Transform buildLocation;



    //create parts specific
    //part 1
    public float sizeMax = 800;
    public int prioritisedPartsNumber = 2;
    

    //part 2
    public GameObject[] prefabLibrary;
    public Mesh[] meshLibrary;
    public Material[,] meshMaterialLibrary;
    public float heightOffset = 0;
    public GameObject partParent;

    private void Start()
    {
        
    }

    //This spawns the parts for the crate
    public void CreateParts()
    {
        #region generate the toSpawn list

        UpdateAllowedParts(edgeList);

        float totalSize = 0;

        List<Node> toSpawn = new List<Node>();

        //Add descendants to the spawn list
        var prioritisedParts = Descendents(edgeList);

        //Add prioritised parts
        for (int i = 0; i < prioritisedPartsNumber; i++)
        {
            //choose a random part and add it
            var index = UnityEngine.Random.Range(0, prioritisedParts.Count);
            toSpawn.Add(prioritisedParts[index]);
            totalSize += prioritisedParts[index].size;

            //Add children of the added part to prioritised parts
            foreach (var newChild in Descendents(new List<Node>() { prioritisedParts[index] }))
            {
                prioritisedParts.Add(newChild);
            }

            prioritisedParts.RemoveAt(index);

            if(prioritisedParts.Count == 0)
            {
                break;
            }
        }

        while (totalSize <= sizeMax)
        {
            var index = UnityEngine.Random.Range(0, allowedParts.Count);
            Node newNode = prioritisedParts[index];
            toSpawn.Add(newNode);
            totalSize += newNode.size;
        }

        #endregion

        #region create the parts

        float spawnHeight = 0;

        foreach (var node in toSpawn)
        {
            //Create the part to be instantiated
            GameObject thisPart = prefabLibrary[(int)node.shape];
            //transform.GetChild(0) SHOULD get the mesh object in each prefab
            var thisPartMeshChild = thisPart.transform.GetChild(0);
            thisPartMeshChild.GetComponent<MeshFilter>().mesh = meshLibrary[(int)node.shape]; //Set mesh filter
            thisPartMeshChild.GetComponent<MeshRenderer>().material = meshMaterialLibrary[(int)node.shape, node.color]; //Set material relative to mesh

            //set variables on Local_Part_Manager
            var thisPartManager = thisPart.GetComponent<Local_Part_Manager>();

            thisPartManager.shape = node.shape;
            thisPartManager.color = node.color;
            thisPartManager.flipped = node.flipped;

            //node SHOULD come from the master blueprint. Descendents and FutureNodes both interface and get references from masterBlueprint.
            thisPartManager.masterBlueprintReference = node;
            thisPartManager.globalPartManager = partManager;

            //instantiate the part
            Instantiate(thisPart, partParent.transform.position + new Vector3(0, spawnHeight, 0), Quaternion.identity, partParent.transform);
            spawnHeight += 0.3f;
        }

        #endregion

        //Timer

        //Door Opens
    }

    public void NewModel()
    {
        //Set up lists
        CreateFutureNodes();
        CreateAllowedParts();
        CreateEdgeList();

        //but a timer before this.
        CreateParts();

        //
    }

    #region parameters for generation
    //descendants looks through the master blueprint and gets descendants of the nodes input into the method
    public List<Node> Descendents(List<Node> ancestorNodes)
    {
        var returnList = new List<Node>();

        foreach (var index in ancestorNodes)
        {
            // This foreach works because masterBlueprint is ordered by id. Therefore id = List index.
            foreach (var child in index.children)
            {
                returnList.Add(child);
            }
        }

        return returnList;
    }

    
    public void CreateAllowedParts()
    {
        allowedParts = new List<Node>() { masterBlueprint[0] };
        UpdateAllowedParts(new List<Node>() { masterBlueprint[0] });
    }

    public void UpdateAllowedParts(List<Node> ancestorNodes)
    {
        var descList = Descendents(ancestorNodes);

        foreach (var descendant in descList)
        {
            if (!allowedParts.Contains(descendant))
            {
                allowedParts.Add(descendant);
                futureNodes.Remove(descendant);
            }
        };

        allowedPartsMax = partManager.playerBlueprint.Count + descList.Count + allowedPartsMaxExtra;

        while (allowedParts.Count < allowedPartsMax)
        {
            
            var randomIndex = UnityEngine.Random.Range(0, futureNodes.Count);
            //Adds a random node from future nodes to allowed parts.
            allowedParts.Add(futureNodes[randomIndex]);
            UpdateFutureNodes(futureNodes[randomIndex], true);
        }
    }

    public void CreateFutureNodes()
    {
        futureNodes = new List<Node>();
            //foreach (Node item in masterBlueprint)
            //{
            //    futureIds.Add(item.id);
            //}
        for (int i = 1; i < masterBlueprint.Count; i++)
        {
            futureNodes.Add(masterBlueprint[i]);
        }
        
    }

    //call this when adding a part. 
    //This takes in a node reference from the master blueprint
    public void UpdateFutureNodes(Node newNode, bool addition)
    {
        // This should show the ids that aren't on the robot or already in the allowed list.
        // call this when you place a part down.
        if(addition)
        {
            if (futureNodes.Contains(newNode))
            {
                futureNodes.Remove(newNode);
            }
        } else
        {
            futureNodes.Add(newNode);
            futureNodesRecursiveAdd(newNode);
        }
    }

    private void futureNodesRecursiveAdd(Node searchNode)
    {
        foreach (var child in searchNode.children)
        {
            if (!futureNodes.Contains(child))
            {
                futureNodes.Add(child);
                futureNodesRecursiveAdd(child);
            }
        }
    }


    public void CreateEdgeList()
    {
        edgeList = new List<Node> { masterBlueprint[0] };
    }

    //call when adding or removing a part
    //This takes in a node reference from the player blueprint
    public void UpdateEdgeList(Node newNode, bool addition)
    {
        if (addition)
        {
            edgeList.Add(newNode);

            int nullChildCount = 0;
            foreach (var child in newNode.parent.children)
            {
                if(child == null)
                {
                    nullChildCount++;
                }
            }
            if(nullChildCount == 0)
            {
                edgeList.Remove(newNode.parent);
            }
        } else
        {
            edgeList.Add(newNode.parent);
            if (edgeList.Contains(newNode))
            {
                edgeList.Remove(newNode);
            } else
            {
                EdgeListRecursiveRemove(newNode);
            }
        }
    }
    #endregion

    //called in UpdateEdgelist in order to remove children of a removed node. Searches through playerBlueprint.
    private void EdgeListRecursiveRemove(Node searchNode)
    {
        foreach (var child in searchNode.children)
        {
            if(child != null)
            {
                if (edgeList.Contains(child))
                {
                    edgeList.Remove(child);
                }
                else
                {
                    EdgeListRecursiveRemove(child);
                }
            } 
        }
    }

    public void BuildRobot(List<Node> blueprint)
    {
       // Instantiate(blueprint[0]., buildLocation.position,
       //     Quaternion.Euler(new Vector3
       //     (
       //         buildLocation.rotation.x,
       //         buildLocation.rotation.y,
       //         buildLocation.rotation.z)
       //     ),
       //     buildLocation);
    }
}
