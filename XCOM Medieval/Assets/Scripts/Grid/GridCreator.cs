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

    public GameObject NodeOBJ;
    public Transform NodeParent;

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

        //
        CreateGridStructure();
        InitializeNodeNeighbors();
    }

    /// <summary>
    /// Creates a grid of nodes according to specified size x size
    /// </summary>
    void CreateGridStructure()
    {
        float nodeSize = NodeOBJ.transform.localScale.x;
        float nodeY_Pos = NodeOBJ.transform.position.y;
        Vector3 position;

        //instantiate central node.
        Instantiate(NodeOBJ, NodeParent);
        // central X-Nodes
        for (int i = 1; i <= verticalSize_X / 2; i++)
        {
            position = new Vector3(i * nodeSize, nodeY_Pos, 0f);
            Instantiate(NodeOBJ, position, Quaternion.identity, NodeParent);

            position = new Vector3(i * nodeSize * -1, nodeY_Pos, 0f);
            Instantiate(NodeOBJ, position, Quaternion.identity, NodeParent);
        }


        // instantiate nodes on the Z. [Horizontal]
        // Positive Side. (+ve on Z)
        for (int i = 1; i <= horizontalSize_Z / 2; i++)
        {
            position = new Vector3(0f, nodeY_Pos, i * nodeSize);
            Instantiate(NodeOBJ, position, Quaternion.identity, NodeParent);

            // positive Y-nodes
            for (int j = 1; j <= verticalSize_X / 2; j++)
            {
                position = new Vector3(j * nodeSize, nodeY_Pos, i*nodeSize);
                Instantiate(NodeOBJ, position, Quaternion.identity, NodeParent);

                position = new Vector3(j * nodeSize*-1, nodeY_Pos, i * nodeSize);
                Instantiate(NodeOBJ, position, Quaternion.identity, NodeParent);
            }

        }

        // negative side
        for (int i = 1; i <= horizontalSize_Z / 2; i++)
        {
            position = new Vector3(0f, nodeY_Pos, i * nodeSize*-1);
            Instantiate(NodeOBJ, position, Quaternion.identity, NodeParent);

            // negative Y-nodes
            
            for (int j = 1; j <= verticalSize_X / 2; j++)
            {
                position = new Vector3(j * nodeSize, nodeY_Pos, i * nodeSize*-1);
                Instantiate(NodeOBJ, position, Quaternion.identity, NodeParent);

                position = new Vector3(j * nodeSize * -1, nodeY_Pos, i * nodeSize*-1);
                Instantiate(NodeOBJ, position, Quaternion.identity, NodeParent);
            }
            

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
            //break;
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
                //c.transform.Translate(Vector3.up * 2.5f);
            }
        }

        Debug.Log("Central node has "+neighbors.Count+" neighbors.");
    }

}
