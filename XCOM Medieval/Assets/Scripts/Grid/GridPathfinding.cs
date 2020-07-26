using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Script used for pathfinding. Default use is to get a path to the destination. 
/// </summary>
public class GridPathfinding : MonoBehaviour
{
    [Header("Pathfinding Variables.")]
    public GameObject StartNode;
    public GameObject EndNode;

    public Transform AllNodesParent;

    public List<GameObject> OpenSet = new List<GameObject>();
    public HashSet<GameObject> ClosedSet = new HashSet<GameObject>();

    LinkedList<Transform> Path = new LinkedList<Transform>();

    [Header("Path")]
    public List<Transform> PathList = new List<Transform>();

    bool foundPath;
    void Start()
    {
        
    }

    void Update()
    {
        if (!foundPath && Input.GetKeyDown(KeyCode.Space))
        {
            if (StartNode != null && EndNode != null)
            {
                foundPath = true;
                PathList= StartPathfindingRegular(StartNode,EndNode);
                DrawPath(50);
            }
            else
                Debug.Log("Starting node or ending node not defined!!");
        }
    }

    #region Pathfinding Functions
    void InitStartCosts()
    {
        int count = AllNodesParent.transform.childCount;

        for (int i = 0; i < count; i++)
        {
            AllNodesParent.transform.GetChild(i).GetComponent<GridNode>().InitFCost(StartNode.transform, EndNode.transform);
            AllNodesParent.transform.GetChild(i).GetComponent<GridNode>().Parent = null;
        }
    }

    //#################### Path finding function ########################################
    // Pathfinding Function:
    // Returns a list of path transforms from Start node to end node

    public List<Transform> StartPathfindingRegular(GameObject start, GameObject end)
    {
        StartNode = start;
        EndNode = end;

        List<Transform> pathList = new List<Transform>();

        OpenSet.Clear();
        ClosedSet.Clear();
        InitStartCosts();
        OpenSet.Add(start);

        while (OpenSet.Count > 0)
        {
            GameObject curNode = OpenSet[0];
            // check for any other nodes having smaller cost that this
            foreach (GameObject g in OpenSet)
            {
                
                if (g.GetComponent<GridNode>().GetFCost() < curNode.GetComponent<GridNode>().GetFCost() && g.transform.name != curNode.transform.name
                    &&
                    (g.GetComponent<GridNode>().hCost < curNode.GetComponent<GridNode>().hCost))
                {
                    curNode = g;
                }
            }

            OpenSet.Remove(curNode);
            ClosedSet.Add(curNode);
            //if (curNode.transform.name != StartNode.transform.name && curNode.transform.name != EndNode.transform.name)
            //    curNode.GetComponent<MeshRenderer>().material = YellowMat;

            //check if it is the target node
            if (curNode.transform.name == end.transform.name)
                break;

            // if not final node, traverse all neighbours
            foreach (GridNode g in curNode.GetComponent<GridNode>().neighbours)
            {
                GameObject nb = g.gameObject;
                if (!ClosedSet.Contains(nb) && g.IsNodeOpen())        // skip closed list neighbours && not-traversable nodes
                {
                    // find cost of moving to neighbour
                    float moveCost = curNode.GetComponent<GridNode>().gCost + Vector3.Distance(curNode.transform.position, nb.transform.position);

                    //if (moveCost < nb.GetComponent<Node>().GetFCost() || !OpenSet.Contains(nb))
                    if (moveCost < nb.GetComponent<GridNode>().gCost || !OpenSet.Contains(nb))
                    {
                        nb.GetComponent<GridNode>().gCost = moveCost;
                        nb.GetComponent<GridNode>().hCost = Vector3.Distance(nb.transform.position, EndNode.transform.position);
                        nb.GetComponent<GridNode>().Parent = curNode;

                        if (!OpenSet.Contains(nb))
                        {
                            OpenSet.Add(nb);
                           // if (nb.transform.name != end.transform.name && nb.GetComponent<MeshRenderer>().material != YellowMat)
                           //     nb.GetComponent<MeshRenderer>().material = BlueMat;
                        }
                    }
                }
            }
        }

        // create path by back tracking from end node to start node.
        // add the path in list
        GameObject currNode = end;

        pathList.Add(end.transform);
        while (currNode.GetComponent<GridNode>().Parent != null)
        {
            pathList.Add(currNode.GetComponent<GridNode>().Parent.transform);
            currNode = currNode.GetComponent<GridNode>().Parent;
        }

        pathList.Reverse();

        return pathList;

    }

