using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character Data", order = 51)]
public class CharacterData : ScriptableObject
{
    public float maxHealth = 100;
    // add more character properties later on such as dmg... exp.. mana maybe??
}
