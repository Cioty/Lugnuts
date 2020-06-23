using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part_Creator : MonoBehaviour
{
    [HideInInspector]
    public List<Node> masterBlueprint;
    public Part_Manager partManager;
    public Game_Manager gameManager;

    public List<Node> futureNodes;
    public List<Node> allowedParts;
    private int allowedPartsMax;
    private int allowedPartsMaxExtra = 2;
    private List<Node> edgeList;

    //public Transform buildLocation;

    //Instantiate the demonstration doll here
    public GameObject dollParent;
    private Animator dollParentAnimator;

    //Instantiate the core part here
    public GameObject coreParent;
    private Animator coreParentAnimator;
    //Core vars
    public GameObject[] coreLibrary;

    //create parts specific
    //part 1
    public float sizeMax = 800;
    public int prioritisedPartsNumber = 2;
    
    //error material for showing parents have null children
    public Material errorMaterial;

    //part 2
    public GameObject[] prefabLibrary;
    public Mesh[][] meshLibrary;
    public Material[][] meshMaterialLibrary;
    public float heightOffset = 0;
    public GameObject partParent;

    // Part prefab public variables
    //public GameObject prefabPart00;
    //public GameObject prefabPart01;
    //public GameObject prefabPart02;
    //public GameObject prefabPart03;
    //public GameObject prefabPart04;
    //public GameObject prefabPart05;
    //public GameObject prefabPart06;
    //
    //Mesh variables //more to be added when artists have them
    public Mesh[] part00Meshes;
    public Mesh[] part01Meshes;
    public Mesh[] part02Meshes;
    public Mesh[] part03Meshes;
    public Mesh[] part04Meshes;
    public Mesh[] part05Meshes;
    public Mesh[] part06Meshes;

    //Material variables //more to be added when artists have them
    public Material[] part00Mats;
    public Material[] part01Mats;
    public Material[] part02Mats;
    public Material[] part03Mats;
    public Material[] part04Mats;
    public Material[] part05Mats;
    public Material[] part06Mats;

    private void Awake()
    {
        meshLibrary = new Mesh[7][];

            meshLibrary[0] = part00Meshes;
            meshLibrary[1] = part01Meshes;
            meshLibrary[2] = part02Meshes;
            meshLibrary[3] = part03Meshes;
            meshLibrary[4] = part04Meshes;
            meshLibrary[5] = part05Meshes;
            meshLibrary[6] = part06Meshes;

        meshMaterialLibrary = new Material[7][];

            meshMaterialLibrary[0] = part00Mats;
            meshMaterialLibrary[1] = part01Mats;
            meshMaterialLibrary[2] = part02Mats;
            meshMaterialLibrary[3] = part03Mats;
            meshMaterialLibrary[4] = part04Mats;
            meshMaterialLibrary[5] = part05Mats;
            meshMaterialLibrary[6] = part06Mats;

        //Get the animator variables so we don't call GetComponent() every time
        dollParentAnimator = dollParent.GetComponent<Animator>();
        partManager.dollParentAnimator = dollParentAnimator;

        coreParentAnimator = coreParent.GetComponent<Animator>();
        gameManager.coreParentAnimator = coreParentAnimator;
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
            var index1 = UnityEngine.Random.Range(0, prioritisedParts.Count);
            toSpawn.Add(prioritisedParts[index1]);
            totalSize += prioritisedParts[index1].size;

            //Add children of the added part to prioritised parts
            foreach (var newChild in Descendents(new List<Node>() { prioritisedParts[index1] }))
            {
                prioritisedParts.Add(newChild);
            }

            prioritisedParts.RemoveAt(index1);

            if(prioritisedParts.Count == 0)
            {
                break;
            }
        }

        while (totalSize <= sizeMax)
        {
            var index2 = UnityEngine.Random.Range(0, allowedParts.Count);
            Node newNode = allowedParts[index2];
            toSpawn.Add(newNode);
            totalSize += newNode.size;
        }

        #endregion

        #region create the parts

        float spawnHeight = 0;

        Debug.Log($"{"toSpawn.Count = "} {toSpawn.Count}");

        foreach (var node in toSpawn)
        {

            GameObject thisPart = SetPrefabAndScriptVars(node);

            var rigidBody = thisPart.GetComponent<Rigidbody>();
            rigidBody.useGravity = true;
            rigidBody.isKinematic = false;

            //instantiate the part
            var thisClone = Instantiate(thisPart, partParent.transform.position + new Vector3(0, spawnHeight, 0), partParent.transform.rotation, partParent.transform);

            if (node.flipped)
            {
                thisClone.transform.localScale = new Vector3(-thisClone.transform.localScale.x, thisClone.transform.localScale.y, thisClone.transform.localScale.z);
            }

            spawnHeight += 0.4f;

            //transform.GetChild(0) SHOULD get the mesh object in each prefab
            SetPartMeshAndMaterial(thisClone.transform.GetChild(0), node);
        }

        #endregion

        //Timer

        //Door Opens
    }

    public void NewLine()
    {
        //Set up lists
        CreateFutureNodes();
        CreateEdgeList();
        CreateAllowedParts();

        BuildRobot(partManager.subBlueprint);
        dollParentAnimator.SetBool("Enter", true);
       
        //NewLine2 is triggered when the demo is over
    }

    public void NewLine2()
    {
        CreateParts();
        InstantiateCore(coreParent);
        coreParentAnimator.SetBool("Enter", true);
    }

    public GameObject InstantiateCore(GameObject positionObject)
    {
        GameObject spawnObject = coreLibrary[masterBlueprint[0].shape - Shape.corePart00];

        var returnObject = Instantiate(spawnObject, positionObject.transform.position, positionObject.transform.rotation, positionObject.transform);
        //returnObject.GetComponent<Local_Part_Manager>().Initalise();
        return returnObject;
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
        allowedParts = new List<Node>();
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

    public void SetPartMeshAndMaterial(Transform instance, Node dataNode, bool thisPartError = false )
    {
        if (!thisPartError)
        {
            instance.GetComponent<MeshFilter>().mesh = meshLibrary[(int)dataNode.shape][dataNode.color]; //Set mesh filter
            instance.GetComponent<MeshRenderer>().material = meshMaterialLibrary[(int)dataNode.shape][dataNode.color]; //Set material relative to mesh
        }
        else //this else is a debug thing that adds the error material if the parent's child is null.
        {
            instance.GetComponent<MeshFilter>().mesh = meshLibrary[(int)dataNode.shape][dataNode.color]; //Set mesh filter
            instance.GetComponent<MeshRenderer>().material = errorMaterial;
        }
        instance.parent.GetComponent<Local_Part_Manager>().Initalise();
    }

    public GameObject SetPrefabAndScriptVars(Node node)
    {
        //Create the part to be instantiated
        GameObject thisPart = prefabLibrary[(int)node.shape];

        //set variables on Local_Part_Manager
        var thisPartManager = thisPart.GetComponent<Local_Part_Manager>();

        thisPartManager.color = node.color;
        thisPartManager.shape = node.shape;
        thisPartManager.flipped = node.flipped;

        //node SHOULD come from the master blueprint. Descendents and FutureNodes both interface and get references from masterBlueprint.
        thisPartManager.masterBlueprintReference = node;
        thisPartManager.globalPartManager = partManager;

        return thisPart;
    }

    public void BuildRobot(List<Node> blueprint)
    {
        for (int i = 0; i < blueprint.Count; i++)
        {
            
            //this does the instantiating. The else covers non-core parts and makes sure they're instantiated on the right socket.
            if(blueprint[i].id == 0)
            {
                var thisClone = InstantiateCore(dollParent);
                blueprint[i].demonstrationObject = thisClone;
            } else
            {
                GameObject thisPart = SetPrefabAndScriptVars(blueprint[i]);

                var rigidBody = thisPart.GetComponent<Rigidbody>();
                rigidBody.useGravity = false;
                rigidBody.isKinematic = true;
                //this is used to apply an error texture after spawning if it's set to a null child. You can remove this after the bug is fixed.
                bool thisPartError = false;

                //socket stuff
                int socketIndex = -1;

                if (!blueprint[i].parent.flipped)
                {
                    for (int j = 0; j < blueprint[i].parent.children.Length; j++)
                    {
                        if (blueprint[i].parent.children[j] == blueprint[i])
                        {
                            socketIndex = j;
                        }
                    }
                    //this is debug stuff. You can remove once this is fixed.
                    if (socketIndex == -1)
                    {
                        for (int j = 0; j < blueprint[i].parent.children.Length; j++)
                        {
                            //if the socket index is zero, this sets it to the null value.
                            if (blueprint[i].parent.children[j] == null)
                            {
                                socketIndex = j;
                                thisPartError = true;
                            }
                            //debug.logs
                            if (blueprint[i].parent.children[j] != null)
                            {
                                Debug.Log($"{"non-flipped"} {blueprint[i].parent.children[j].shape}");
                            } else
                            {
                                Debug.Log($"{"non-flipped"} {"null"}");
                            }
                        }
                        
                    }
                } else
                {
                    for (int j = 0; j < blueprint[i].parent.children.Length; j++)
                    {
                        if (blueprint[i].parent.children[j] == blueprint[i])
                        {
                            socketIndex = blueprint[i].parent.children.Length-1-j;
                        }
                    }
                    //this is debug stuff. You can remove once this is fixed.
                    if (socketIndex == -1)
                    {
                        for (int j = 0; j < blueprint[i].parent.children.Length; j++)
                        {
                            //if the socket index is zero, this sets it to the null value.
                            if (blueprint[i].parent.children[j] == null)
                            {
                                socketIndex = blueprint[i].parent.children.Length - 1 - j;
                                thisPartError = true;
                            }
                            //debug.logs
                            if (blueprint[i].parent.children[j] != null)
                            {
                                Debug.Log($"{"flipped"} {blueprint[i].parent.children[j].shape}");
                            }
                            else
                            {
                                Debug.Log($"{"flipped"} {"null"}");
                            }
                        }

                    }
                }

                //if(socketIndex == -1)
                //{
                //    Debug.Log($"{blueprint[i].parent.children.Length}");  
                //    for (int j = 0; j < blueprint[i].parent.children.Length; j++)
                //    {
                //        Debug.Log($"{blueprint[i].parent.children[j] == null}");
                //    }   
                //}

                if(blueprint[i].parent.demonstrationObject.GetComponent<Local_Part_Manager>().socketList.Count == socketIndex)
                {
                    Debug.Log($"{"parent flipped = "}{blueprint[i].parent.flipped}"); 
                }

                // Sets parentSocket, the object that the new part will be instantiated as a child of, in an overly convoluted way.
                //Debug.Log($"{"socketlist count and index = "} {blueprint[i].parent.demonstrationObject.GetComponent<Local_Part_Manager>().socketList.Count} {"socket index = "} {socketIndex}");
                GameObject parentSocket = blueprint[i].parent.demonstrationObject.GetComponent<Local_Part_Manager>().socketList[socketIndex];

                var thisClone = Instantiate(thisPart, parentSocket.transform.position, parentSocket.transform.rotation, dollParent.transform);

                if (blueprint[i].flipped)
                {
                    thisClone.transform.localScale = new Vector3(-thisClone.transform.localScale.x, thisClone.transform.localScale.y, thisClone.transform.localScale.z);
                }

                blueprint[i].demonstrationObject = thisClone;

                SetPartMeshAndMaterial(thisClone.transform.GetChild(0), blueprint[i], thisPartError);
            }

            

        }
    }
}
