using UnityEngine;

public class PizzaSlice : CuttableBase
{
    [Header("Pizza Slice Parameters")]
    [SerializeField] private IngredientType currentIngredient = IngredientType.None;
    [SerializeField] private SpriteRenderer spriteRenderer;

    public IngredientType CurrentIngredient { get { return currentIngredient; } }

    public void UpdateCutIngredient(IngredientScriptable ingredient)
    {
        currentIngredient = ingredient.type;
        spriteRenderer.sprite = ingredient.sprite;
    }
}
