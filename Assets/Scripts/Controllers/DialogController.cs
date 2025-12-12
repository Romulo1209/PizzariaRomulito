using System.Collections;
using UnityEngine;
using TMPro;
using System;
using System.Collections.Generic;

public class DialogController : MonoBehaviour
{
    [SerializeField] private TMP_Text dialogText;
    [SerializeField] [Range(0.01f, 0.1f)] private float typingSpeed = 0.05f;
    [SerializeField] private TutorialScriptable tutorial;
    private int currentTutorialStep = 0;

    private Coroutine writeCoroutine;

    void OnEnable()
    {
        DialogUi.OnClicked += NextDialog;
    }

    void OnDisable()
    {
        DialogUi.OnClicked -= NextDialog;
    }

    public void StartTutorial()
    {
        currentTutorialStep = 0;
        SetDialog(tutorial.tutorialSteps[currentTutorialStep]);
        currentTutorialStep++;
    }

    public void NextDialog()
    {
        if(!GameController.Instance.OnTutorialMode)
            return;

        if(currentTutorialStep >= tutorial.tutorialSteps.Count)
        {
            GameController.Instance.EndTutorial();
            return;
        }  

        if(GameController.Instance.OnTutorialMode)
        {
            if(writeCoroutine != null)
            {
                StopCoroutine(writeCoroutine);
                writeCoroutine = null;
                dialogText.text = tutorial.tutorialSteps[currentTutorialStep - 1];
                return;
            }
            SetDialog(tutorial.tutorialSteps[currentTutorialStep]);
            currentTutorialStep++;
        }  
    }

    public void SetDialog(int sliceCount, List<RoundPizzasIngredients> ingredients)
    {
        string chatDialog = $"Olá, quero uma pizza com {sliceCount} fatias!";
        foreach(var ingredient in ingredients)
        {
            chatDialog += $"\n - {ingredient.quantity} / {sliceCount} de {ingredient.ingredientType}";
        }

        SetDialog(chatDialog);
    }

    public void SetDialog(string message)
    {
        if(writeCoroutine != null)
            StopCoroutine(writeCoroutine);
            
        writeCoroutine = StartCoroutine(TypeWriteEffect(message));
    }

    private IEnumerator TypeWriteEffect(String message)
    {
        dialogText.text = "";
        foreach (char letter in message.ToCharArray())
        {
            dialogText.text += letter;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
