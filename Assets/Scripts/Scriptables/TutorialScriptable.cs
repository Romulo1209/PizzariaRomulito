using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "TutorialScriptable", menuName = "Scriptables/TutorialScriptable", order = 1)]
public class TutorialScriptable : ScriptableObject
{
    public List<string> tutorialSteps;
}
