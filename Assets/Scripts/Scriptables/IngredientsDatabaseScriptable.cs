using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IngredientDatabase", menuName = "Pizza/Ingredient Database")]
public class IngredientsDatabaseScriptable : ScriptableObject
{
    public List<IngredientScriptable> ingredients;

    private Dictionary<IngredientType, IngredientScriptable> _map;

    void OnEnable()
    {
        _map = new Dictionary<IngredientType, IngredientScriptable>();
        foreach (var item in ingredients)
            _map[item.type] = item;
    }

    public IngredientScriptable Get(IngredientType type)
    {
        return _map[type];
    }

    public IngredientType GetRandomIngredientType()
    {
        var values = (IngredientType[])System.Enum.GetValues(typeof(IngredientType));
        return values[Random.Range(1, values.Length)];
    }
}
