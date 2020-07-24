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

    public enum Team { Player1, Player2, Enemy};           // player 2 incase i make a 1v1 mode.        
    [Header("Teams")]
    public Team unitTeam = new Team();

    [Header("Gameplay Data")]
    public int availableActions;

    [Header("UI References")]
    public Transform UI_Parent;
    public Image Health_FG_UI;
    public Text Health_UI;

    public Image currentControlUI;
    public Text actionsUI;
    public Text callSignUI;

    Vector3 camDirRef;

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
    }

    private void Update()
    {
        // billboard the UI towards Camera
        UI_Parent.LookAt(camDirRef*-1);
        
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

    // dynamic data & static profile data reader methods
    public int GetUnitHealth() { return health; }


    #region Common Methods to all Units:
    // Actionsc common to all units such as move, overwatch and cover.
    public void MoveUnit()
    {

    }
    #endregion
}
