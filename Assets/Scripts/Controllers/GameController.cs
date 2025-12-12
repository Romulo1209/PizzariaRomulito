using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ToolType { Hand = 0, Cutter = 1 }



public class GameController : MonoBehaviour
{
    [Header("Controllers")]
    [SerializeField] private CutterController cutterController;
    [SerializeField] private SlicesController slicesController;
    [SerializeField] private IngredientsController ingredientsController;
    [SerializeField] private DialogController dialogController;
    [SerializeField] private RoundController roundController;
    
    [Header("Game Settings")]
    [SerializeField] private ToolType toolType;
    [SerializeField] private bool onTutorialMode = true;

    public ToolType ToolType { get { return toolType; } }


    public bool OnTutorialMode { get { return onTutorialMode; } }
    public CutterController CutterController { get { return cutterController; } }
    public SlicesController SlicesController { get { return slicesController; } }
    public IngredientsController IngredientsController { get { return ingredientsController; } }
    public DialogController DialogController { get { return dialogController; } }
    public RoundController RoundController { get { return roundController; } }


    public static GameController Instance { get { return instance; } }

    static GameController instance;
    void Awake()
    {
        instance = this;
    }

    void Start()
    {
        if(onTutorialMode)
        {
            StartTutorial();
            return;
        }
    
        roundController.StartRound(0);
    }

    public void StartTutorial()
    {
        if(RoundController.IsRoundActive)
        {
            RoundController.EndRound(true);
        }

        onTutorialMode = true;
        dialogController.StartTutorial();
    }

    public void EndTutorial()
    {
        onTutorialMode = false;
        roundController.StartRound(0);
    }

    public void SelectTool(int toolIndex)
    {
        toolType = (ToolType)toolIndex;

        if(toolType == ToolType.Cutter)
        {
            cutterController.IsSelected = true;
            ingredientsController.SelectIngredient(0);
        }
        else cutterController.IsSelected = false;
    }

    public void SelectIngredient(int ingredientIndex)
    {
        SelectTool(0);
        ingredientsController.SelectIngredient(ingredientIndex);
    }
}
