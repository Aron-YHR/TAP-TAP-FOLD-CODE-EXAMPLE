using System.Collections;
using UnityEngine;

[RequireComponent(typeof(CircleCollider2D))]
public class CreaseAwayHit : MonoBehaviour
{
    //public SpriteRenderer tapPointRenderer;
    [Header("Crease Away Hit Circles")]
    [SerializeField] private GameObject outerCircle;
    [SerializeField] private GameObject circles;
    [SerializeField] public GameObject creaseLine;
    [SerializeField] private GameObject tapPoint;
    [SerializeField] private GameObject outerBoundary;
    [SerializeField] private GameObject innerBoundary;
    [SerializeField] private CircleCollider2D circleCollider;

    [Header("Scales and Expanding Time")]
    [SerializeField] private Vector3 startScale;
    [SerializeField] private Vector3 innerScale;
    [SerializeField] private Vector3 outerScale;
    //[SerializeField] private Vector3 endScale;
    [SerializeField] private float expandTime;

    public bool isSelected;
    public bool isComplete;

    public Vector3 postionOffset;

    private Coroutine currentLerpCoroutine;
    [SerializeField] public int failCount; 

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //StartCoroutine(ScaleLerp());
        //transform.localScale = startScale;
        tapPoint.transform.localScale = innerScale;
        innerBoundary.transform.localScale = innerScale;
        outerBoundary.transform.localScale = outerScale;
        circleCollider.radius = outerScale.x * 0.5f;
    }

    

    public IEnumerator ScaleLerp()
    {
        while (true)
        {
            bool playSFX = false;

            for (float i = 0; i < expandTime; i += Time.deltaTime)
            {
                tapPoint.transform.localScale = Vector3.Lerp(startScale, outerScale, i / expandTime);
                if(tapPoint.transform.localScale.x >= innerScale.x && !playSFX)
                {
                    playSFX = true;
                    Debug.Log("Reach correct boundary");
                    EventSender.SendSFXChange(MusicType.Crease_Point_Border);
                }
                yield return null;
            }
            
        }
        
    }

    private void OnMouseDown()
    {
        var currentPuzzleState = OrigamiController_Puzzle.Instance.GetComponent<CreaseAwayState>();

        EventSender.SendSFXChange(MusicType.Tap_Point_Pressed);

        if (!isSelected)
        {
            isSelected = true;
            outerCircle.SetActive(false);
            circles.SetActive(true);

            currentLerpCoroutine = StartCoroutine(ScaleLerp());
            return;
        }

        if (isSelected && !isComplete)
        {
            StopCoroutine(currentLerpCoroutine);
            isComplete = true;

            float score;
            bool isSuccessStep = false;

            score = tapPoint.transform.localScale.x - innerScale.x;

            float upperLimit = outerScale.x - innerScale.x;

            isSuccessStep = score >= 0f && score <= upperLimit;


                //1 - Vector3.Distance(tapPoint.transform.localScale.normalized, endScale.normalized);

            //score *= 100;

            //if(score >= 90)
            //{

            //}


            Debug.Log("Hit! " + isSuccessStep);

            if (!isSuccessStep)
            {

                currentLerpCoroutine = StartCoroutine(ScaleLerp());
                isComplete = false;
                failCount ++;
                currentPuzzleState.CalculateCombo(false);

                currentPuzzleState.isThisStepFailed = true;

                EventSender.SendTapMistake();
                EventSender.SendSFXChange(MusicType.Crease_Point_Failed);

                return;
            }

            circles.SetActive(false);
            creaseLine.SetActive(true);

            StarsAndScoresCaculator.CalculateScores(failCount);

            //combo
            currentPuzzleState.CalculateCombo(true);

            // score pop up
            if(currentPuzzleState.currentScoreCoroutine != null)
                StopCoroutine(currentPuzzleState.currentScoreCoroutine);

            //currentPuzzleState.currentScoreCoroutine = StartCoroutine( currentPuzzleState.ShowScore(OrigamiSystem_Puzzle.Instance.gameplayCamera.WorldToScreenPoint(transform.position+new Vector3(0.1f,0.1f,0))));

            currentPuzzleState.StartShowScore(OrigamiSystem_Puzzle.Instance.gameplayCamera.WorldToScreenPoint(transform.position + new Vector3(0.1f, 0.1f, 0)));

            EventSender.SendSFXChange(MusicType.Crease_Point_Complete);


            currentPuzzleState.OnPuzzleAction();
        }
    }

    public void ResetPoint()
    {
        isSelected = false;
        isComplete = false;
        failCount = 0;
        outerCircle.SetActive(true);
        circles.SetActive(false);
        creaseLine.SetActive(false);
    }


}
