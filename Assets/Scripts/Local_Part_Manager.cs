using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VR;
using UnityEngine.EventSystems;

public class Local_Part_Manager : MonoBehaviour
{
    //This sits on the parent obejct in every part prefab.

    #region Matt's vars
    // Part properties
    public Shape shape;
    public int color;
    public bool flipped;
    //Transitioning Part_Creator to use references to the masterblueprint. This node is a reference to the part on the master blueprint that it's associated with.
    public Node masterBlueprintReference;
    [HideInInspector]
    public Part_Manager globalPartManager;
    [HideInInspector]
    public Socket_Laser socketLaser;
    private Local_Part_Manager thisScript;

    public List<GameObject> socketList;
    public bool[] socketOccupied;

    //This gets set on part_manager and refers to the node that this part is associated with in playerBlueprint. Used to get a parent node from socketObject
    [HideInInspector]
    public Node thisNode;

    //Glowfade // All these values are in seconds.
    public float colorAttackTime = 0.3f;
    public float colorSustainTime = 0.4f;
    public float silhouetteAttackTime = 0.2f;

    //Glowfade shader properties
    //public float colorMeter;
    //public float shadowMeter;
    public Material materialRef;

    //cool movement
    private Vector3? targetPosition;
    private Quaternion? targetRotation;
    //rotate speed is in degrees per sec
    private float rotateSpeed;
    private float moveSpeed;
    private GameObject coreParent;
    #endregion

    private Rigidbody localRigidBody;
    private bool selected;
    private Transform myHand;
    private bool holdingPiece;
    private Transform origParent;

    // stores the script of a parent part.
    private Local_Part_Manager parentPartScript;
    private GameObject parentSocket;
    private GameObject[] childObjects;

    //stores true when this part is on the player's robot model
    private bool onModel;

    #region matt's code
    public void Initalise()
    {
        materialRef = gameObject.transform.GetChild(0).GetComponent<Renderer>().material;
        //colorMeter = materialRef.GetFloat("colorMeter");
        //shadowMeter = materialRef.GetFloat("shadowMeter");

        //weird script reference setting
        GameObject gameManagerObject = GameObject.Find("GameManager");
        globalPartManager = gameManagerObject.GetComponent<Part_Manager>();
        var gameManager = gameManagerObject.GetComponent<Game_Manager>();
        socketLaser = gameManager.socketLaser;
        
        //grab these from the GameManager
        rotateSpeed = gameManager.rotateSpeed;
        moveSpeed = gameManager.moveSpeed;

        coreParent = gameManager.partCreator.coreParent;
        thisScript = gameObject.GetComponent<Local_Part_Manager>();

        myHand = gameManagerObject.GetComponent<Game_Manager>().myHand;
        localRigidBody = gameObject.GetComponent<Rigidbody>();

        //grabs the parent when initalised, the part parent object.
        origParent = transform.parent;

        //sets the length of socketOccupied
        socketOccupied = new bool[socketList.Count];

        //turn off the socket colliders
        if(shape != Shape.corePart00 || shape != Shape.corePart01)
        {
            SocketToggle(false);
        } else
        {
            masterBlueprintReference = globalPartManager.masterBlueprint[0];
        }

        childObjects = new GameObject[socketList.Count];
    }

    public void SetMasterBlueprintRef(Node masterNode)
    {
        //Debug.Log("ran");
        masterBlueprintReference = masterNode;
        //Debug.Log($"Master blueprint ref = {masterBlueprintReference.shape}");
    }

    private void SocketToggle(bool enable, GameObject specificSocket = null)
    {
        foreach (var socket in socketList)
        {
            if (socket == specificSocket || specificSocket == null)
            {
                socket.GetComponent<Collider>().enabled = enable;
            }
        }
    }

    private void PlayParticleSystem(GameObject socket, bool enable = true)
    {
        for (int i = 0; i < socket.transform.childCount; i++)
        {
            if (enable)
            {
                socket.transform.GetChild(0).GetChild(i).GetComponent<ParticleSystem>().Play();
            }
            else
            {
                socket.transform.GetChild(0).GetChild(i).GetComponent<ParticleSystem>().Stop();
            }
        }
    }

    public void Connect(GameObject socket)
    {
        PlayParticleSystem(socket);
    }

    public void Disconnect(GameObject socket)
    {
        PlayParticleSystem(socket, false);
    }

    public void SetRotate(GameObject socket)
    {
        targetRotation = socket.transform.rotation;
    }

    public void UnsetRotate()
    {
        //targetRotation = null;
    }

    public void AddPieceToGlobalManager(GameObject socketObject)
    {
        //tree stuff
        globalPartManager.AddPlayerPart(shape, color, socketObject, gameObject);

        //socket stuff
        SocketToggle(true);
        Audio_Manager.Play("Part_Connect");

        //connect to object
        AddToModel(socketLaser.previousCollider.gameObject);
    }

