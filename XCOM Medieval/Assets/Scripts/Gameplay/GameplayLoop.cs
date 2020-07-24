using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayLoop : MonoBehaviour
{


    public List<GameObject> PlayerUnitList = new List<GameObject>();                    // since no of units in a player's team stays constant 
                                                                                        //(except temporary mind controlled units if that is implemented.) 
    int turnCount;                                  // just a counter if needed
    [Range(1,2)]public int currentTurn;             // team 1 or team 2

    [Header("References")]
    public Transform NodesParentRef;
    public Transform PlayerPod_Spawn;
    public Transform GameplayCamera;

    void Start()
    {
        if (PlayerUnitList.Count > 0)
            Invoke("InitializeGame",0.3f);
        else
            Debug.Log("No Player Units !!! Cannot Start.");
    }

    void Update()
    {
        
    }

    void InitializeGame()
    {
        // Spawn Players near the player pod. Toggle pods to occupied
        List<GameObject> arr = FindPlayerPodNodes(PlayerUnitList.Count);

        int index = 0;
        // now spawn players in these nodes.
        foreach (GameObject gb in PlayerUnitList)
        {
            gb.transform.position = arr[index].transform.position;
            arr[index].GetComponent<GridNode>().ToggleNodeOccupied();
            //arr.RemoveAt(index);
            index++;
        }

        GameplayCamera.GetComponent<MainCameraScript>().SetTarget(PlayerUnitList[0].transform, true);
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
