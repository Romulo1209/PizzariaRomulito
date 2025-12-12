using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Ingredient", menuName = "Pizza/Ingredient")]
public class IngredientScriptable : ScriptableObject
{
    public IngredientType type;
    public Sprite sprite;
}
