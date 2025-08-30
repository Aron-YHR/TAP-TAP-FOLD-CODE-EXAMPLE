using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class FoldTimeState : MonoBehaviour, IPuzzleState
{
    [Header("Splines")]
    public GameObject linePrefab;
    public UISpline splineForFold;
    public UISpline splineForRotation;
    //public UISpline splineForTap;
    //public UISpline consistSpline;
    [SerializeField] public float minDistance = 0.1f;
    [SerializeField] private UISpline currentSpline;

    [Header("Splines list and Tap points list")]
    public List<LineRenderer> lineRendererList;
    public List<UISpline> uISplineList;

    public List<TapPoint_Mono> touchPoints;
    public List<TapPoint_Mono> selectedButtons;
    public int selectedCount = 0;

    [Header("Control Mode")]
    public bool drawLineTime;
    public bool selectFoldPointsTime;

    public bool rotationTime; // trigger when it's a rotation step

    public Toggle switchToggle;

    public List<Vector3> touch_Point_Positions;

    public FoldTimeAnswer foldTimeAnswer;

    [Header("Scores related")]
    public GameObject tips;
    //public GameObject tips1;
    public GameObject scorePopup;
    public TextMeshProUGUI comboText;

    [SerializeField]private int failCount;
    [SerializeField]private int comboCount = 0;

    /*public void SetUpFoldTime(List<Vector3> touch_Point_Positions)
    {
        this.touch_Point_Positions = touch_Point_Positions;

        GeneratePoints();
    }*/

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
        if(OrigamiController_Puzzle.Instance.currentPuzzle != PuzzleType.FoldTime) return;
        int tempPoints = (int)points;
        comboText.text = $"+COMBO {points}";
        comboText.gameObject.SetActive(true);
        //Debug.Log("Combo active");
    } 

    public void GeneratePoints()
    {
        //if (puzzleForEachSteps[currentOrderIndex].puzzleType == PuzzleType.CreaseTime)
        //{
            //puzzleForEachSteps[currentOrderIndex].touch_Point_Positions.Count

            for (int i = 0; i < touch_Point_Positions.Count; i++)
            {
                touchPoints[i].gameObject.SetActive(true);

                touchPoints[i].posOffset = touch_Point_Positions[i];
            //touchPoints[i].onClick.AddListener(CreateCreases(touchPoints[i].transform));
            //Debug.Log(OrigamiController_Puzzle.Instance.origamiSpawnPos.localPosition);
            touchPoints[i].transform.position = OrigamiController_Puzzle.Instance.origamiSpawnPos.transform.TransformPoint(touchPoints[i].posOffset);
                //Camera.main.WorldToScreenPoint(OrigamiController_Puzzle.Instance.origamiSpawnPos.transform.TransformPoint( touchPoints[i].worldPos));

            }

            for (int i = touch_Point_Positions.Count; i < touchPoints.Count; i++)
            {
                touchPoints[i].gameObject.SetActive(false);
            }
        //}
    }

    public void CompletePuzzle()
    {
        drawLineTime = false;
        selectFoldPointsTime = false;

        bool answer1 = false;
        //bool answer2 = false;

        int rightLinesCount = 0;

        if(uISplineList.Count != 0) //(lineRendererList.Count!=0)
        {

            for (int i = 0; i < uISplineList.Count; i++)
            {
                for (int j = 0; j < foldTimeAnswer.linePos.Count; j++)
                {
                    //Vector3 startPoint = lineRendererList[i].GetPosition(0);
                    //Vector3 endPoint = lineRendererList[i].GetPosition(1);
                    Vector3 startPoint = uISplineList[i].startOffset;
                    Vector3 endPoint = uISplineList[i].endOffest;

                    if ((startPoint == foldTimeAnswer.linePos[j].points[0] && endPoint == foldTimeAnswer.linePos[j].points[1]) || (endPoint == foldTimeAnswer.linePos[j].points[0] && startPoint == foldTimeAnswer.linePos[j].points[1]))
                    {
                        rightLinesCount++;
                        //Debug.Log(rightLinesCount);
                        break;
                    }
                    else
                    {
                        //Debug.Log(rightLinesCount);
                    }
                }
            }
            //Debug.Log(foldTimeAnswer.linePos.Count);
            //Debug.Log(rightLinesCount);

            if (rightLinesCount == uISplineList.Count) answer1 = true;
            else answer1 = false;
        }

        /*        int rightPointsCount = 0;

                if (selectedButtons.Count!=0)
                {

                    for (int i = 0; i < selectedButtons.Count; i++)
                    {
                        if (foldTimeAnswer.pointChoices.Contains(selectedButtons[i].posOffset))
                        {
                            rightPointsCount++;
                        }
                    }


                    if (rightPointsCount == foldTimeAnswer.pointChoices.Count) answer2 = true;
                    else answer2 = false;

                }*/

        Debug.Log(answer1); //+ " " + answer2);

        //for()

        if(answer1) //(answer1 && answer2)
        {
            //NextPuzzle();
            //answer1 = false;
            //answer2 = false;
            //rightLinesCount = 0;
            //rightPointsCount = 0;
            //ResetPuzzle();
            OrigamiController_Puzzle.Instance.isPuzzleSolved = true;

            comboCount++;

            StarsAndScoresCaculator.CalculateScores(failCount);
            StarsAndScoresCaculator.CalculateComboPoints(comboCount, PuzzleType.FoldTime);

            if(failCount == 0)
            {
                OrigamiController_Puzzle.Instance.AddCorrectSteps();
            }

            failCount = 0;

            StartCoroutine( ShowScore());
        }
        else
        {
            //shaking paper...
            //reset
            //answer1 = false;
            //answer2 = false;
            //rightLinesCount = 0;
            //rightPointsCount = 0;
            ResetPuzzle(true);
            //drawLineTime = true;
            failCount++;
            comboCount = 0;
            EventSender.SendTapMistake();
            EventSender.SendSFXChange(MusicType.Incorrect_Points);

            Debug.Log("Wrong answer!!!");
        }
    }

    IEnumerator ShowScore()
    {
        scorePopup.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        scorePopup.SetActive(false);
        comboText.gameObject.SetActive(false);
    }

    public void ExitPuzzle()
    {
        //hide fold lines when exit fold time puzzle
        EventSender.SendFoldLinesChange(-1);

        switchToggle.gameObject.SetActive(false);
        tips.SetActive(false);
        //scorePopup.SetActive(false);
        //comboText.gameObject.SetActive(false);
        ResetPuzzle(false);
        

    }

    public void OnPuzzleAction()
    {


        //Drag line detection
        /*if (lineRendererList.Count == foldTimeAnswer.linePos.Count)
        {
            drawLineTime = false;
            tips.SetActive(false);
            tips1.SetActive(true);
            //selectFoldPointsTime = true;
        }*/

        //Tap to create direction detection
        if (uISplineList.Count == foldTimeAnswer.linePos.Count)
        {
            drawLineTime = false;
            selectFoldPointsTime = false;
            //tips.SetActive(false);
            //tips1.SetActive(true);
            //selectFoldPointsTime = true;

            OrigamiController_Puzzle.Instance.SubmitAnswer();
        }
    }

    public void OnToggleValueChanged()
    {
        drawLineTime = !switchToggle.isOn;
        selectFoldPointsTime = switchToggle.isOn;
        ResetPuzzle(true);
    }

    public void StartPuzzle()
    {
        touch_Point_Positions = OrigamiController_Puzzle.Instance.puzzleForEachSteps[OrigamiController_Puzzle.Instance.currentOrderIndex].touch_Point_Positions;
        //selectedButtons = OrigamiController_Puzzle.Instance.selectedButtons;

        foldTimeAnswer = OrigamiController_Puzzle.Instance.puzzleForEachSteps[OrigamiController_Puzzle.Instance.currentOrderIndex].foldTimeAnswer;

        GeneratePoints();

        //drawLineTime = true;
        //selectFoldPointsTime=true;
        switchToggle.gameObject.SetActive(true);

        OnToggleValueChanged();
        if (OrigamiController_Puzzle.Instance.currentOrderIndex == 0)
            comboCount = 0;

        failCount = 0;
        selectedCount = 0;

        if (OrigamiController_Puzzle.Instance.currentPuzzle == PuzzleType.RotationTime)
        {
            currentSpline = splineForRotation;
            //rotationTime = true;
        }
        else
        {
            currentSpline = splineForFold;
            //rotationTime = false;
        }

        //inform how many fold lines 
        EventSender.SendFoldLinesChange(foldTimeAnswer.linePos.Count);

        //tips.SetActive(true);
    }

    public void ResetPuzzle(bool reset)
    {
        //OnToggleValueChanged();
        drawLineTime = !switchToggle.isOn;
        selectFoldPointsTime = switchToggle.isOn;

        for (int i = 0; i < touch_Point_Positions.Count; i++)
        {
            touchPoints[i].ResetPoint();
            touchPoints[i].gameObject.SetActive(reset);
        }

        /*for(int i=0;i<selectedButtons.Count;i++)
        {
            selectedButtons.Remove(selectedButtons[i]);
        }*/
        selectedButtons.Clear();


       /* for (int i = lineRendererList.Count -1; i >= 0; i--) // from back to the front avoiding the index issue
        {
            Destroy( lineRendererList[i].gameObject);
            lineRendererList.Remove(lineRendererList[i]); // clear reference
            //Destroy(lineG);
        }*/

        for (int i = uISplineList.Count - 1; i >= 0; i--) // from back to the front avoiding the index issue
        {
            Destroy(uISplineList[i].gameObject);
            uISplineList.Remove(uISplineList[i]); // clear reference
            //Destroy(lineG);
        }

        //splineForFold.gameObject.SetActive(false);

        tips.SetActive(true);
        selectedCount = 0;
        //tips1.SetActive(false);
    }

    public void CreateCreases(Vector3 clickPos)
    {
        LineRenderer tempLine = Instantiate(linePrefab, OrigamiController_Puzzle.Instance.origamiSpawnPos.TransformPoint( new Vector3(0, 0, -0.1f)), OrigamiController_Puzzle.Instance.origamiSpawnPos.rotation).GetComponent<LineRenderer>();
        lineRendererList.Add(tempLine);

        tempLine.positionCount = 2;
        tempLine.SetPosition(0, clickPos);
        tempLine.SetPosition(1, clickPos);
    }

    public void CreateSpline(Vector3 startPos, Vector3 endPos)
    {
        UISpline uISpline = null;
        //if (drawLineTime)
        uISpline = Instantiate(currentSpline.gameObject, OrigamiController_Puzzle.Instance.uiParent).GetComponent<UISpline>();
        //else if(selectFoldPointsTime)
       // uISpline = Instantiate(splineForTap.gameObject, OrigamiController_Puzzle.Instance.uiParent).GetComponent<UISpline>();
        if (uISpline != null)
        {
            uISplineList.Add(uISpline);

            //Debug.Log(startPos * OrigamiSystem_Puzzle.Instance.ratio+" "+ endPos * OrigamiSystem_Puzzle.Instance.ratio);

            startPos = startPos * OrigamiSystem_Puzzle.Instance.ratio;
            endPos = endPos * OrigamiSystem_Puzzle.Instance.ratio;

            uISpline.UpdateUISpline(startPos , endPos);
        }
    }

    public Vector3 WorldPosConverter(Vector3 screenPos)
    {
        Vector3 vector3 = Camera.main.ScreenToWorldPoint(screenPos);
        vector3.y = 0;
        return vector3;
    }

    /*private void Update()
    {
        //if (currenPuzzle == PuzzleType.CreaseTime && drawLineTime == true) //draw line & crease update
        if ( drawLineTime == true)
        {
            if (Mouse.current.leftButton.wasPressedThisFrame)
            {
                Vector3 vector3 = Vector3.zero;

                if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.tag == "TapPoint")
                {
                    vector3 = WorldPosConverter(EventSystem.current.currentSelectedGameObject.transform.position);

                    //EventSystem.current.currentSelectedGameObject.GetComponent<TapPoint_Mono>().SelectedThisPoint(true);

                    CreateCreases(vector3);
                }
            }

            if (Mouse.current.leftButton.isPressed && lineRendererList.Count > 0 && EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.tag == "TapPoint")
            {
                Vector3 vector3 = WorldPosConverter(Input.mousePosition);
                for (int i = 0; i < touch_Point_Positions.Count; i++)
                {
                    if (Vector3.Distance(vector3, OrigamiController_Puzzle.Instance.origamiSpawnPos.position + touch_Point_Positions[i]) < minDistance)
                    {
                        vector3 = OrigamiController_Puzzle.Instance.origamiSpawnPos.position + touch_Point_Positions[i];

                        //change transparency
                        touchPoints[i].HightLightPoint();
                    }
                    else
                    {
                        touchPoints[i].LowLightPoint();
                    }
                }
                //if( Vector3.Distance( Input.mousePosition))
                lineRendererList[lineRendererList.Count - 1].SetPosition(1, vector3);


            }

            if (Mouse.current.leftButton.wasReleasedThisFrame)
            {
                for (int i = 0; i < touchPoints.Count; i++)
                {
                    if (!touchPoints[i].isSelected && touchPoints[i].isHightlighted)
                    {
                        touchPoints[i].SelectedThisPoint(true);
                        OnPuzzleAction();
                    }
                }
                
            }
        }


    }*/

}
