using System.Collections;
using System.Collections.Generic;
using TeamNotMuch.UI;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class OrigamiController_Puzzle : Singleton<OrigamiController_Puzzle>
{
    //public GameObject linePrefab;
    //public List<LineRenderer> lineRendererList;
    //public Vector3 previousPosition;

    [Header("Animation Switch")]
    [SerializeField] private bool isSwitch;

    //[SerializeField]private float minDistance = 0.1f;
    [Header("Puzzle UI")]
    //public Button submitButton;
    public Image nextStepImg;
    //public GameObject CompletePuzzlePopUp;
    public GameObject tempImg;
    public GameObject animObject;
    [SerializeField]private Vector3 animScale = Vector3.one;
    

    [Header("Overlay")]
    [SerializeField] private GameObject overlayPre;
    [SerializeField] private Vector3 finalScale = Vector3.one;
    [SerializeField] private float scaleDuration = 1.0f;
    [SerializeField] private Ease easeScale = Ease.OutBounce;


    //public int MaxSteps;


    [Header("Puzzle Essential")]
    public Transform origamiSpawnPos;
    public Transform animSpawnPos;
    public Transform uiParent;



    //public List<TapPoint_Mono> touchPoints;
    //public List<TapPoint_Mono> selectedButtons;

    [Header("Current Puzzle")]
    public int currentOrderIndex = 0;
    public OrigamiType currentOrigamiType;
    public GameObject currentOrigami;

    public PuzzleType currentPuzzle;

    public List<GameObject> origamiModelForEachStepList;
    public List<GameObject> animationForEachStepList;

    public List<PuzzleForEachStep> puzzleForEachSteps;

    public List<Sprite> StepsImgList;

    public Tutorial tutorial;

    public Dictionary<int,string> TutorialSteps = new Dictionary<int,string>();

    public bool HasTutorial;



    //public bool drawLineTime;
    //public bool selectFoldPointsTime;

    IPuzzleState puzzleState;
    public Sprite currentSprite;


    public bool isPuzzleSolved;
    private bool isChangingStep;

    [SerializeField] private float delayTime = 1f;

    [Header("Scores")]
    [SerializeField] private StarType currentStarType = StarType.Zero;
    [SerializeField] private int correctCount = 0;
    [SerializeField] private int incorrectCount = 0;
    [SerializeField] private int totalPoints = 0;
    [SerializeField] private int finalScrap = 0;
    private ScoreScreenData temp;


    private void OnEnable()
    {
        EventDispatcher.Register(EventNames.Scrap_Change, OnCalculatePoints);
        EventDispatcher.Register(EventNames.Combo_Change, OnCalculatePoints);
        EventDispatcher.Register(EventNames.FinalScrap_Change, OnCalculateFinalScraps);

        EventDispatcher.Register(EventNames.Star_Change, OnFinalStars);
    }
    private void OnDisable()
    {
        EventDispatcher.Unregister(EventNames.Scrap_Change, OnCalculatePoints);
        EventDispatcher.Unregister(EventNames.Combo_Change, OnCalculatePoints);
        EventDispatcher.Unregister(EventNames.FinalScrap_Change, OnCalculateFinalScraps);


        EventDispatcher.Unregister(EventNames.Star_Change, OnFinalStars);
    }

    private void Start()
    {
        //CreateCreases();
    }

    private void Update()
    {
        /*       if (currenPuzzle == PuzzleType.CreaseTime && drawLineTime == true) //draw line & crease update
               {
                   if (Mouse.current.leftButton.wasPressedThisFrame)
                   {
                       Vector3 vector3 = Vector3.zero;

                       if (EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.tag== "TapPoint")
                       {
                           vector3 = WorldPosConverter(EventSystem.current.currentSelectedGameObject.transform.position);

                           //EventSystem.current.currentSelectedGameObject.GetComponent<TapPoint_Mono>().SelectedThisPoint(true);

                           CreateCreases(vector3);
                       }
                   }

                   if (Mouse.current.leftButton.isPressed && lineRendererList.Count > 0 && EventSystem.current.currentSelectedGameObject != null && EventSystem.current.currentSelectedGameObject.tag == "TapPoint")
                   {
                       Vector3 vector3 = WorldPosConverter( Input.mousePosition);
                       for(int i =0; i < puzzleForEachSteps[currentOrderIndex].touch_Point_Positions.Count; i++)
                       {
                           if(Vector3.Distance( vector3, origamiSpawnPos.position + puzzleForEachSteps[currentOrderIndex].touch_Point_Positions[i]) < minDistance)
                           {
                               vector3 = origamiSpawnPos.position + puzzleForEachSteps[currentOrderIndex].touch_Point_Positions[i];

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

                   if(Mouse.current.leftButton.wasReleasedThisFrame)
                   {
                       for(int i = 0; i<touchPoints.Count; i++)
                       {
                           if (!touchPoints[i].isSelected && touchPoints[i].isHightlighted)
                           {
                               touchPoints[i].SelectedThisPoint(true);
                               drawLineTime = false;
                               selectFoldPointsTime = true;
                           }
                       }
                   }
               }*/


    }

    public void InitialiseOrigami()
    {
        currentStarType = StarType.Zero;
        correctCount = 0;
        incorrectCount = 0;
        totalPoints = 0;
        finalScrap = 0;

        if (tutorial.steps.Count > 0)
        {
            HasTutorial = true;

            TutorialSteps.Clear();
            foreach(var step in tutorial.steps)
            {
                TutorialSteps.Add(step.triggerAtStep,step.tutorialMessage);
            }
        }
        else
        {
            HasTutorial = false;
            TutorialSteps.Clear();
        }

        //currentTexture = OrigamiSystem_Puzzle.Instance.textures_SO.textures[OrigamiSystem_Puzzle.Instance.currentTextureChoice];

        /*if(isSwitch)
        {
            StartCoroutine(ShowOverlayAndAnimation());
        }*/

        SpawnOrigamiPuzzle();

        //StartCoroutine( SpawnOrigamiPuzzle() );
    }

    private void SpawnOrigamiPuzzle()
    {
        //yield return new WaitForSeconds(1f);

        isPuzzleSolved = false;

        //ActivePuzzleUI();

        if (currentOrigami != null)
        {
            Destroy(currentOrigami);
        }

        if(animObject != null)
            Destroy(animObject);

        currentPuzzle = PuzzleType.noPuzzle;
        puzzleState = null;

        currentOrigami = Instantiate(origamiModelForEachStepList[currentOrderIndex], origamiSpawnPos.position, origamiModelForEachStepList[currentOrderIndex].transform.rotation);

        //Set up selected texture

        SpriteRenderer[] tempSpriteList = currentOrigami.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < tempSpriteList.Length; i++)
        {
            tempSpriteList[i].material = OrigamiSystem_Puzzle.Instance.GetOverlayMaterial();
        }



        animObject = Instantiate(animationForEachStepList[currentOrderIndex], animSpawnPos.position , animationForEachStepList[currentOrderIndex].transform.rotation);
        animObject.transform.localScale = animScale;

        SpriteRenderer[] tempSpriteList1 = animObject.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < tempSpriteList1.Length; i++)
        {
            tempSpriteList1[i].material = OrigamiSystem_Puzzle.Instance.GetOverlayMaterial();
        }

        //currentOrigami.GetComponent<SpriteRenderer>().sprite = currentTexture;

        if (currentOrderIndex >= puzzleForEachSteps.Count) return;

        currentPuzzle = puzzleForEachSteps[currentOrderIndex].puzzleType;

        //nextStepImg.sprite = StepsImgList[currentOrderIndex];
        //nextStepImg.SetNativeSize();

        //GeneratePoints();

        //drawLineTime= true; // start draw line session

        

        switch (currentPuzzle)
        {
            case PuzzleType.FoldTime:
                puzzleState ??= GetComponent<FoldTimeState>();
                Debug.Log(puzzleState);
                puzzleState.StartPuzzle();
 

                break;
            case PuzzleType.Crease:
                puzzleState ??= GetComponent<CreaseAwayState>();
                puzzleState.StartPuzzle();
                break;
            case PuzzleType.RotationTime:
                puzzleState ??= GetComponent<FoldTimeState>();
                puzzleState.StartPuzzle();

                break;
            default:
                Debug.Log("No such puzzle!");
                break;
        }

        //Check tutorial spawn for step 0
        CheckAndSpawnTutorial();


    }

    /*public void GeneratePoints()
    {
        if (puzzleForEachSteps[currentOrderIndex].puzzleType == PuzzleType.CreaseTime)
        {
            //puzzleForEachSteps[currentOrderIndex].touch_Point_Positions.Count

            for (int i = 0; i < puzzleForEachSteps[currentOrderIndex].touch_Point_Positions.Count; i++)
            {
                touchPoints[i].gameObject.SetActive(true);

                touchPoints[i].worldPos = puzzleForEachSteps[currentOrderIndex].touch_Point_Positions[i];
                //touchPoints[i].onClick.AddListener(CreateCreases(touchPoints[i].transform));

                touchPoints[i].transform.position = Camera.main.WorldToScreenPoint(origamiSpawnPos.position + touchPoints[i].worldPos);
            }

            for (int i = puzzleForEachSteps[currentOrderIndex].touch_Point_Positions.Count; i< touchPoints.Count; i++)
            {
                touchPoints[i].gameObject.SetActive(false);
            }
        }
    }*/

    /*public void CreateCreases(Vector3 clickPos)
    {
        LineRenderer tempLine = Instantiate(linePrefab, origamiSpawnPos.position + new Vector3(0,0.1f,0), Quaternion.identity).GetComponent<LineRenderer>();
        lineRendererList.Add(tempLine);

        tempLine.positionCount = 2;
        tempLine.SetPosition(0, clickPos);
        tempLine.SetPosition(1, clickPos);
    }*/

    /*public Vector3 WorldPosConverter(Vector3 screenPos)
    {
        Vector3 vector3 = Camera.main.ScreenToWorldPoint(screenPos);
        vector3.y = 0;
        return vector3;
    }*/

    public void SubmitAnswer()
    {
        puzzleState.CompletePuzzle();

        if (isPuzzleSolved)
        {
            puzzleState.ExitPuzzle();

            currentOrderIndex++;
            EventSender.SendStepIndexChange(currentOrderIndex);

            //correctCount++;
            //DisactivePuzzleUI();

            if (currentOrderIndex >= puzzleForEachSteps.Count)
            {
                //CompletePuzzlePopUp.SetActive(true);

                //currentOrderIndex = 0;
                /*if (currentOrigami != null)
                {
                    Destroy(currentOrigami);
                }

                if (animObject != null)
                    Destroy(animObject);*/

               

                incorrectCount = puzzleForEachSteps.Count - correctCount;



                StarsAndScoresCaculator.CalculateStars((float)correctCount / puzzleForEachSteps.Count);

                StarsAndScoresCaculator.CalculateFinalScraps(totalPoints);

                temp = new ScoreScreenData(currentStarType, incorrectCount, totalPoints, finalScrap, currentSprite);

                //EventSender.SendScoreScreenUpdate(temp);

                currentPuzzle = PuzzleType.noPuzzle;
                puzzleState = null;

                //Unlock pedastal
                PedastalManager.Instance.UnlockCover(currentOrigamiType, currentStarType);

                //Puzzle finishes
                StartCoroutine(DelayFinish());
                //EventSender.SendOrigamiIsFinished(currentOrigamiType);
                //UIManager

            }
            else
            {
                EventSender.SendSFXChange(MusicType.Fold_Sound);


               

                //NextPuzzle();
                //StopAllCoroutines();
                StartCoroutine(NextPuzzle());
            }
        }
        else
        {
            StartCoroutine(ShowTempImg());
        }


        /*        drawLineTime = false;
                selectFoldPointsTime = false;

                bool answer1 = false;
                bool answer2 = false;

                int rightLinesCount = 0;

                FoldTimeAnswer foldTimeAnswer = puzzleForEachSteps[currentOrderIndex].foldTimeAnswer;

                for (int i = 0; i < lineRendererList.Count; i++)
                {
                    for (int j = 0; j < foldTimeAnswer.linePos.Count; j++)
                    {
                        Vector3 startPoint = lineRendererList[i].GetPosition(0);
                        Vector3 endPoint = lineRendererList[i].GetPosition(1);

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

                if(rightLinesCount == lineRendererList.Count) answer1 = true;
                else answer1 = false;

                int rightPointsCount = 0;

                for(int i = 0; i< foldTimeAnswer.pointChoices.Count; i++)
                {
                    if (foldTimeAnswer.pointChoices.Contains(selectedButtons[i].worldPos))
                    {
                        rightPointsCount++;
                    }
                }

                if(rightPointsCount == foldTimeAnswer.pointChoices.Count) answer2 = true;
                else answer2=false;

                Debug.Log(answer1 + " " + answer2);

                //for()

                if (answer1 && answer2)
                {
                    NextPuzzle();
                    answer1 = false;
                    answer2 = false;
                    rightLinesCount = 0;
                    rightPointsCount = 0;
                }
                else
                {
                    //shaking paper...
                    Debug.Log("Wrong answer!!!");
                }*/
    }

    public IEnumerator DelayFinish()
    {
        isChangingStep = true;

        if (currentOrigami != null)
        {
            Destroy(currentOrigami);
        }

        if (animObject != null)
            Destroy(animObject);
        //play animation
        //StartCoroutine(PlayFoldPaperAnimation());

        //Overlay pop up


        if (overlayPre != null)
        {
            overlayPre.transform.DOScale(finalScale, scaleDuration).SetEase(easeScale).OnStart(() => overlayPre.SetActive(true));
        }

        GameObject foldPaperAni = Instantiate(animationForEachStepList[^1], origamiSpawnPos.position + new Vector3(0, 0, -0.3f), animationForEachStepList[^1].transform.rotation);

        //Change texture
        SpriteRenderer[] tempSpriteList = foldPaperAni.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < tempSpriteList.Length; i++)
        {
            tempSpriteList[i].material = OrigamiSystem_Puzzle.Instance.GetOverlayMaterial();
        }

        yield return new WaitForSeconds(foldPaperAni.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length);

        Destroy(foldPaperAni);

        if (overlayPre != null)
        {
            overlayPre.transform.DOScale(new Vector3(0, 0, 0), scaleDuration).SetEase(easeScale).OnComplete(() => overlayPre.SetActive(false));
        }

        SpawnOrigamiPuzzle();



        yield return new WaitForSeconds(delayTime);
        currentOrderIndex = 0;
        EventSender.SendStepIndexChange(currentOrderIndex);
        if (currentOrigami != null)
        {
            Destroy(currentOrigami);
        }

        if (animObject != null)
            Destroy(animObject);

        EventSender.SendOrigamiIsFinished(currentOrigamiType);
        isChangingStep = false;
    }

    public void QuitOrigami()
    {
        if(isChangingStep) return;

        puzzleState.ExitPuzzle();

        currentOrderIndex = 0;
        if (currentOrigami != null)
        {
            Destroy(currentOrigami);
        }

        if (animObject != null)
            Destroy(animObject);

        currentPuzzle = PuzzleType.noPuzzle;
        puzzleState = null;

        EventSender.SendOrigamiIsFinished(OrigamiType.Quit);
    }

    public void SkipOrigami() // only for development process not in real game
    {
        puzzleState.ExitPuzzle();

        currentOrderIndex = 0;
        if (currentOrigami != null)
        {
            Destroy(currentOrigami);
        }

        if (animObject != null)
            Destroy(animObject);

        currentPuzzle = PuzzleType.noPuzzle;
        puzzleState = null;

        EventSender.SendOrigamiIsFinished(currentOrigamiType);
    }

    public IEnumerator NextPuzzle()
    {
        isChangingStep = true;
        Debug.Log("Next !!!");

        if (currentOrigami != null)
        {
            Destroy(currentOrigami);
        }

        if (animObject != null)
            Destroy(animObject);

        //play animation
        //StartCoroutine(PlayFoldPaperAnimation());

        //Overlay pop up
        if (overlayPre != null)
        {
            overlayPre.transform.DOScale(finalScale, scaleDuration).SetEase(easeScale).OnStart(() => overlayPre.SetActive(true));
        }

        GameObject foldPaperAni = Instantiate(animationForEachStepList[currentOrderIndex - 1], origamiSpawnPos.position + new Vector3(0, 0, -0.3f), animationForEachStepList[currentOrderIndex - 1].transform.rotation);

        //Change texture
        SpriteRenderer[] tempSpriteList = foldPaperAni.GetComponentsInChildren<SpriteRenderer>();
        for (int i = 0; i < tempSpriteList.Length; i++)
        {
            tempSpriteList[i].material = OrigamiSystem_Puzzle.Instance.GetOverlayMaterial();
        }

        yield return new WaitForSeconds(foldPaperAni.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length);

        Destroy(foldPaperAni);

        if (overlayPre != null)
        {
            overlayPre.transform.DOScale(new Vector3(0, 0, 0), scaleDuration).SetEase(easeScale).OnComplete(() => overlayPre.SetActive(false));
        }

        //StartCoroutine(ShowOverlayAndAnimation());

        //yield return new WaitForSeconds(1f);
        SpawnOrigamiPuzzle();
        //StartCoroutine(SpawnOrigamiPuzzle());

        UIManager.Instance.ShowStepPopUpPopup(currentOrderIndex);

        Invoke(nameof(CheckAndSpawnTutorial),2f);

        //GeneratePoints();

        isChangingStep = false;
    }

    IEnumerator ShowOverlayAndAnimation()
    {
        if (overlayPre != null)
        {
            overlayPre.transform.DOScale(finalScale, scaleDuration).SetEase(easeScale).OnStart(() => overlayPre.SetActive(true));
        }

        int tempIndex;
        if (isSwitch)
          tempIndex = currentOrderIndex;
        else 
          tempIndex = currentOrderIndex - 1;

        GameObject foldPaperAni = Instantiate(animationForEachStepList[tempIndex], origamiSpawnPos.position + new Vector3(0, 0, -0.3f), animationForEachStepList[tempIndex].transform.rotation);

        //useless cannot work
        
        Debug.LogError("1");
        yield return new WaitForSeconds(foldPaperAni.GetComponent<Animator>().runtimeAnimatorController.animationClips[0].length);

        Destroy(foldPaperAni);

        if (overlayPre != null)
        {
            overlayPre.transform.DOScale(new Vector3(0, 0, 0), scaleDuration).SetEase(easeScale).OnComplete(() => overlayPre.SetActive(false));
        }
    }

    IEnumerator ShowTempImg()
    {
        tempImg.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.5f);

        tempImg.gameObject.SetActive(false);
    }

    private void ActivePuzzleUI()
    {
        //submitButton.gameObject.SetActive(true);
        //nextStepImg.gameObject.SetActive(true);
    }

    private void DisactivePuzzleUI()
    {
        //submitButton.gameObject.SetActive(false);
        //nextStepImg.gameObject.SetActive(false);
    }

    public void AddCorrectSteps()
    {
        correctCount++;
    }

    private void OnCalculatePoints(object points)
    {
        totalPoints += (int)points;
    }

    private void OnCalculateFinalScraps(object scraps)
    {
        finalScrap = (int)scraps;
    }

    private void OnFinalStars(object stars)
    {
        currentStarType = (StarType)stars;
    }

    public ScoreScreenData GetScoreScreenData()
    {
        return temp;
    }

    public void CheckAndSpawnTutorial()
    {
        if (HasTutorial && TutorialSteps.TryGetValue(currentOrderIndex, out var tutorialMessage))
        {
            UIManager.Instance.ShowBanner(tutorialMessage);

            TutorialSteps.Remove(currentOrderIndex);
        }

        if(TutorialSteps.Count == 0)
        {
            HasTutorial = false;
        }
    }

    public void PlayAnimationNow()
    {
        animObject.GetComponentInChildren<AnimationLooper>()?.StartLoopFromBeginning();
    }
}

public class ScoreScreenData
{
    public StarType currentStarType;
    public int incorrectCount;
    public int totalPoints;
    public int finalScrap;
    public Sprite origamiImage;

    public ScoreScreenData(StarType currentStarType, int incorrectCount, int totalPoints, int finalScrap, Sprite origamiImage)
    {
        this.currentStarType = currentStarType;
        this.incorrectCount = incorrectCount;
        this.totalPoints = totalPoints;
        this.finalScrap = finalScrap;
        this.origamiImage = origamiImage;
    }
}


