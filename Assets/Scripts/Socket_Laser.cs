using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Socket_Laser : MonoBehaviour
{
    //script references
    [HideInInspector]
    public Part_Manager partManager;
    private Local_Part_Manager currentPartManager;

    //ray vars
    private RaycastHit hit;
    public float distanceOfRay = 10;
    public LayerMask layerMask;

    //other vars
    public Collider previousCollider;
    private Local_Part_Manager currentParticleScriptRef;
    public Local_Part_Manager currentHeldScriptRef;
    public bool connected;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        bool raycast = Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, distanceOfRay, layerMask);

        if (raycast)
        {   
            if(hit.collider != previousCollider)
            {
                //disconnect previous particle system before connecting to the new one
                if(previousCollider != null)
                {
                    currentParticleScriptRef.Disconnect(previousCollider.gameObject);
                    if (currentHeldScriptRef != null)
                    {
                        currentHeldScriptRef.UnsetRotate();
                    }
                    connected = false;
                }
                
                currentParticleScriptRef = hit.collider.transform.parent.GetComponent<Local_Part_Manager>();
                
                //connect enables the new particle system
                currentParticleScriptRef.Connect(hit.collider.gameObject);
                if (currentHeldScriptRef != null)
                {
                    currentHeldScriptRef.SetRotate(hit.collider.gameObject);
                }
                
                connected = true;

                //reset previousCollider
                previousCollider = hit.collider;
            }
        }
        else
        {
            //disconnect disables the particle system
            if (previousCollider != null)
            {
                currentParticleScriptRef.Disconnect(previousCollider.gameObject);
                if (currentHeldScriptRef != null)
                {
                    currentHeldScriptRef.UnsetRotate();
                }
            }
            previousCollider = null;
            currentParticleScriptRef = null;
            connected = false;
        }
    }
}
