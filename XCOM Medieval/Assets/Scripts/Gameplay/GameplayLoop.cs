using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayLoop : MonoBehaviour
{


    public List<GameObject> PlayerUnitList = new List<GameObject>();                    // since no of units in a player's team stays constant 
    int alivePlayerUnits;

    public int curUnitSelectedIndex;
                                                                                        //(except temporary mind controlled units if that is implemented.) 
    int turnCount;                                  // just a counter if needed
    [Range(1,2)]public int currentTurn;             // team 1 or team 2

    [Header("References")]
    public Transform NodesParentRef;
    public Transform PlayerPod_Spawn;
    public Transform GameplayCamera;
    MainCameraScript cameraRef;

    void Start()
    {
        cameraRef = GameplayCamera.GetComponent<MainCameraScript>();

        alivePlayerUnits = PlayerUnitList.Count;
        currentTurn = 1;
        if (PlayerUnitList.Count > 0)
            Invoke("InitializeGame",0.3f);
        else
            Debug.Log("No Player Units !!! Cannot Start.");
    }

    void Update()
    {

        if (currentTurn == 1)
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
                if (index != curUnitSelectedIndex && !PlayerUnitList[index].transform.GetChild(0).GetComponent<Character>().isDead)
                {
                    curUnitSelectedIndex= index;
                    break;
                }
            }
        }

        // do the actual cycling.
        // UpdateActionMenu
        cameraRef.SetTarget(PlayerUnitList[curUnitSelectedIndex].transform, true);
        
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
        curUnitSelectedIndex = 0;

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
