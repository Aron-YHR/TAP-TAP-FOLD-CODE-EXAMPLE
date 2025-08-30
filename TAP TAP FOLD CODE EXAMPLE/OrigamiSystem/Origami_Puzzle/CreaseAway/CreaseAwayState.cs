using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class CreaseAwayState : MonoBehaviour, IPuzzleState
{
    //public GameObject creaseAwayHitPrefab;

    public List<CreaseAwayHit> touchPoints;
    public List<Vector3> touch_Point_Positions;

    public int creaseAwayCount;

    public GameObject scorePopup;
    public TextMeshProUGUI comboText;

    private int comboCount;
    public bool isThisStepFailed;

    public Coroutine currentScoreCoroutine;

    private void OnEnable()
    {
        EventDispatcher.Register(EventNames.Scrap_Change, OnCalculatePoints);
        EventDispatcher.Register(EventNames.Combo_Change, OnComboPoints);
    }

    private void OnDisable()
    {
        EventDispatcher.Unregister(EventNames.Scrap_Change, OnCalculatePoints);
        EventDispatcher.Unregister(EventNames.Combo_Change, OnComboPoints);
    }

    private void OnCalculatePoints(object points)
    {
        int tempPoints = (int)points;

        scorePopup.GetComponent<TextMeshProUGUI>().text = $"+{tempPoints}";
    }

    private void OnComboPoints(object points)
    {
        if (OrigamiController_Puzzle.Instance.currentPuzzle != PuzzleType.Crease) return;

        int tempPoints = (int)points;
        comboText.text = $"+COMBO {points}";
        comboText.gameObject.SetActive(true);
    }

    public void StartShowScore(Vector3 pos)
    {
        currentScoreCoroutine = StartCoroutine(ShowScore(pos));
    }

    public IEnumerator ShowScore(Vector3 pos)
    {
        scorePopup.transform.position = pos;

        scorePopup.SetActive(true);
        //Debug.Log("Crease score show up");
        yield return new WaitForSeconds(1.5f);
        scorePopup.SetActive(false);
        comboText.gameObject.SetActive(false);
        //Debug.Log("Crease score hide");
    }

    public void GeneratePoints()
    {
        //if (puzzleForEachSteps[currentOrderIndex].puzzleType == PuzzleType.CreaseTime)
        //{
        //puzzleForEachSteps[currentOrderIndex].touch_Point_Positions.Count

        //Vector3 direction = (touch_Point_Positions[touch_Point_Positions.Count-1] - touch_Point_Positions[0]).normalized;
        Vector3 direction = (touch_Point_Positions[0] - touch_Point_Positions[touch_Point_Positions.Count - 1]).normalized;
        direction = new Vector3(-direction.y, direction.x, 0).normalized;
        //Debug.Log();

        for (int i = 0; i < touch_Point_Positions.Count; i++)
        {
            touchPoints[i].gameObject.SetActive(true);

            touchPoints[i].postionOffset = touch_Point_Positions[i];
            //touchPoints[i].onClick.AddListener(CreateCreases(touchPoints[i].transform));
            //Debug.Log(OrigamiController_Puzzle.Instance.origamiSpawnPos.localPosition);
            touchPoints[i].transform.position =OrigamiController_Puzzle.Instance.origamiSpawnPos.transform.TransformPoint(touchPoints[i].postionOffset);

            touchPoints[i].creaseLine.transform.localRotation = Quaternion.LookRotation(Vector3.forward,direction);

            touchPoints[i].failCount = 0;
        }



        for (int i = touch_Point_Positions.Count; i < touchPoints.Count; i++)
        {
            touchPoints[i].gameObject.SetActive(false);
        }
        //}
    }

    public void CompletePuzzle()
    {
        if (creaseAwayCount != touch_Point_Positions.Count)
        {
            Debug.Log("You haven't done all the creases!");
            //ResetPuzzle(true);
        }
        else
        {
            OrigamiController_Puzzle.Instance.isPuzzleSolved = true;
            if(!isThisStepFailed) OrigamiController_Puzzle.Instance.AddCorrectSteps();
        }
    }

    public void ExitPuzzle()
    {
        //scorePopup.SetActive(false);
        //comboText.gameObject.SetActive(false);
        //StopCoroutine(currentScoreCoroutine);
        //StartCoroutine(DelayExit());
        ResetPuzzle(false);
    }

    public void OnPuzzleAction()
    {
        creaseAwayCount++;
        if(creaseAwayCount == touch_Point_Positions.Count)
        {
            //ResetPuzzle(true);
            //CompletePuzzle();

            OrigamiController_Puzzle.Instance.SubmitAnswer();
            Debug.Log("Finish all tap points!"); //UI pop up
        }
    }

    public void StartPuzzle()
    {
        touch_Point_Positions = OrigamiController_Puzzle.Instance.puzzleForEachSteps[OrigamiController_Puzzle.Instance.currentOrderIndex].touch_Point_Positions;

        GeneratePoints();

        creaseAwayCount = 0;
        comboCount = 0;
        isThisStepFailed = false;

    }

    public void ResetPuzzle(bool reset)
    {
        for (int i = 0; i < touch_Point_Positions.Count; i++)
        {
            touchPoints[i].ResetPoint();
            touchPoints[i].gameObject.SetActive(reset);
           
        }
        creaseAwayCount = 0;
        comboCount = 0;
        isThisStepFailed = false;

    }

    public void CalculateCombo(bool isCombo)
    {
        if (isCombo)
            comboCount++;
        else
            comboCount = 0;
        StarsAndScoresCaculator.CalculateComboPoints(comboCount, PuzzleType.Crease);
    }
}
