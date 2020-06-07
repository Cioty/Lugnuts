using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    public Vector3Int position;
    public Part occupyingPart = null;
}

public class Part
{
    public GameObject prefab = null;
    public bool flip = false;
    public bool[] requiredSpace = null;

    //Use index values
    //-1 = null
    // 0 = socket
    // 1 = part
    // 2 = blocked
    public int useIndex = -1;

    // todo add weight

    // rotation handles:
    // core rotation (to do)
    // socket orientation
    public int rotation = -1;
}

public enum Axis
{
    X,
    Y,
    Z
}

public class Part_Generation: MonoBehaviour
{
    #region declarations
    // Array holding every part that exists besides the core nodes and end-parts
    Part[] partLibrary;

    // Array holding the core nodes
    Part[] coreLibrary;

    // Array holding all the end parts
    Part[] endPartLibrary;

    // Part prefab public variables
    public GameObject prefabPart00;
    public GameObject prefabPart01;
    public GameObject prefabPart02;
    public GameObject prefabPart03;
    public GameObject prefabPart04;
    public GameObject prefabPart05;
    public GameObject prefabPart06;

    //core part public variables
    public GameObject prefabCore01;

    //the socket part
    public Part socket;
    public Part blocked;
    //socket list
    public List<Vector3Int> socketPositions;

    //celldict declaration for grid
    Dictionary<Vector3Int, Cell> cellDict;

    //Test Transform
    public Transform centreTransform;

    public int gridRange = 2;
    #endregion

