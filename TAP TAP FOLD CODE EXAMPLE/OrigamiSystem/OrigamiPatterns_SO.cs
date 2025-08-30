using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "OrigamiPatterns_SO", menuName = "Scriptable Objects/OrigamiPatterns_SO")]
public class OrigamiPatterns_SO : ScriptableObject
{
    public List<Origami_Pattern> origamiPatterns;
}

[Serializable]
public class Origami_Pattern
{
    public int origamiID;
    public OrigamiType origamiType;
    public Sprite scoreScreenImg;
    public GameObject petModel;

    public List<GameObject> origamiModelForEachStepList;
    public List<GameObject> animationForEachStepList;

    public List<PuzzleForEachStep> puzzleForEachSteps;

    //public RuntimeAnimatorController origamiAnimationController;

    //
    //public List<TouchPointPosition> touchPointPositions;

    //public List<CreasePosition> creaseLinesPoints;

    //public int totalGuessSteps;
    //public List<GuessModePoints> guessModePoints;

    // the value of scrap for each level
    //public List<int> scrapValueOfLevels;

    public List<Sprite> StepsImgList;

    public Tutorial tutorial;

}

[Serializable]
public class PuzzleForEachStep
{
    public PuzzleType puzzleType;

   
    public List<Vector3> touch_Point_Positions;
    //public List<Touch_Point_Position> guessModePoints;
    public FoldTimeAnswer foldTimeAnswer;

}

[Serializable]
public class FoldTimeAnswer
{
    public List<FoldLine> linePos;
    public List<Vector3> pointChoices;
    //public Vector3 pos2;
}


[Serializable]
public class FoldLine
{
    public Vector3[] points;
}

public enum OrigamiType
{
    Quit,
    Butterfly,
    Frog,
    Crane
}

public enum PuzzleType
{
    noPuzzle,
    FoldTime,
    Crease,
    RotationTime
}