    void DrawPath(float time)
    {
        for (int i = 0; i < PathList.Count - 1; i++)
        {
            Debug.DrawLine(PathList[i].position,PathList[i+1].position,Color.red,time);
        }
    }

    /// <summary>
    /// This method pathfinds to target ignoring all obstacle and depending upon character's mobility decides whether the unit can move here
    /// whether it costs 1 action or 2 actions.
    /// </summary>
    /// <param name="start">Starting Node Position</param>
    /// <param name="end">Ending Node Position</param>
    /// <param name="mobility">The mobility stat of the character</param>
    /// <param name="actionsLeft">No. of actions the character has left.</param>
    /// <returns>Returns int: 0 = not reacheable, 1 = costs 1 action, 2=costs 2 action (Dash).</returns>
    public int Pathfind_Walkable(GameObject start, GameObject end, int mobility)
    {
        StartNode = start;
        EndNode = end;

        List<Transform> pathList = new List<Transform>();

        OpenSet.Clear();
        ClosedSet.Clear();
        InitStartCosts();
        OpenSet.Add(start);

        while (OpenSet.Count > 0)
        {
            GameObject curNode = OpenSet[0];
            // check for any other nodes having smaller cost that this
            foreach (GameObject g in OpenSet)
            {

                if (g.GetComponent<GridNode>().GetFCost() < curNode.GetComponent<GridNode>().GetFCost() && g.transform.name != curNode.transform.name
                    &&
                    (g.GetComponent<GridNode>().hCost < curNode.GetComponent<GridNode>().hCost))
                {
                    curNode = g;
                }
            }

            OpenSet.Remove(curNode);
            ClosedSet.Add(curNode);
            
            //check if it is the target node
            if (curNode.transform.name == end.transform.name)
                break;

            // if not final node, traverse all neighbours
            foreach (GridNode g in curNode.GetComponent<GridNode>().neighbours)
            {
                GameObject nb = g.gameObject;
                if (!ClosedSet.Contains(nb))
                {
                    // find cost of moving to neighbour
                    float moveCost = curNode.GetComponent<GridNode>().gCost + Vector3.Distance(curNode.transform.position, nb.transform.position);

                    if (moveCost < nb.GetComponent<GridNode>().gCost || !OpenSet.Contains(nb))
                    {
                        nb.GetComponent<GridNode>().gCost = moveCost;
                        nb.GetComponent<GridNode>().hCost = Vector3.Distance(nb.transform.position, EndNode.transform.position);
                        nb.GetComponent<GridNode>().Parent = curNode;

                        if (!OpenSet.Contains(nb))
                        {
                            OpenSet.Add(nb);
                        }
                    }
                }
            }
        }

        // create path by back tracking from end node to start node.
        // add the path in list
        GameObject currNode = end;

        pathList.Add(end.transform);
        while (currNode.GetComponent<GridNode>().Parent != null)
        {
            pathList.Add(currNode.GetComponent<GridNode>().Parent.transform);
            currNode = currNode.GetComponent<GridNode>().Parent;
        }

        pathList.Reverse();

        // ############################## Calculate if you can move here. ##########################

        if (pathList.Count == 0)
            return 0;               // unreachable.

        // To calculate if you can make it there: use the no. of tiles you are using to go there, and the no. of obstacles you encountered in your path.
        int obstacles = 0;
        foreach (Transform T in pathList)
        {
            if (!T.GetComponent<GridNode>().IsNodeOpen())
                obstacles++;
        }
        obstacles /= 2;

        Debug.Log("Path list count is: "+pathList.Count);

        // mobility / 2 = 1 action, beyond this, costs 2 actions.
        if (pathList.Count > mobility - obstacles)                   // unreachable.
            return 0;
        else if (pathList.Count > (mobility - obstacles) / 2)
            return 1;
        else
            return 2;
    }

    //###############################################################################
    #endregion
}