    private void Start()
    {

        #region part Library definitions
        
        // oh boy here I go hardcoding every part in a single array
        partLibrary = new Part[10]
        {
            //part 1                index 00
            new Part {useIndex = 1, prefab = prefabPart01, flip = false, requiredSpace = new bool[6] {true, false, false, true, false, false}},
            //part 2 and flips      index 01, 02
            new Part {useIndex = 1, prefab = prefabPart02, flip = false, requiredSpace = new bool[6] {true, false, true, false, false, false}},
            new Part {useIndex = 1, prefab = prefabPart02, flip = true, requiredSpace = new bool[6] {true, false, false, false, true, false}},
            //part 3 and flips      index 03, 04
            new Part {useIndex = 1, prefab = prefabPart03, flip = false, requiredSpace = new bool[6] {true, true, false, false, false, false}},
            new Part {useIndex = 1, prefab = prefabPart03, flip = true, requiredSpace = new bool[6] {true, false, false, false, false, true}},
            //part 4 and flips      index 05, 06
            new Part {useIndex = 1, prefab = prefabPart04, flip = false, requiredSpace = new bool[6] {true, false, true, true, false, false}},
            new Part {useIndex = 1, prefab = prefabPart04, flip = true, requiredSpace = new bool[6] {true, false, false, true, true, false}},
            //part 5 and flips      index 07, 08
            new Part {useIndex = 1, prefab = prefabPart05, flip = false, requiredSpace = new bool[6] {true, false, true, false, false, true}},
            new Part {useIndex = 1, prefab = prefabPart05, flip = true, requiredSpace = new bool[6] {true, true, false, false, true, false}},
            //part 6                index 09
            new Part {useIndex = 1, prefab = prefabPart06, flip = false, requiredSpace = new bool[6] {true, false, true, false, true, false}}
        };

        endPartLibrary = new Part[1]
        {
            //endPart 1
            new Part {useIndex = 1, prefab = prefabPart00, flip = false, requiredSpace = new bool[6] {true, false, false, false, false, false}},
        };

        // note, requiredSpace[0] is true on parts but false on cores. This is used in SetPart to determine is a part is a core.
        coreLibrary = new Part[1]
        {
            //core 1.               index 00
            new Part {useIndex = 1, prefab = prefabCore01, flip = false, requiredSpace = new bool[6] {false, true, true, false, true, true}}
        };

        socket = new Part { useIndex = 0 };
        blocked = new Part { useIndex = 2 };

        #endregion

        #region generate the grid

        //generate the grid
        cellDict = new Dictionary<Vector3Int, Cell>();

        //this generates a triple-axis hex grid in cellDict. Use CellTransform() to move between cells.
        for (int i = -gridRange; i <= gridRange; i++)
        {
            for (int j = -gridRange; j <= gridRange; j++)
            {
                //Debug.Log($"{i}, {j}, {-i - j}");

                //without this check we generate a skewed grid that lets z be double gridRange
                if(Mathf.Abs(-i-j) <= gridRange)
                {
                    cellDict.Add(new Vector3Int(i, j, -i - j), new Cell { position = new Vector3Int(i, j, -i - j) });
                }
                
            }
        }
        //need to block off inaccessible grid bits
        #endregion

        #region Actual Generation Code
        var coreGen = coreLibrary[UnityEngine.Random.Range(0, coreLibrary.Length)];

        //The 0 here is temp until we have orientation on core parts
        SetPart(coreGen, 0, Vector3Int.zero);

        //this while loop does all the non-core part placement.
        //while (socketPositions.Count < 0)
        //{
        //    //chooses a random socket from the socketPositions list
        //    var randomSocket = UnityEngine.Random.Range(0, socketPositions.Count);
        //
        //    List<Cell> adjacentCells = CellCheck(socketPositions[randomSocket], cellDict[socketPositions[randomSocket]].occupyingPart.rotation);
        //
        //    List<Part> validPartChoices = new List<Part>();
        //
        //    foreach (var item in partLibrary)
        //    {
        //        //i == 1 because you never have to check the centre hex since there should always be a socket there.
        //        for (int i = 1; i < adjacentCells.Count; i++)
        //        {
        //            //returns true if you can't add the part. You can't add it if occupyingPart isn't null when requiredSpace is true 
        //            if (adjacentCells[i].occupyingPart != null && cellDict[socketPositions[randomSocket]].occupyingPart.requiredSpace[i])
        //            {
        //                break;
        //            }
        //            
        //            if (i==adjacentCells.Count-1)
        //            {
        //                validPartChoices.Add(item);
        //            }
        //        }
        //    }
        //
        //    SetPart
        //        (
        //            validPartChoices[UnityEngine.Random.Range(0, 
        //            validPartChoices.Count)], cellDict[socketPositions[randomSocket]].occupyingPart.rotation, 
        //            socketPositions[randomSocket]
        //        );
        //
        //    socketPositions.RemoveAt(randomSocket);
        //
        //}

        #endregion


    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {

        
            //this while loop does all the non-core part placement.
            if (socketPositions.Count > 0)
            {
                //chooses a random socket from the socketPositions list
                var randomSocket = UnityEngine.Random.Range(0, socketPositions.Count);

                List<Cell> adjacentCells = CellCheck(socketPositions[randomSocket], cellDict[socketPositions[randomSocket]].occupyingPart.rotation);

                List<Part> validPartChoices = new List<Part>();

                foreach (var item in partLibrary)
                {
                    //i == 1 because you never have to check the centre hex since there should always be a socket there.
                    for (int i = 1; i < adjacentCells.Count; i++)
                    {
                        //returns true if you can't add the part. You can't add it if occupyingPart isn't null when requiredSpace is true 
                        if (adjacentCells[i].occupyingPart != null && item.requiredSpace[i])
                        {    
                            break;
                        }

                        if (i == adjacentCells.Count - 1)
                        {
                            validPartChoices.Add(item);
                        }
                    }
                }

                //This adds end-pieces seperately so we have more control
                if (validPartChoices.Count == 0 || (socketPositions.Count > 2))
                {
                    //Adding only one endpart allows for variation in endParts to be added without making them more common in generation
                    validPartChoices.Add(endPartLibrary[UnityEngine.Random.Range(0, endPartLibrary.Length)]);
                }

                //Debug.Log(validPartChoices.Count);
                //foreach (Part part in validPartChoices)
                //{
                //    Debug.Log(part.prefab);
                //}

                SetPart
                    (
                        validPartChoices[UnityEngine.Random.Range(0,validPartChoices.Count)],
                        cellDict[socketPositions[randomSocket]].occupyingPart.rotation,
                        socketPositions[randomSocket]
                    );

                socketPositions.RemoveAt(randomSocket);
                //choose part randomly from list, add part to graph, set weights based on part, add sockets.
            } else
            {
                Debug.Log("No sockets left! SUCCESS");
            }
         }   

        if (Input.GetKeyDown(KeyCode.L))
        {
            for (int i = -gridRange; i <= gridRange; i++)
            {
                for (int j = -gridRange; j <= gridRange; j++)
                {
                    //Debug.Log($"{i}, {j}, {-i - j}");

                    //without this check we generate a skewed grid that lets z be double gridRange
                    if (Mathf.Abs(-i - j) <= gridRange)
                    {
                        if (cellDict[new Vector3Int(i, j, -i - j)].occupyingPart != null)
                        {
                            if (cellDict[new Vector3Int(i, j, -i - j)].occupyingPart.useIndex == 1) {
                                Debug.Log(cellDict[new Vector3Int(i, j, -i - j)].position);
                                Debug.Log(cellDict[new Vector3Int(i, j, -i - j)].occupyingPart.prefab);
                                //Debug.Log(cellDict[new Vector3Int(i, j, -i - j)].occupyingPart.rotation);
                            }
                            
                        }
                    }

                }
            }
        }

        if (Input.GetKeyDown(KeyCode.O))
        {
            Debug.Log(socketPositions.Count);
        }
        
    }

