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
                DrawPath();
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

    List<Transform> StartPathfindingRegular(GameObject start, GameObject end)
    {
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
            {
                Debug.Log("Found end node.");
                break;
            }

            // if not final node, traverse all neighbours
            foreach (GridNode g in curNode.GetComponent<GridNode>().neighbours)
            {
                GameObject nb = g.gameObject;
                if (!ClosedSet.Contains(nb))        // skip closed list neighbours
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

    void DrawPath()
    {
        for (int i = 0; i < PathList.Count - 1; i++)
        {
            Debug.DrawLine(PathList[i].position,PathList[i+1].position,Color.red,15f);
        }
    }
    //###############################################################################
    #endregion
}
