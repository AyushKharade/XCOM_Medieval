using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// class to represent an individual node in the grid based world for this game. Saves neighbours, states (blocked / free), cover states, etc.
/// </summary>
public class GridNode : MonoBehaviour
{
    public enum NodeStatus { Open, Closed, Obstacle_Full, Obstacle_Half};            // all neighbours of a obstacle node (on object gives cover to all neighboring nodes). .
                                                                 // closed node means, there is an unit standing on it.
    public enum NodeCover { Full, Half, None};         // 40%, 20%, 0% defense bonuses respectively.
    [Header("Node Information")]
    public NodeStatus nodeState = new NodeStatus();
    public NodeCover nodeCover = new NodeCover();
    [HideInInspector]public bool alwaysVisible;

    // list of neighbours: each node has 8 neighbours: 4 perpendicular, 4 diagonal
    [Header("Node Neighbours")]
    public List<GridNode> neighbours = new List<GridNode>();

    [Header("State Colors")]
    public Material defaultColor;
    public Material occupiedColor;
    public Material halfCoverColor;
    public Material closedColor;

    [Header("Pathfinding Variables")]
    public float gCost;
    public float hCost;
    public float fCost;
    public GameObject Parent;

    public float nodeSize;

    [Header("Node Info UI")]
    public Text moveCostUI;
    public Text coverInfoUI;
    public GameObject nodeUI_Parent;

    // for resetting scale
    float ogScaleXZ;
    float ogScaleY;
    [HideInInspector] public bool underCursor;

    void Start()
    {

        nodeSize = transform.localScale.x;

        ogScaleXZ = nodeSize;
        ogScaleY = transform.localScale.y;

        // find and save neighbours
    }

    private void Update()
    {
        if (underCursor)
            SelectionScale();
        else
            ResetScale();
        underCursor = false;
    }

    /// <summary>
    /// Sees if give node is adjacent to current node (for restricting diagonal things like movement in certain situations.
    /// </summary>
    /// <param name="node">The node to test</param>
    /// <returns>true if node is adjacent, false otherwise.</returns>
    public bool IsNodeAdjacent(Transform node)
    {
        float angle = Vector3.Angle(transform.forward, (node.position - transform.position));
        if (angle == 0 || angle == 90 || angle == 180 || angle == 270)
            return true;
        else
            return false;
    }

    #region Open/Close Nodes
    void CloseNode()
    {
        nodeState = NodeStatus.Closed;
        GetComponent<MeshRenderer>().material=closedColor;
    }

    /// <summary>
    /// Someone is standing on it.
    /// </summary>
    void OccupyNode()
    {
        nodeState = NodeStatus.Closed;
    }

    void OpenNode()
    {
        nodeState = NodeStatus.Open;
        GetComponent<MeshRenderer>().material = defaultColor;
    }


   

    // Flips node status between Open and closed.
    public void ToggleNode()
    {
        if (nodeState == NodeStatus.Open)
            CloseNode();
        else
            OpenNode();
    }

    /// <summary>
    /// Sets that this node is closed with someone standing on it.
    /// </summary>
    public void ToggleNodeOccupied()
    {
        OccupyNode();
        //GetComponent<MeshRenderer>().material = occupiedColor;
    }

    public bool IsNodeOpen() { if (nodeState == NodeStatus.Open) return true; else return false; }
    #endregion

    #region Obstacle adding & removing.

    public void AddObstacleToNode(NodeStatus status)
    {
        nodeState = status;
        // Assign this cover to all adjacent neighboring nodes.
        foreach (GridNode gb in neighbours)
        {
            if (gb.nodeCover!=NodeCover.Full && gb.IsNodeOpen()                                        // only overwrite cover if its half
                && transform.GetComponent<GridNode>().IsNodeAdjacent(gb.gameObject.transform))        // only set cover to adjacent tiles
            {
                if (status == NodeStatus.Obstacle_Full)
                    gb.AddCoverToNode(NodeCover.Full);
                else
                    gb.AddCoverToNode(NodeCover.Half);
            }
        }
    }


    public void AddCoverToNode(NodeCover cover)
    {
        nodeCover = cover;
    }

    public void RemoveCover()
    {
        nodeCover = NodeCover.None;
    }

    /// <summary>
    /// If it was full cover, not its half, if its half, its now none. Cover can be destroyed by certain things (if thats a feature)
    /// </summary>
    public void CollapseCover()
    {
        if (nodeCover == NodeCover.Full)
            nodeCover = NodeCover.Half;
        else
            nodeCover = NodeCover.None;
    }


    #endregion





    public void InitFCost(Transform start, Transform end)
    {
        gCost = Vector3.Distance(start.position, transform.position);
        hCost = Vector3.Distance(end.position, transform.position);
        fCost = gCost + fCost;
    }

    public float GetFCost()
    { return fCost; }

    void ResetScale()
    {
        if (alwaysVisible)
        {
            if(transform.localScale.x!=ogScaleXZ)
                transform.localScale = new Vector3(ogScaleXZ, ogScaleY, ogScaleXZ);
        }
        else
            GetComponent<MeshRenderer>().enabled = false;
        nodeUI_Parent.SetActive(false);
    }

    public void SelectionScale()
    {
        if(alwaysVisible)
            transform.localScale = new Vector3(ogScaleXZ + ogScaleXZ * 0.25f, ogScaleY + ogScaleY * 0.25f, ogScaleXZ + ogScaleXZ * 0.25f);
        else
            GetComponent<MeshRenderer>().enabled = true;

        nodeUI_Parent.SetActive(true);
    }



    public void UpdateNodeUI(int cost)
    {
        moveCostUI.enabled = true;
        coverInfoUI.enabled = true;

        if (cost == 0)
        {
            moveCostUI.text = "Unreachable.";
            coverInfoUI.enabled = false;
        }
        else
        {
            moveCostUI.text = "Cost: "+cost;
            coverInfoUI.text = "Cover: " + nodeCover;
        }
    }
}