    public List<Cell> CellCheck(Vector3Int initPos, int orientation)
    {
        // orientation is the orientation of the socket-generating part. 0 is down and incrementing moves the socket-part clockwise around initPos
        // position is the centre of the cellcheck.

        #region definitions
        // aCells will become adjacentCells later
        List<Cell> aCells = new List<Cell>();

        // array of positions for the search. Taken from CellTransform
        List<Cell> rotarySearch = GetAdj(initPos);

        #endregion

        // adds cell at initpos
        aCells.Add(cellDict[new Vector3Int(initPos.x, initPos.y, initPos.z)]);

        // does one less loop than would catch all of them
        for (int i = 1; i < 6; i++) //i starts as 1 to offset the search positions to not include the socket location
        {
            Cell indexedCell = rotarySearch[(i + orientation) % 6];

            //feeds in incremented index that loops with modulo
            aCells.Add(indexedCell);
            
        }

        return aCells;

    }

    public void SetPart(Part part, int orientation, Vector3Int position)
    {
        // Orientation is the orientation of the socket-generating part. 0 is down and incrementing moves the socket-part clockwise around initPos
        cellDict[position].occupyingPart = part;
        cellDict[position].occupyingPart.rotation = orientation;

        
        //Test instantiation of part
        var s = 0.25f;
        float xValue = Mathf.Sqrt(3f) * s * (position.y / 2f + position.x);
        float yValue = 3f / 2f * s * position.y;
        Instantiate(part.prefab, new Vector3(centreTransform.position.x + xValue, centreTransform.position.y + yValue, centreTransform.position.z), Quaternion.Euler(0,0,0), centreTransform);
        

        // array of positions for the search. Taken from CellTransform
        List<Cell> rotarySearch = GetAdj(position);

        //Loop through all the adjacent positions. If the part is a core part (which has requiredSpace[0] = false), start at the orientation position.
        for (int i = (0 + Convert.ToInt32(part.requiredSpace[0])); i < 6; i++)
        {
            if (part.requiredSpace[i])
            {
                //stores the position gotten by rotary search
                var searchedCell = rotarySearch[(i + orientation) % 6];

                if(searchedCell.occupyingPart == null)
                {
                    //sets socket's rotation to the inverse of the search's orientation
                    Part socketInstance = socket;
                    socketInstance.rotation = (i + 3) % 6;

                    searchedCell.occupyingPart = socketInstance;
                    socketPositions.Add(searchedCell.position);
                } else
                {
                    Debug.Log("Adjacent Cell Status = " + searchedCell.occupyingPart.prefab);
                }  
            }
        }         
    }

    List<Cell> GetAdj(Vector3Int c)
    {
        Vector3Int[] adjCellTransforms = new Vector3Int[6]
        {
            new Vector3Int(0, -1, 1), new Vector3Int(-1, 0, 1), new Vector3Int(-1, 1, 0),
            new Vector3Int(0, 1, -1), new Vector3Int(1, 0, -1), new Vector3Int(1, -1, 0)
        };

        List<Cell> adjacentCells = new List<Cell>();

        foreach (Vector3Int transform in adjCellTransforms)
        {
            if (cellDict.TryGetValue(c + transform, out Cell cell))
            {
                adjacentCells.Add(cell);
            } else
            {
                Cell newCell = new Cell { position = (c + transform), occupyingPart = blocked };
                cellDict.Add(c + transform, newCell);
                adjacentCells.Add(newCell);
            }
        }

        return adjacentCells;
    }
}