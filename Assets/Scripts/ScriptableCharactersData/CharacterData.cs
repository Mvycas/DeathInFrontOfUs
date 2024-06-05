using UnityEngine;

namespace ScriptableCharactersData
{
    [CreateAssetMenu(fileName = "NewCharacterData", menuName = "Character Data", order = 51)]
    public class CharacterData : ScriptableObject
    {
        public float maxHealth = 100;
    }
}
