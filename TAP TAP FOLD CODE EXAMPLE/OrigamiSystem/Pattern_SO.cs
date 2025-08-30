using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Pattern_SO", menuName = "Scriptable Objects/Pattern_SO")]
public class Pattern_SO : ScriptableObject
{
    public List<OrigamiPattern> origamiPatterns;
}

[Serializable]
public class OrigamiPattern
{
    public int origamiID;
    public string patternName;

    public GameObject origamiModel;
    public GameObject petModel;

    public RuntimeAnimatorController origamiAnimationController;

    //
    public List<TouchPointPosition> touchPointPositions;

    public List<CreasePosition> creaseLinesPoints;

    public int totalGuessSteps;
    public List<GuessModePoints> guessModePoints;

    // the value of scrap for each level
    public List<int> scrapValueOfLevels;

    public List<Sprite> StepsImgList;

    public Tutorial tutorial;
    public bool HasTutorial => tutorial.steps.Count > 0;

}

[Serializable]
public class TouchPointPosition
{
    public List<Vector3> pos;
    //public Vector3 pos2;
}

[Serializable]
public class CreasePosition
{
    public List<CreaseStartPointAndEndPoint> creases;
}

[Serializable]
public class CreaseStartPointAndEndPoint
{
    public Vector3[] points;
}

[Serializable]
public class GuessModePoints
{
    public int step;
    public List<Vector3> extraPoints;
}

[Serializable]
public class Tutorial
{
    public List<TutorialStep> steps;
}

[Serializable]
public class TutorialStep
{
    public int triggerAtStep; //Refer to OrigamiController_Puzzle's currentOrderIndex. The step will fire when currentOrderIndex is completed
    public string tutorialMessage;
}