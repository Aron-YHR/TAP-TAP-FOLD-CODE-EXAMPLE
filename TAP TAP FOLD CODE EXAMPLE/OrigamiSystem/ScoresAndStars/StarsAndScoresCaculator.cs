using UnityEngine;

public class StarsAndScoresCaculator 
{
    public static void CalculateStars(float ratio)
    {
        StarType tempStars = StarType.Zero;
        int tempScrap = 0;
        //int
        //StarType currentPuzzleStar;

        if (ratio == 1.0f)
        {
            //3 stars
            //currentPuzzleStar = StarType.Three;
            tempStars = StarType.Three;
            
            // 250 scrap
            tempScrap += OrigamiSystem_Puzzle.Instance.scoresAndStars_SO.GetScrapsForStars(StarType.Three);
            
        }
        else if (ratio >= 0.7f)
        {
            //2 stars
            //currentPuzzleStar = StarType.Two;
            tempStars = StarType.Two;
            
            // 150 scrap
            tempScrap += OrigamiSystem_Puzzle.Instance.scoresAndStars_SO.GetScrapsForStars(StarType.Two);
           
        }
        else if (ratio == 0)
        {
            //0 star
            //currentPuzzleStar = StarType.Zero;
            tempStars = StarType.Zero;
            tempScrap += OrigamiSystem_Puzzle.Instance.scoresAndStars_SO.GetScrapsForStars(StarType.Zero);
            
            // 0 scrap

        }
        else
        {
            //1 star
            //currentPuzzleStar = StarType.One;
            tempStars = StarType.One;
            
            // 50 scrap
            tempScrap += OrigamiSystem_Puzzle.Instance.scoresAndStars_SO.GetScrapsForStars(StarType.One);
            

        }

        EventSender.SendStarChange(tempStars);
        EventSender.SendScrapChange(tempScrap);
    }

    public static void CalculateScores(int failCount)
    {
        int tempPoints = 0;
        if (failCount >= OrigamiSystem_Puzzle.Instance.scoresAndStars_SO.pointsScheme.points.Length)
        {
            tempPoints = OrigamiSystem_Puzzle.Instance.scoresAndStars_SO.pointsScheme.points[^1];
        }
        else
        {
            tempPoints = OrigamiSystem_Puzzle.Instance.scoresAndStars_SO.pointsScheme.points[failCount];
        }
        EventSender.SendScrapChange(tempPoints);
    }

    public static void CalculateComboPoints(int comboCount, PuzzleType puzzleType)
    {   

        int tempPoints = 0;

        switch (puzzleType)
        {
            case PuzzleType.FoldTime:
                comboCount = comboCount - OrigamiSystem_Puzzle.Instance.scoresAndStars_SO.pointsScheme.foldComboPoints.Length;

                if(comboCount < 0) return;

                if (comboCount >= OrigamiSystem_Puzzle.Instance.scoresAndStars_SO.pointsScheme.foldComboPoints.Length)
                {
                    tempPoints = OrigamiSystem_Puzzle.Instance.scoresAndStars_SO.pointsScheme.foldComboPoints[^1];
                }
                else
                {
                    tempPoints = OrigamiSystem_Puzzle.Instance.scoresAndStars_SO.pointsScheme.foldComboPoints[comboCount];
                }
                break;

            case PuzzleType.Crease:
                comboCount = comboCount - OrigamiSystem_Puzzle.Instance.scoresAndStars_SO.pointsScheme.creaseComboPoints.Length;

                if (comboCount < 0) return;

                if (comboCount >= OrigamiSystem_Puzzle.Instance.scoresAndStars_SO.pointsScheme.creaseComboPoints.Length)
                {
                    tempPoints = OrigamiSystem_Puzzle.Instance.scoresAndStars_SO.pointsScheme.creaseComboPoints[^1];
                }
                else
                {
                    tempPoints = OrigamiSystem_Puzzle.Instance.scoresAndStars_SO.pointsScheme.creaseComboPoints[comboCount];
                }
                break;
        }
        //if (comboCount >= OrigamiSystem_Puzzle.Instance.scoresAndStars_SO.pointsScheme.foldComboPoints)
        //EventSender.SendScrapChange(tempPoints);
        EventSender.SendComboChange(tempPoints);
    }

    public static void CalculateFinalScraps(int points)
    {
        int finalScraps = OrigamiSystem_Puzzle.Instance.scoresAndStars_SO.GetScrapsForPoints(points);
        EventSender.SendFinalScraps(finalScraps);
    }
}
