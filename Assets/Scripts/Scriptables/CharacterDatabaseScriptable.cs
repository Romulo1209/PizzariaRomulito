using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "CharacterDatabase", menuName = "Pizza/CharacterDatabase")]
public class CharacterDatabaseScriptable : ScriptableObject
{
    public List<CharacterScriptable> characters;
}
