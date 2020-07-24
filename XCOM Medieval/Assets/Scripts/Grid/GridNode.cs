using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// class to represent an individual node in the grid based world for this game. Saves neighbours, states (blocked / free), cover states, etc.
/// </summary>
public class GridNode : MonoBehaviour
{
    public enum NodeStatus { Open, Closed, Obstacle};            // all neighbours of a obstacle node (on object gives cover to all neighboring nodes). .
                                                                 // closed node means, there is an unit standing on it.
    public enum NodeCover { Full, Half, None};         // 40%, 20%, 0% defense bonuses respectively.
    [Header("Node Information")]
    public NodeStatus nodeState = new NodeStatus();
    public NodeCover nodeCover = new NodeCover();

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

    void Start()
    {

        nodeSize = transform.localScale.x;
        // find and save neighbours
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

    public void ToggleNode()
    {
        if (nodeState == NodeStatus.Open)
            CloseNode();
        else
            OpenNode();
    }

    public void ToggleNodeOccupied()
    {
        OccupyNode();
        GetComponent<MeshRenderer>().material = occupiedColor;
    }

    public bool IsNodeOpen() { if (nodeState == NodeStatus.Open) return true; else return false; }
    #endregion

    public void InitFCost(Transform start, Transform end)
    {
        gCost = Vector3.Distance(start.position, transform.position);
        hCost = Vector3.Distance(end.position, transform.position);
        fCost = gCost + fCost;
    }

    public float GetFCost()
    { return fCost; }
}
