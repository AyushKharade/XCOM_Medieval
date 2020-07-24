using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// used to store statistics about each units.
/// </summary>
/// 
[CreateAssetMenu(fileName = "Create Unit", menuName = "Characters")]
public class CharacterInfo : ScriptableObject
{
    public enum ClassType { Combatant, Shieldbearer, Archer, Mage};
    /* Combatants: Rangers --> melee units using swords.
     * Shieldbearer --> hunker up provides cover to other units, high ranged resistant. medium melee resistance, low magic resistance
     * Archers --> ranged units, no melee resistance, no magic resistance, high dodge: grazed status
     * Mage --> support / psyops kind of unit. Provides healing and offensive capabilties.
     */

    [Header("Unit Details")]
    public ClassType UnitClass = new ClassType();
    public int health;
    [Range(0, 1f)] public float aim;
    [Range(0, 1f)] public float will;
    public string characterName;
    public string callSign;

    [Header("Resistances")]
    [Range(0, 1f)] public float meleeResistance;
    [Range(0, 1f)] public float dodge;
    
}
