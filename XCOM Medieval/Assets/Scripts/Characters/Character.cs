using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    public enum Team { Player1, Player2, Enemy};           // player 2 incase i make a 1v1 mode.        
    [Header("Teams")]
    public Team unitTeam = new Team();

    [Header("Gameplay Data")]
    public int availableActions;

    private void Start()
    {
        health = characterProfile.health;
    }

    // methods
    public void DeductActions(int n) { availableActions -= n;if (availableActions < 0) availableActions = 0; }
    public void ResetActions() { availableActions = 2; }
    public GameObject GetCurUnitNode() { return currentNode; }

    // dynamic data & static profile data reader methods
    public int GetUnitHealth() { return health; }

}
