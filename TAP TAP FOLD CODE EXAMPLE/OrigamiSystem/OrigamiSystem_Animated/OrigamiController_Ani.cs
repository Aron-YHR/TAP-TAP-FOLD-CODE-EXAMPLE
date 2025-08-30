using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class OrigamiController_Ani : Singleton<OrigamiController_Ani>
{
    public int currentOrderIndex = 0;
    public int MaxSteps;

    //public bool clickBlocked = false;
    //public bool isFolding;

    public List<Button> touchPoints;
    public Button backButton;

    public List<TouchPointPosition> touchPointsPos;

    public int totalGuessSteps;
    public List<int> currentGuessSteps;
    public List<GuessModePoints> guessModePoints;

    public int touchCount=0;

    public Animator animatorController;

    [SerializeField] GameObject tapPrefab;
    public bool isGuessing = false;
    [SerializeField] int failCount;
    int tryCount;

    private void Start()
    {
        GenerateOrigami();
        GenerateRandomGuessSteps();
    }

    void GenerateTapPoints()
    {
        for (int i = 0; i < 2; i++)
        {
            /*if (i == 0)
                touchPoints[i].gameObject.SetActive(true);
            else
                touchPoints[i].gameObject.SetActive(false);*/

            touchPoints[i].interactable = true;

            touchPoints[i].transform.position = Camera.main.WorldToScreenPoint(OrigamiSystem.Instance.origamiSpawnPos.position + touchPointsPos[currentOrderIndex].pos[i]);
        }

        if (currentGuessSteps!=null && currentGuessSteps.Contains(currentOrderIndex))
        {
            isGuessing = true;
            for(int i = touchPoints.Count-2; i < guessModePoints[currentOrderIndex-1].extraPoints.Count; i++)
            {
                GameObject newTap = Instantiate(tapPrefab, touchPoints[0].transform.parent);

                newTap.GetComponent<Button>().onClick.AddListener(FakeFold);

                touchPoints.Add(newTap.GetComponent<Button>());
                newTap.SetActive(false);

            }

            for (int i = 2; i < touchPoints.Count; i++)
            {
                /*if (i == 0)
                    touchPoints[i].gameObject.SetActive(true);
                else
                    touchPoints[i].gameObject.SetActive(false);*/

                touchPoints[i].interactable = true;

                touchPoints[i].transform.position = Camera.main.WorldToScreenPoint(OrigamiSystem.Instance.origamiSpawnPos.position + guessModePoints[currentOrderIndex - 1].extraPoints[i-2]);

                
            }

            

        }
    }

    void GenerateRandomGuessSteps()
    {
        List<int> pool = Enumerable.Range(1, MaxSteps-1).ToList(); // Generate a index pool

        for(int i = 0; i < totalGuessSteps; i++)
        {
            if(pool.Count ==  0) break;

            int index = Random.Range(0, pool.Count);

            currentGuessSteps.Add(pool[index]);

            pool.RemoveAt(index); // delete index to avoid repeat
        }
    }

    void GenerateOrigami()
    {
        for (int i = 0; i < touchPoints.Count; i++)
        {
            touchPoints[i].onClick.AddListener(TouchPointAdd);
            touchPoints[i].transform.position = Camera.main.WorldToScreenPoint(OrigamiSystem.Instance.origamiSpawnPos.position + touchPointsPos[currentOrderIndex].pos[i]);
        }

        backButton.onClick.AddListener(OneStepBack);

        touchPoints[0].gameObject.SetActive(true);
    }

    void TouchPointAdd()
    {
        if (EventSystem.current.currentSelectedGameObject.TryGetComponent<Button>(out Button currentButton))
        {
            currentButton.interactable = false;

            //for (int i = 1; i < 2; i++)
            //{
                touchPoints[1].gameObject.SetActive(true);
            //}
        }

        touchCount++;

        if (touchCount == 2)
        {
            for (int i = 1; i < touchPoints.Count; i++)
            {
                touchPoints[i].gameObject.SetActive(false);
            }

            isGuessing = false;

            OneStepFold();
            TouchPointClear();

            currentOrderIndex++;

            if (currentOrderIndex < touchPointsPos.Count)
            {
                /*for (int i = 0; i < touchPoints.Count; i++)
                {
                    *//*if (i == 0)
                        touchPoints[i].gameObject.SetActive(true);
                    else
                        touchPoints[i].gameObject.SetActive(false);*//*

                    touchPoints[i].interactable = true;

                    touchPoints[i].transform.position = Camera.main.WorldToScreenPoint(OrigamiSystem.Instance.origamiSpawnPos.position + touchPointsPos[currentOrderIndex].pos[i]);
                }*/
                GenerateTapPoints();
            }
            else
            {
                foreach(Button button in touchPoints)
                {
                    button.gameObject.SetActive(false);
                    //OrigamiSystem.Instance.creaseManager.HideCreases();
                }
            }
        }
    }

    IEnumerator ResetOrigami()
    {
        for(int i = currentOrderIndex; i >0; i--)
        {
            OneStepBack();
            yield return new WaitForSeconds(2f);
        }
        
    }

    void FakeFold()
    {
        Debug.Log("Wrong tap!"); // to be detailed

        touchCount++;

        if (touchCount == 2)
        {
            TouchPointClear();
            tryCount++;
            for (int i = 0; i < touchPoints.Count; i++)
            {
                touchPoints[i].interactable = true;
            }
        }

        if(tryCount == failCount)
        {
            tryCount = 0;
            StartCoroutine(ResetOrigami());
        }
    }

    void TouchPointClear()
    {
        touchCount = 0;
    }


    public void OneStepFold()
    {
        
        animatorController.SetTrigger("NextStep");
        OrigamiSystem.Instance.ShowNextStepImg();
        
    }

    public void OneStepBack()
    {
        if(currentOrderIndex <= 0) return ;

        isGuessing = false;

        currentOrderIndex--;

        TouchPointClear();

        for (int i = 1; i < touchPoints.Count; i++)
        {
            touchPoints[i].gameObject.SetActive(false);
        }

        animatorController.SetTrigger("BackStep");



        /*for (int i = 0; i < touchPoints.Count; i++)
            {
                *//*if (i == 0)
                    touchPoints[i].gameObject.SetActive(true);
                else
                    touchPoints[i].gameObject.SetActive(false);*//*

                touchPoints[i].interactable = true;

                touchPoints[i].transform.position = Camera.main.WorldToScreenPoint(OrigamiSystem.Instance.origamiSpawnPos.position + touchPointsPos[currentOrderIndex].pos[i]);
            }*/
        GenerateTapPoints(); // fold back, bring the extra taps back again?


    }

    

}



