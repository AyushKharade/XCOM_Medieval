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

    //covers
    public NodeCover nodeCoverFront = new NodeCover();         // down the z axis of the node
    public NodeCover nodeCoverBack = new NodeCover();          // -z
    public NodeCover nodeCoverLeft = new NodeCover();          // -x
    public NodeCover nodeCoverRight = new NodeCover();         // +x



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
    public Image Cover_Front_UI;
    public Image Cover_Back_UI;
    public Image Cover_Left_UI;
    public Image Cover_Right_UI;
    public Sprite FullCover_UI_Image;
    public Sprite HalfCover_UI_Image;
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
        if (angle == 0 || angle == 90 || angle == 180)
            return true;
        else
            return false;
    }

    public float AngleBetweenNodes(Transform node)
    {
        return Vector3.Angle(transform.forward, (node.position - transform.position));
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

    /// <summary>
    /// This method will assign cover status to neighboring adjacent nodes depending on where they are. Each node has 4 covers in 4 directions
    /// this function will assign the correct cover based on the angle & position.
    /// </summary>
    /// <param name="status"></param>
    public void AddObstacleToNode(NodeStatus status)
    {
        nodeState = status;
        NodeCover cover = new NodeCover();
        if (status == NodeStatus.Obstacle_Full)
            cover = NodeCover.Full;
        else
            cover = NodeCover.Half;

        // Assign this cover to all adjacent neighboring nodes.
        foreach (GridNode gb in neighbours)
        {
            // depending upon angle, assign cover
            if (GetComponent<GridNode>().IsNodeAdjacent(gb.transform))
            {
                // now we know its adjacent.
                if (AngleBetweenNodes(gb.transform) == 90)       // means either left or right
                {
                    if ((gb.transform.position.x - transform.position.x) > 0 && gb.nodeCoverLeft!=NodeCover.Full)
                        gb.nodeCoverLeft = cover;
                    else if(gb.nodeCoverRight!= NodeCover.Full)
                        gb.nodeCoverRight = cover;
                }
                else // == 0, its front or back.
                {
                    if ((gb.transform.position.z - transform.position.z) < 0 && gb.nodeCoverFront != NodeCover.Full)        // means its forward.
                        gb.nodeCoverFront = cover;
                    else if(gb.nodeCoverBack != NodeCover.Full)
                        gb.nodeCoverBack = cover;
                }

            }







            // old
            /*
            if (gb.nodeCover!=NodeCover.Full && gb.IsNodeOpen()                                        // only overwrite cover if its half
                && transform.GetComponent<GridNode>().IsNodeAdjacent(gb.gameObject.transform))        // only set cover to adjacent tiles
            {
                if (status == NodeStatus.Obstacle_Full)
                    gb.AddCoverToNode(NodeCover.Full);
                else
                    gb.AddCoverToNode(NodeCover.Half);
            }
            */
        }
    }

    

    /// <summary>
    /// If it was full cover, not its half, if its half, its now none. Cover can be destroyed by certain things (if thats a feature)
    /// </summary>
    public void CollapseCover()
    {
        
    }


    #endregion





    public void InitFCost(Transform start, Transform end)
    {
        gCost = Vector3.Distance(start.position, transform.position);
        hCost = Vector3.Distance(end.position, transform.position);
        fCost = gCost + hCost;
    }

    public float GetFCost()
    { return fCost; }


    #region Node selection highlight
    /// <summary>
    /// Removes highlight from a node when it is no longer being pointed at.
    /// </summary>
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

    /// <summary>
    /// Highlights currently selected node.
    /// </summary>
    public void SelectionScale()
    {
        if(alwaysVisible)
            transform.localScale = new Vector3(ogScaleXZ + ogScaleXZ * 0.25f, ogScaleY + ogScaleY * 0.25f, ogScaleXZ + ogScaleXZ * 0.25f);
        else
            GetComponent<MeshRenderer>().enabled = true;

        nodeUI_Parent.SetActive(true);
    }
    #endregion


    public void UpdateNodeUI(int cost)
    {
        moveCostUI.enabled = true;
        Cover_Front_UI.enabled = true;
        Cover_Back_UI.enabled = true;
        Cover_Left_UI.enabled = true;
        Cover_Right_UI.enabled = true;

        if (cost > 0)
        {
            moveCostUI.text = "Cost: "+cost;

            #region cover ui updates
            // Front
            if (nodeCoverFront == NodeCover.Full)
                Cover_Front_UI.sprite = FullCover_UI_Image;
            else if (nodeCoverFront == NodeCover.Half)
                Cover_Front_UI.sprite = HalfCover_UI_Image;
            else
                Cover_Front_UI.enabled = false;

            // back
            if (nodeCoverBack == NodeCover.Full)
                Cover_Back_UI.sprite = FullCover_UI_Image;
            else if (nodeCoverBack == NodeCover.Half)
                Cover_Back_UI.sprite = HalfCover_UI_Image;
            else
                Cover_Back_UI.enabled = false;

            // left
            if (nodeCoverLeft == NodeCover.Full)
                Cover_Left_UI.sprite = FullCover_UI_Image;
            else if (nodeCoverLeft == NodeCover.Half)
                Cover_Left_UI.sprite = HalfCover_UI_Image;
            else
                Cover_Left_UI.enabled = false;

            // right
            if (nodeCoverRight == NodeCover.Full)
                Cover_Right_UI.sprite = FullCover_UI_Image;
            else if (nodeCoverRight == NodeCover.Half)
                Cover_Right_UI.sprite = HalfCover_UI_Image;
            else
                Cover_Right_UI.enabled = false;
            #endregion

        }
    }
}
