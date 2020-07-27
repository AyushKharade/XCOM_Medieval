using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayLoop : MonoBehaviour
{
    bool gameInitialized;
    bool controlLock;          // No inputs accepted (show action cams & animations)

    public List<GameObject> PlayerUnitList = new List<GameObject>();                    // since no of units in a player's team stays constant 
    List<Character> PlayerUnit_ScriptRef = new List<Character>();
    int alivePlayerUnits;

    public int curUnitSelectedIndex;
                                                                                        //(except temporary mind controlled units if that is implemented.) 
    int turnCount;                                  // just a counter if needed
    [Range(1,2)]public int currentTurn;             // team 1 or team 2

    [Header("References")]
    public Transform NodesParentRef;
    public Transform PathDrawingRef;
    public Transform PlayerPod_Spawn;
    public Transform GameplayCamera;
    MainCameraScript cameraRef;
    public Transform PathfindingParent;
    GridPathfinding PathfinderRef;

    void Start()
    {
        cameraRef = GameplayCamera.GetComponent<MainCameraScript>();
        PathfinderRef = PathfindingParent.GetComponent<GridPathfinding>();

        alivePlayerUnits = PlayerUnitList.Count;
        currentTurn = 1;
        if (PlayerUnitList.Count > 0)
            Invoke("InitializeGame",0.3f);
        else
            Debug.Log("No Player Units !!! Cannot Start.");
    }

    void Update()
    {

        if (currentTurn == 1 && gameInitialized && !controlLock)
        {
            PlayerInputs();
        }
    }

    #region Player inputs
    void PlayerInputs()
    {
        // change controls
        if (Input.GetKeyDown(KeyCode.Tab))
            CycleCharacter(1);                     // +1 = increment indices
        else if (Input.GetKeyDown(KeyCode.LeftShift))
            CycleCharacter(-1);                     // -1 = decrement indices

        // Movement loop.
        UnitMovement();
    }

    void CycleCharacter(int direction)
    {
        if (alivePlayerUnits > 1)
        {
            // get next index that has an alive unit, if same as current unit, dont do anything.
            // current only cycle on the right side
            int index = curUnitSelectedIndex;
            while (true)
            {
                index = (index + 1) % PlayerUnitList.Count;
                if (index != curUnitSelectedIndex && !PlayerUnitList[index].transform.GetChild(0).GetComponent<Character>().isDead
                    && PlayerUnitList[index].transform.GetChild(0).GetComponent<Character>().HasActionsLeft())
                {
                    //PlayerUnitList[curUnitSelectedIndex].transform.GetChild(0).GetComponent<Character>().ToggleControlUI();
                    PlayerUnit_ScriptRef[curUnitSelectedIndex].ToggleControlUI();
                    curUnitSelectedIndex = index;
                    break;
                }
                else if (index == curUnitSelectedIndex)
                {
                    Debug.Log("Error in Cycling. Came back to same index.");
                    break;
                }
            }
        }

        // do the actual cycling.
        // UpdateActionMenu
        cameraRef.SetTarget(PlayerUnitList[curUnitSelectedIndex].transform, true);
        PlayerUnit_ScriptRef[curUnitSelectedIndex].ToggleControlUI();

        // reset start nodes so pathfinding is reset
        movementStartNode = PlayerUnit_ScriptRef[curUnitSelectedIndex].currentNode;
        movementTargetNode = null;

    }

    /*
    int GetNextAliveUnit(int direction)
    {
        int index = curUnitSelectedIndex + direction;       // from here check n-1 times for the next index.
    }
    */

    int Modulus(int i, int j) // i%j
    {
        if (i > 0) return i % j;
        else return j + ((i*-1)%j);
    }
    #endregion


    #region Movement

    GameObject movementStartNode;
    GameObject movementTargetNode;
    void UnitMovement()
    {
        // check if current selected unit has actions left. If yes, enable raycast movement.
        if (PlayerUnit_ScriptRef[curUnitSelectedIndex].HasActionsLeft() && !controlLock)
        {
            movementStartNode = PlayerUnit_ScriptRef[curUnitSelectedIndex].currentNode;
            RaycastMovement();
            if (validTarget && Input.GetMouseButtonDown(1))
            {
                TravelToTarget();
            }
        }
    }

    /// <summary>
    /// If where player is aiming with mouse is a valid target where the current unit can move.
    /// </summary>
    bool validTarget;
    void RaycastMovement()
    {
        RaycastHit hit;
        Ray ray = cameraRef.GetComponent<Camera>().ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit))
        {
            if (hit.collider.CompareTag("GridNode"))
            {
                if (hit.collider.GetComponent<GridNode>().IsNodeOpen())
                {
                    // see if this node is in range, if yes, enable undercursor and display path.
                    if (IsNodeReachable_MobilityPath(false, hit.collider.transform) > 0)
                    {

                        hit.collider.GetComponent<GridNode>().underCursor = true;
                        if (movementTargetNode != hit.collider.gameObject)              // update path and target node
                        {
                            movementTargetNode = hit.collider.gameObject;
                            GetPathToTarget();
                        }
                        else
                        {
                            // nothing ,still aiming at the same node. Make sure to reset while cycling.
                        }
                        validTarget = true;
                    }
                    else
                    {
                        PathDrawingRef.GetComponent<LineRenderer>().positionCount = 0;
                        validTarget = false;
                    }
                }
                else
                {
                    PathDrawingRef.GetComponent<LineRenderer>().positionCount = 0;
                    validTarget = false;
                }
            }
        }
    }

    List<Transform> path = new List<Transform>();
    void GetPathToTarget()
    {
        path= PathfinderRef.StartPathfindingRegular(movementStartNode,movementTargetNode);
        //Debug.Log("Path to target has " + path.Count + " nodes.");
        DrawPathOnScreen(path);
    }

    bool pathDrawn;
    void DrawPathOnScreen(List<Transform> pathList)
    {
        LineRenderer lineRendererRef = PathDrawingRef.GetComponent<LineRenderer>();

        if (pathDrawn)
        { //clear old path
            lineRendererRef.positionCount = 0;
        }

        lineRendererRef.positionCount = pathList.Count;
        for (int i = 0; i < pathList.Count; i++)
        {
            lineRendererRef.SetPosition(i, pathList[i].position);
        }
        pathDrawn = true;
    }

    void ResetDrawnPath() { PathDrawingRef.GetComponent<LineRenderer>().positionCount = 0; }

    void TravelToTarget()
    {
        validTarget = false;
        controlLock = true;
        //cameraRef.GetComponent<MainCameraScript>().freeCam = false;
        cameraRef.GetComponent<MainCameraScript>().ResetCamOnTarget();

        // set character's destination so they will move.
        PlayerUnit_ScriptRef[curUnitSelectedIndex].MoveUnitToNewLocation(movementTargetNode,path);
    }

    public void UnitReachedDestination()
    {
        controlLock = false;
        ResetDrawnPath();

    }


    /// <summary>
    /// A temporary function to see what path is taken by the mobility path finder function. (ignores all closed nodes)
    /// </summary>
    public Transform mobilityPathDrawerRef;
    int IsNodeReachable_MobilityPath(bool display, Transform endNode)
    {
        if (Vector3.Distance(endNode.position, movementStartNode.transform.position) > NodesParentRef.GetChild(0).localScale.x * 15)
            return 0;

        List<Transform> mobilityPath = PathfinderRef.Pathfind_Walkable(movementStartNode,endNode.gameObject);
        // draw this
        LineRenderer mobilityLineRenderer= mobilityPathDrawerRef.GetComponent<LineRenderer>();

        if (display)      // for debug purposes.
        {
            mobilityLineRenderer.positionCount = 0;
            mobilityLineRenderer.positionCount = mobilityPath.Count;
            for (int i = 0; i < mobilityPath.Count; i++)
            {
                mobilityLineRenderer.SetPosition(i, mobilityPath[i].position);
            }
        }


        //Calculate if can go here based on obstacle nos and mobility.
        // obstacle rating, -1 for full obstacle, - 0.5 for half cover.
        float obstacleRatingf = 0f;
        
        foreach (Transform t in mobilityPath)
        {
            if (t.GetComponent<GridNode>().nodeState==GridNode.NodeStatus.Obstacle_Full)
                obstacleRatingf += 0.9f;
            else if (t.GetComponent<GridNode>().nodeState==GridNode.NodeStatus.Obstacle_Half)
                obstacleRatingf += 0.35f;
        }
        
        //Debug.Log("Mobility path method has been commented");

        int moveCost;

        int range = PlayerUnit_ScriptRef[curUnitSelectedIndex].GetMobility() - (int)(obstacleRatingf);

        if (range < mobilityPath.Count)
            moveCost=0;
        else if ((range / 2) < mobilityPath.Count)
            moveCost=2;
        else if (PlayerUnit_ScriptRef[curUnitSelectedIndex].availableActions == 2)
            moveCost=1;
        else
            moveCost = 0;

        endNode.GetComponent<GridNode>().UpdateNodeUI(moveCost);

        return moveCost;

    }

    #endregion
    /// <summary>
    /// Init game by placing characters in pod locations.
    /// </summary>
    void InitializeGame()
    {
        foreach (GameObject gb in PlayerUnitList)
        {
            PlayerUnit_ScriptRef.Add(gb.transform.GetChild(0).GetComponent<Character>());
            gb.transform.GetChild(0).GetComponent<Character>().GameDirectorRef = this.gameObject;
        }

        
        // Spawn Players near the player pod. Toggle pods to occupied
        List<GameObject> arr = FindPlayerPodNodes(PlayerUnitList.Count);

        int index = 0;
        // now spawn players in these nodes.
        foreach (GameObject gb in PlayerUnitList)
        {
            gb.transform.position = arr[index].transform.position;
            gb.transform.GetChild(0).GetComponent<Character>().currentNode = arr[index];
            arr[index].GetComponent<GridNode>().ToggleNodeOccupied();
            index++;
        }

        GameplayCamera.GetComponent<MainCameraScript>().SetTarget(PlayerUnitList[0].transform, true);
        PlayerUnit_ScriptRef[0].ToggleControlUI();
        curUnitSelectedIndex = 0;

        gameInitialized = true;

    }

    List<GameObject> FindPlayerPodNodes(int noOfPlayers)
    {
        List<GameObject> emptySpawnNodes = new List<GameObject>();
        Collider[] arr = Physics.OverlapSphere(PlayerPod_Spawn.position, 4f);
        int count = 0;
        Debug.Log("Objects found at player pod"+arr.Length);
        foreach (Collider c in arr)
        {
            if (count > noOfPlayers)
                break;
            if (c.CompareTag("GridNode") && c.GetComponent<GridNode>().IsNodeOpen())
            {
                emptySpawnNodes.Add(c.gameObject);
                count++;
            }
        }
        return emptySpawnNodes;

    }

    public void AdvanceTurn() { }
}
