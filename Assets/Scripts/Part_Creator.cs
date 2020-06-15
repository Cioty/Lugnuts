using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part_Creator : MonoBehaviour
{
    [HideInInspector]
    public List<Node> masterBlueprint;
    public Part_Manager partManager;

    public List<int> futureIds;
    public List<Node> allowedParts;
    private int allowedPartsMax;
    private int allowedPartsMaxExtra = 2;
    private List<int> edgeList;

    public Transform buildLocation;

    public GameObject[] prefabLibrary;

    private void Start()
    {
        
    }

    //This spawns the parts for the crate
    public void CreateParts()
    {

    }

    public void NewModel()
    {
        //Set up lists
        CreateFutureIds();
        CreateAllowedParts();
        CreateEdgeList();

        //but a timer before this.
        CreateParts();

        //
    }

    public List<Node> Descendents(List<int> ancestorIds)
    {
        var returnList = new List<Node>();

        foreach (int index in ancestorIds)
        {
            // This foreach works because masterBlueprint is ordered by id. Therefore id = List index.
            foreach (var item in masterBlueprint[index].children)
            {
                returnList.Add(item);
            }
        }

        return returnList;
    }

    public void CreateAllowedParts()
    {
        allowedParts = new List<Node>() { masterBlueprint[0] };
        UpdateAllowedParts(new List<int>() { 0 });
    }

    public void UpdateAllowedParts(List<int> ancestorIds)
    {
        var descList = Descendents(ancestorIds);

        foreach (var descendant in descList)
        {
            if (!allowedParts.Contains(descendant))
            {
                allowedParts.Add(descendant);
                futureIds.Remove(descendant.id);
            }
        };

        allowedPartsMax = partManager.playerBlueprint.Count + descList.Count + allowedPartsMaxExtra;

        while (allowedParts.Count < allowedPartsMax)
        {
            
            var randomIndex = UnityEngine.Random.Range(0, futureIds.Count);
            //Adds a random node from future nodes to allowed parts.
            allowedParts.Add(masterBlueprint[futureIds[randomIndex]]);
            UpdateFutureIds(randomIndex);
        }
    }

    public void CreateFutureIds()
    {
        futureIds = new List<int>();
            //foreach (Node item in masterBlueprint)
            //{
            //    futureIds.Add(item.id);
            //}
        for (int i = 1; i < masterBlueprint.Count; i++)
        {
            futureIds.Add(masterBlueprint[i].id);
        }
        
    }

    //call this when adding a part.
    public void UpdateFutureIds(int newId)
    {
        // This should show the ids that aren't on the robot or already in the allowed list.
        // call this when you place a part down.
        if(futureIds.Contains(newId))
        {
            futureIds.Remove(newId);
        }
    }

    public void CreateEdgeList()
    {
        edgeList = new List<int> { 0 };
    }

    public void BuildRobot(List<Node> blueprint)
    {
        Instantiate(blueprint[0].,buildLocation.position, 
            Quaternion.EulerAngles(new Vector3
            (
                buildLocation.rotation.x,
                buildLocation.rotation.y, 
                buildLocation.rotation.z)
            ),
            buildLocation
    }
}
