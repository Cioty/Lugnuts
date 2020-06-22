using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Local_Part_Manager : MonoBehaviour
{
    //This sits on the parent obejct in every part prefab.

    // Part properties
    public Shape shape;
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

    //Glowfade // All these values are in seconds.
    public float colorAttackTime = 0.2f;
    public float colorSustainTime = 0.3f;
    public float silhouetteAttackTime = 0.2f;

    //Glowfade shader properties
    public float colorMeter;
    public float shadowMeter;

    private void Awake()
    {
        var materialRef = gameObject.transform.GetChild(0).GetComponent<Renderer>().material;
        //colorMeter = materialRef.colorMeter;
        //shadowMeter = materialRef.shadowMeter;
    }

    public void AddPieceToGlobalManager(GameObject socketObject)
    {
        globalPartManager.AddPlayerPart(shape, color, socketObject, gameObject);
    }

    public void RemovePieceFromGlobalManager()
    {
        globalPartManager.RemovePlayerPart(gameObject);
    }

    public IEnumerator GlowFade()
    {
        //color increases every frame
        for (float i = 0; i < colorAttackTime; i += Time.deltaTime)
        {
            colorMeter = i / colorAttackTime;
            yield return null;
        }

        //color sustain
        colorMeter = 1;
        yield return new WaitForSeconds(colorSustainTime);

        for (float i = 0; i < silhouetteAttackTime; i += Time.deltaTime)
        {
            shadowMeter = i / silhouetteAttackTime;
            yield return null;
        }

        shadowMeter = 1;

    }
}
