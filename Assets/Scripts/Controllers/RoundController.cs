using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum SlicesCount { Two = 2, Four = 4, Six = 6, Eight = 8 }

public class RoundController : MonoBehaviour
{
    [Header("Round Settings")]
    [SerializeField] private float roundStartTimer = 3f;
    [SerializeField] private bool isRoundActive = false;
    [SerializeField] private SlicesCount requiredTotalSlices;
    [SerializeField] private List<RoundPizzasIngredients> roundPizzasIngredients;

    [Header("Character Info")]
    [SerializeField] private CharacterDatabaseScriptable characterDatabase;
    [SerializeField] private CharacterScriptable currentCharacter;
    [SerializeField] private Image characterImage;

    public bool IsRoundActive { get { return isRoundActive; } set { isRoundActive = value; } }
    private Coroutine startRoundCoroutine;

    public void StartRound(float? forceDelay = null)
    {
        if(startRoundCoroutine != null)
            StopCoroutine(startRoundCoroutine);
            
        startRoundCoroutine = StartCoroutine(StartRoundTimer(forceDelay ?? roundStartTimer));
    }

    public void EndRound(bool forceEnd = false)
    {
        if(!forceEnd)
            if(!isRoundActive || !RoundEnded()) 
                return;
            
        CameraController.Instance.MoveToOutsidePosition();
        isRoundActive = false;
        
        if(!forceEnd)
            StartRound();
    }

    public bool RoundEnded()
    {
        int currentSlicesCount = SlicesController.Instance.PizzaSlicesCount;
        if(currentSlicesCount != (int)requiredTotalSlices)
        {
            GameController.Instance.DialogController.SetDialog($"Número incorreto de fatias! Preciso de {(int)requiredTotalSlices} fatias!");
            characterImage.sprite = currentCharacter.characterSpriteSad;
            return false;
        }

        var pizzaSlices = SlicesController.Instance.PizzaSlices;
        foreach(var slice in pizzaSlices)
        {
            RoundPizzasIngredients existing = roundPizzasIngredients.Find(x => x.ingredientType == slice.CurrentIngredient);
            if(existing == null) continue;

            existing.CountAlreadyAdded();
        }
        bool ingredientsIncorrect = false;
        string dialogMessage = "";
        foreach(var ingredient in roundPizzasIngredients)
        {
            if(ingredient.currentQuantity > 0)
            {
                dialogMessage += $"\nFaltam {ingredient.currentQuantity} / {(int)requiredTotalSlices} das fatias de {ingredient.ingredientType}!";
                ingredientsIncorrect = true;
                continue;
            }
        }

        if(ingredientsIncorrect)
        {
            GameController.Instance.DialogController.SetDialog("Ingredientes incorretos!" + dialogMessage);
            characterImage.sprite = currentCharacter.characterSpriteSad;
            return false;
        }

        GameController.Instance.DialogController.SetDialog("Pizza correta! Bom trabalho!");
        characterImage.sprite = currentCharacter.characterSpriteHappy;
        return true;
    }

    void GenerateRoundIngredients()
    {
        requiredTotalSlices = Utils.GetRandomSlice();
        roundPizzasIngredients.Clear();
        int slicesLeft = (int)requiredTotalSlices;
        while(slicesLeft > 0)
        {
            RoundPizzasIngredients roundIngredient = new RoundPizzasIngredients();
            roundIngredient.ingredientType = GameController.Instance.IngredientsController.IngredientsDb.GetRandomIngredientType();
            roundIngredient.AddQuantity(Random.Range(1, slicesLeft + 1));

            RoundPizzasIngredients existing = roundPizzasIngredients.Find(x => x.ingredientType == roundIngredient.ingredientType);

            if (existing != null) existing.AddQuantity(roundIngredient.quantity);
            else roundPizzasIngredients.Add(roundIngredient);

            slicesLeft -= roundIngredient.quantity;
        }
        if(slicesLeft < 0)
        {
            GenerateRoundIngredients();
            return;
        }
        SlicesController.Instance.GeneratePizza();
    }

    IEnumerator StartRoundTimer(float delay)
    {
        yield return new WaitForSeconds(delay);
        
        currentCharacter = characterDatabase.characters[Random.Range(0, characterDatabase.characters.Count)];
        characterImage.sprite = currentCharacter.characterSpriteIdle;

        GenerateRoundIngredients();
        isRoundActive = true;
        GameController.Instance.DialogController.SetDialog((int)requiredTotalSlices, roundPizzasIngredients);
        CameraController.Instance.MoveToMainPosition();
    }
}

[System.Serializable]
public class RoundPizzasIngredients
{
    public IngredientType ingredientType;
    public int quantity;
    public int currentQuantity;

    public void AddQuantity(int value)
    {
        quantity += value;
        currentQuantity += value;
    }

    public void CountAlreadyAdded()
    {
        currentQuantity--;
    }
}
