using UnityEngine;

public class IngredientsController : MonoBehaviour
{
    [SerializeField] private GameController gameController;

    [SerializeField] private IngredientType ingredientType;

    [SerializeField] private IngredientsDatabaseScriptable ingredientsDb;

    public IngredientsDatabaseScriptable IngredientsDb { get { return ingredientsDb; } }

    void OnValidate()
    {
        gameController = GetComponent<GameController>();
    }

    void OnEnable()
    {
        CutterUi.OnClicked += HandleClick;
    }

    void OnDisable()
    {
        CutterUi.OnClicked -= HandleClick;
    }

    public void SelectIngredient(int ingredientIndex)
    {
        if(ingredientType == (IngredientType)ingredientIndex)
        {
            ingredientType = IngredientType.None; 
            return;
        }
        ingredientType = (IngredientType)ingredientIndex;
    }

    public void HandleClick(GameObject hitObject)
    {
        if(hitObject == null)
            return;

        if(gameController.ToolType == ToolType.Hand)
        {
            PizzaSlice slice = hitObject.GetComponent<PizzaSlice>();
            if(slice != null)
            {
                slice.UpdateCutIngredient(ingredientsDb.Get(ingredientType));
            }
        }
    }
}
