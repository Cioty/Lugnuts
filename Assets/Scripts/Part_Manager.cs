using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Part_Manager : MonoBehaviour
{
    public Part_Generation partGenScript;

    //variables from partGenScript
    [HideInInspector]
    public List<Node> masterBlueprint;
    private List<Node> subBlueprint;
    [HideInInspector]
    public List<Node> playerBlueprint;

    public void CompareBLueprints()
    {

    }
}
