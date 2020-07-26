using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// creates a grid structure before the start of the game. Responsible for initialzing a grid structure and playable map.
/// </summary>
public class GridCreator : MonoBehaviour
{

    [Header("Grid Parameters")]
    public int horizontalSize_Z=14;      // how many nodes down the z-axis ---> HAS TO BE ODD NUMBER.
    public int verticalSize_X=10;        // how many nodes down the x-axis

    public int highCovers;  // no of covers to spawn in the map.
    public int lowCovers;   

    public GameObject NodeOBJ;
    public Transform NodeParent;
    public Transform ObstacleParent;

    [Header("Obstacle Prefabs")]
    public GameObject Obscable_Full_Cover;
    public GameObject Obscable_Half_Cover;

    void Start()
    {
        #region Checking Conditions
        if ((horizontalSize_Z / 2) % 2 != 0)
            horizontalSize_Z--;
        if ((verticalSize_X / 2) % 2 != 0)
            verticalSize_X--;

        if (horizontalSize_Z < 2 || verticalSize_X < 2)
            Debug.Log("Grid sizes are incorrect must be atleast larger than 2.");
        #endregion

        CreateGridStructure();
        InitializeNodeNeighbors();
        SpawnCovers();
    }

  /// <summary>
  /// Creates a Grid structure with specified size. Instantiates node objects and names them so tracking is easier.
  /// </summary>
    void CreateGridStructure()
    {
        // use a single nested for loop.
        float nodeSize = NodeOBJ.transform.localScale.x;
        float nodeY_Pos = NodeOBJ.transform.position.y;

        float starting_Z_Pos = (nodeSize * horizontalSize_Z / 2) * -1;     // so that grid is centered.
        int nameCount = 0;

        GameObject gb;

        for (int i = 0; i < horizontalSize_Z; i++)
        {
            float starting_X_Pos = (nodeSize * verticalSize_X / 2) * -1;
            for (int j = 0; j < verticalSize_X; j++)
            {
                if (starting_X_Pos != 0f || true)
                {
                    gb = Instantiate(NodeOBJ, new Vector3(starting_X_Pos, nodeY_Pos, starting_Z_Pos), Quaternion.identity, NodeParent);
                    gb.name = "GridNode" + nameCount;
                    nameCount++;
                }
                starting_X_Pos += nodeSize;
            }
            starting_Z_Pos += nodeSize;
        }

    }



    /// <summary>
    /// for each node made in CreateGridStructure, set up its neighbors
    /// </summary>
    void InitializeNodeNeighbors()
    {
        for (int i = 0; i < NodeParent.childCount; i++)
        {
            FindNeighborNodes(NodeParent.GetChild(i).gameObject, NodeOBJ.transform.localScale.x);
        }
    }

    /// <summary>
    /// has the overlap function to do the neighbor assignment. Update function later to use layermask and sort orders
    /// </summary>
    void FindNeighborNodes(GameObject curNode, float nodeSize)
    {
        Collider []arr= Physics.OverlapBox(curNode.transform.position,new Vector3(nodeSize+0.5f,1f,nodeSize+0.5f),Quaternion.identity);
        List<GameObject> neighbors=new List<GameObject>();

        foreach (Collider c in arr)
        {
            if (c.transform.CompareTag("GridNode") && Vector3.Distance(c.transform.position,curNode.transform.position)>0f)
            {
                neighbors.Add(c.gameObject);
                curNode.GetComponent<GridNode>().neighbours.Add(c.GetComponent<GridNode>());
            }
        }
    }

    /// <summary>
    /// Closes n random nodes, so path finding can be tested.
    /// </summary>
    /// <param name="n">No. of nodes to close.</param>
    void CloseRandomNodes(int n)
    {
        for (int i = 0; i < n; i++)
        {
            int r = Random.Range(0, NodeParent.childCount);
            if (NodeParent.GetChild(r).GetComponent<GridNode>().IsNodeOpen())
                NodeParent.GetChild(r).GetComponent<GridNode>().ToggleNode();
            else
                i--;
        }
    }


    void SpawnCovers()
    {
        GameObject coverObj;
        // full covers
        for (int i = 0; i < highCovers; i++)
        {
            int r = Random.Range(0, NodeParent.childCount);
            if (NodeParent.GetChild(r).GetComponent<GridNode>().IsNodeOpen())
            {
                coverObj = Instantiate(Obscable_Full_Cover,NodeParent.GetChild(r).position,Quaternion.identity,ObstacleParent);
                NodeParent.GetChild(r).GetComponent<GridNode>().AddObstacleToNode(GridNode.NodeStatus.Obstacle_Full);
            }
            else
                i--;
        }

        //half covers
        for (int i = 0; i <lowCovers; i++)
        {
            int r = Random.Range(0, NodeParent.childCount);
            if (NodeParent.GetChild(r).GetComponent<GridNode>().IsNodeOpen())
            {
                coverObj = Instantiate(Obscable_Half_Cover, NodeParent.GetChild(r).position, Quaternion.identity, ObstacleParent);
                NodeParent.GetChild(r).GetComponent<GridNode>().AddObstacleToNode(GridNode.NodeStatus.Obstacle_Half);
            }
            else
                i--;
        }

    }
}
