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


    public float nodeSize;

    void Start()
    {
        nodeSize = transform.localScale.x;
        // find and save neighbours
    }

    
}
