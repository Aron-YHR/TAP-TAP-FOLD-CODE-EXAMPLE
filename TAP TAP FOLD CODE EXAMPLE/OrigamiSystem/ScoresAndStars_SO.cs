using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoresAndStars_SO", menuName = "Scriptable Objects/ScoresAndStars_SO")]
public class ScoresAndStars_SO : ScriptableObject
{
    public List<StarsScheme> starsScheme;
    //public Dictionary<StarType, int> temp;
    public PointsScheme pointsScheme;

    public List<PointsToScrapsScheme> scrapsScheme;

    public int GetScrapsForStars(StarType starType)
    {
        foreach (StarsScheme s in starsScheme)
        {
            if (s.starType == starType)
            {
                return s.scraps;
            }
        }
        return 0;
    }

    public int GetScrapsForPoints(int points)
    {
        for (int i = scrapsScheme.Count -1; i >= 0; i--)
        {
            if ( points >= scrapsScheme[i].totalPoints)
            {
                return scrapsScheme[i].Scraps;
            }
        }
        return 0;
    }
}

[Serializable]
public class StarsScheme
{
    public StarType starType;
    public int scraps;
}

[Serializable]
public class PointsScheme
{
    public int[] points;
    public int[] foldComboPoints;
    public int[] creaseComboPoints;
}

public enum StarType
{
    Zero,
    One,
    Two,
    Three

}

[Serializable]
public class PointsToScrapsScheme
{
    public int totalPoints;
    public int Scraps;
}