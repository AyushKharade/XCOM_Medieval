using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character : MonoBehaviour
{
    //Character Info reference
    public CharacterInfo characterProfile;
    //public sound profile

    // getter methods will simply return values from the profile.
    [Header("Dynamic Data")]              // any values that will temporarily change on the battlefield.
    public int health;
    int maxHealth;
    public GameObject currentNode;        // keep track of what nodes units are standing on.
    public bool isDead;
    public bool isMoving;

    public enum Team { Player1, Player2, Enemy};           // player 2 incase i make a 1v1 mode.        
    [Header("Teams")]
    public Team unitTeam = new Team();

    [Header("Gameplay Data")]
    public int availableActions;
    List<Transform> movementPath=new List<Transform>();

    // movement variables
    int movementPathIndex=1;
    List<Transform> pathToFollow = new List<Transform>();

    #region References
    [Header("UI References")]
    public Transform UI_Parent;
    public Image Health_FG_UI;
    public Text Health_UI;
    public Image currentControlUI;
    public Text actionsUI;
    public Text callSignUI;

    [Header("References")]
    public GameObject GameDirectorRef;
    Animator animator;

    Vector3 camDirRef;
    #endregion

    private void Start()
    {
        health = characterProfile.health;
        maxHealth = health;
        UpdateHP_UI();

        availableActions = 2;

        camDirRef = Camera.main.transform.position;
        currentControlUI.enabled = false;
        callSignUI.enabled = false;
        callSignUI.text = characterProfile.callSign + "";

        //references
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        // billboard the UI towards Camera
        UI_Parent.LookAt(camDirRef*-1);

        if (isMoving)
        {
            if (Vector3.Distance(transform.position, currentNode.transform.position) < 0.1f)
            {
                //reached
                transform.position = currentNode.transform.position;
                isMoving = false;
                animator.SetBool("isRunning", false);
                EndMovement();
            }
            else if (Vector3.Distance(transform.position, pathToFollow[movementPathIndex].position) < 0.1f)
            {
                // go to next position.
                transform.position = pathToFollow[movementPathIndex].position;
                movementPathIndex++;
            }
            else
            {
                Vector3 moveDir = (pathToFollow[movementPathIndex].position - transform.position).normalized;
                transform.parent.Translate(moveDir * characterProfile.moveSpeed * Time.deltaTime);
                OrientCharacter(moveDir);
            }
        }
        
    }

    void OrientCharacter(Vector3 dir)
    {
        Quaternion lookDirection;

        //set quaternion to this dir
        lookDirection = Quaternion.LookRotation(dir, Vector3.up);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, lookDirection, 5f);
    }

    public void ToggleControlUI()
    {
        if (currentControlUI.IsActive())
        {
            currentControlUI.enabled = false;
            callSignUI.enabled = false;
        }
        else
        {
            currentControlUI.enabled = true;
            callSignUI.enabled = true;
        }
    }

    public void UpdateHP_UI()
    {
        Health_FG_UI.fillAmount = (health * 1f / maxHealth * 1f);
        Health_UI.text = health + "/" + maxHealth;
    }
    // methods
    public void DeductActions(int n) { availableActions -= n;if (availableActions < 0) availableActions = 0; actionsUI.text = availableActions + ""; }
    public bool HasActionsLeft() { return availableActions > 0; }
    public void ResetActions() { availableActions = 2; actionsUI.text = 2+""; }
    public GameObject GetCurUnitNode() { return currentNode; }
    public int GetMobility() { return characterProfile.mobility; }

    // dynamic data & static profile data reader methods
    public int GetUnitHealth() { return health; }


    #region Common Methods to all Units:
    /// <summary>
    /// Will move an unit to new target location. Takes care of animations. Control lock will be on until unit reaches target.
    /// Once target is reached, updates nodes (original node is now open, target node is now closed)
    /// Calls function on director to let it know the action has been completed.
    /// </summary>
    /// <param name="endNode">The target node where the unit is going</param>
    /// <param name="path">path the unit will follow.</param>
    public void MoveUnitToNewLocation(GameObject endNode, List<Transform> path)
    {
        //currentNode.GetComponent<GridNode>().ToggleNodeOccupied();
        currentNode.GetComponent<GridNode>().ToggleNode();
        currentNode = endNode;

        // start running.
        animator.SetBool("isRunning", true);
        isMoving = true;
        pathToFollow = path;
        movementPathIndex = 1;
    }

    void EndMovement()
    {
        currentNode.GetComponent<GridNode>().ToggleNodeOccupied();
        // call function on director.
        GameDirectorRef.GetComponent<GameplayLoop>().UnitReachedDestination();
    }
    #endregion
}
