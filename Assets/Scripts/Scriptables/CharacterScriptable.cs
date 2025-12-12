using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Character", menuName = "Pizza/Character")]
public class CharacterScriptable : ScriptableObject
{
    public string characterName;
    public Sprite characterSpriteIdle;
    public Sprite characterSpriteHappy;
    public Sprite characterSpriteSad;
}
