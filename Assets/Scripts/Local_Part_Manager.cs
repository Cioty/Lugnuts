using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Local_Part_Manager : MonoBehaviour
{
    //This sits on the parent obejct in every part prefab.

    // Part properties
    [HideInInspector]
    public Shape shape;
    [HideInInspector]
    public int color;
    [HideInInspector]
    public bool flipped;
    //Transitioning Part_Creator to use references to the masterblueprint. This node is a reference to the part on the master blueprint that it's associated with.
    [HideInInspector]
    public Node masterBlueprintReference;
    [HideInInspector]
    public Part_Manager globalPartManager;

    public List<GameObject> socketList;

    //This gets set on part_manager and refers to the node that this part is associated with in playerBlueprint. Used to get a parent node from socketObject
    [HideInInspector]
    public Node thisNode;

    public void AddPieceToGlobalManager(GameObject socketObject)
    {
        globalPartManager.AddPlayerPart(shape, color, socketObject, gameObject);
    }

    public void RemovePieceFromGlobalManager()
    {
        globalPartManager.RemovePlayerPart(gameObject);
    }
}