    public void RemovePieceFromGlobalManager()
    {
        //tree stuff
        globalPartManager.RemovePlayerPart(gameObject);
        //socket stuff
        SocketToggle(false);
        //this object stuff
        RemoveFromModel();
    }

    public IEnumerator GlowFade()
    {
        //color increases every frame
        for (float i = 0; i < colorAttackTime; i += Time.deltaTime)
        {
            materialRef.SetFloat("colorMeter", i / colorAttackTime);

           yield return null;
        }

        //color sustain
        materialRef.SetFloat("colorMeter", 1 );

       yield return new WaitForSeconds(colorSustainTime);

        for (float i = 0; i < silhouetteAttackTime; i += Time.deltaTime)
        {
            materialRef.SetFloat("shadowMeter", i / silhouetteAttackTime);
            yield return null;
        }

        materialRef.SetFloat("shadowMeter", 1);

        globalPartManager.finishedDemos++;
    }
    #endregion

    //occurs when reticle is pointing over object and player presses button
    public void PieceInteract()
    {
        //tag names need to be changed

        //if player is not holding piece and the piece is being looked at
        if (holdingPiece == false && selected == true)
        {
            Pickup();
        }
        //drops the piece if it's being held, and it becomes subject to gravity
        else if (holdingPiece == true)
        {
            PutDown();
        }
    }

    private void Pickup()
    {
        if (onModel)
        {
            RemovePieceFromGlobalManager();
        }


        //parents the object to the hand so that it follows where the player looks
        transform.parent = myHand.transform;
        localRigidBody.isKinematic = true;
        holdingPiece = true;
        socketLaser.currentHeldScriptRef = thisScript;
        localRigidBody.useGravity = false;

        //changes the position and rotation of the object to be the same distance/rotation each time
        transform.position = myHand.transform.position;
        localRigidBody.transform.position = myHand.position;

        //shader select
        materialRef.SetFloat("frenSelect", 1);
    }

    private void PutDown()
    {
        if (socketLaser.connected)
        {
            //put on the model

            //connect to tree
            AddPieceToGlobalManager(socketLaser.previousCollider.gameObject);
            
        } else
        {
            //put on the ground

            transform.parent = origParent;
            localRigidBody.isKinematic = false;
            //targetRotation = null is otherwise commented out
            targetRotation = null;
            localRigidBody.useGravity = true;
        }

        //universal changes
        holdingPiece = false;
        socketLaser.currentHeldScriptRef = null;

        //shader deselect
        materialRef.SetFloat("frenSelect", 0);
    }

    public void AddToModel(GameObject socket)
    {
        transform.parent = coreParent.transform;
        targetPosition = socket.transform.position;
        onModel = true;
        localRigidBody.isKinematic = true;
        localRigidBody.useGravity = false;

        parentPartScript = socket.transform.parent.gameObject.GetComponent<Local_Part_Manager>();
        parentSocket = socket;
        parentPartScript.SocketToggle(false, socket);

    }
    public void RemoveFromModel()
    {
        targetPosition = null;
        targetRotation = null;
        onModel = false;

        parentPartScript.SocketToggle(true, parentSocket);
    }

    public void RemoveRecursive()
    {

    }

    private void Update()
    {
        if (targetPosition != null)
        {
            // The step size is equal to speed times frame time.
            var step = moveSpeed * Time.deltaTime;

            // Move our transform a step closer to the target's.
            transform.position = Vector3.MoveTowards(transform.position, targetPosition.Value, step);
        }

        if (holdingPiece || onModel)
        {
            //Debug.Log(masterBlueprintReference);

            //var moveTowards = Vector3.MoveTowards(transform.position, targetPosition, maxReadjustSpeed);
            //localRigidBody.velocity.Set(moveTowards.x, moveTowards.y, moveTowards.z);
            //localRigidBody.MovePosition(moveTowards);
            if (targetRotation != null)
            {
                // The step size is equal to speed times frame time.
                var step = rotateSpeed * Time.deltaTime;

                // Rotate our transform a step closer to the target's.
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation.Value, step);
            }
        }
    }

    //sets game object currently being looked at
    //occurs when player looks at object
    public void ActiveLook()
    {
        //tag names need to be changed
        selected = true;
        materialRef.SetFloat("frenSelect", 1);
        //this.gameObject.GetComponentInChildren<Renderer>().material.color = hoverMaterial.color;
    }
    //occurs when player stops looking at object
    public void DeActiveLook()
    {
        selected = false;
        if (!holdingPiece)
        {
            materialRef.SetFloat("frenSelect", 0);
        }
        //this.gameObject.GetComponentInChildren<Renderer>().material.color = baseMaterial.color;
    }
}
