using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class OrigamiController_Code : Singleton<OrigamiController_Code>
{
    public int currentOrderIndex = 0;
    public int MaxSteps;

    public List<FoldingPaper> foldPapers;

    public List<FoldingPaper> foldingSteps;

    public AnimationCurve rotationCurve; // better rotation effect

    public bool clickBlocked = false;

    public int maxNumberOfRotations;

    //int test = 0;

    public bool isFolding;

    public Pattern_SO patternList_SO;

    public Animator animatorController;

    public List<Button> touchPoints;
    public Button backButton;
    public List<TouchPointPosition> touchPointsPos;
    public List<GuessModePoints> guessModePoints;
    public int touchCount = 0;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        for (int i = 0; i < touchPoints.Count; i++)
        {
            touchPoints[i].onClick.AddListener(TouchPointAdd);
            touchPoints[i].transform.position = Camera.main.WorldToScreenPoint(OrigamiSystem.Instance.origamiSpawnPos.position +touchPointsPos[currentOrderIndex].pos[i]);
        }
        backButton.onClick.AddListener(OneStepBack);
        touchPoints[0].gameObject.SetActive(true);
    }

    // Update is called once per frame
    /*void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && clickBlocked == false)
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100))
            {
                if (hit.transform.GetComponent<ClickPointDetector>()?.foldPaperReference.rotated == false)
                {
                    //hit.transform.GetComponent<ClickPointDetector>().foldPaperReference.RotatePaper(false);
                    //hit.transform.GetComponent<ClickPointDetector>().foldPaperReference.FoldAnimation();
                }
                *//*else
                {
                    // if paper is already rotated, check that if paper is beneath other papers by comparing order indexes
                    if (hit.transform.GetComponent<ClickPointDetector>().foldPaperReference.orderIndex < currentOrderIndex - 1)
                    {
                        Debug.Log("Back to original");
                        StartCoroutine(RotateBackToIndex(hit.transform.GetComponent<ClickPointDetector>().foldPaperReference.orderIndex));
                    }
                    else // otherwise we only need to rotate back once
                    {
                        Debug.Log("Back once");
                        //hit.transform.GetComponent<ClickPointDetector>().foldPaperReference.RotatePaper(true);

                        StartCoroutine(RotateBackToIndex(test));

                    }

                }*//*
            }
        }
    }*/

    IEnumerator RotateBackToIndex(int index)
    {
        int tmpIndex = currentOrderIndex;

        while (currentOrderIndex != index)
        {
            if (tmpIndex == currentOrderIndex)
            {
                foldPapers[tmpIndex - 1].RotatePaper(true);
                tmpIndex--;
            }
            
            yield return new WaitForSeconds(1f);// wait for longest animation 
            //Debug.Break();
        }
        yield return null;
    }

    /*public void CheckIfRotatedEverythingCorrectly()
    {
        bool everythingCorrect = true;

        foreach(FoldPaper fp in foldPapers)
        {
            if(fp.rightOrder == false)
            {
                everythingCorrect = false;
            }
        }

        if(everythingCorrect == true)
        {
            Debug.Log("congrats");
        }
        else
        {
            StartCoroutine(RotateBackToIndex(0));
            Debug.Log("Try Again");
        }
    }*/

    public void OneStepBack()
    {
        if (currentOrderIndex > 0 && clickBlocked == false)
        {
            /*foreach (Button button in touchPoints)
            {
                button.gameObject.SetActive(true);
            }*/

            touchCount = 0;

            StartCoroutine(RotateBackToIndex(currentOrderIndex - 1));

            for (int i = 0; i < touchPoints.Count; i++)
            {
                if (i == 0)
                    touchPoints[i].gameObject.SetActive(true);
                else
                    touchPoints[i].gameObject.SetActive(false);
                touchPoints[i].interactable = true;
                touchPoints[i].transform.position = Camera.main.WorldToScreenPoint(OrigamiSystem.Instance.origamiSpawnPos.position+touchPointsPos[currentOrderIndex].pos[i]);
            }
        }
        //animatorController.SetTrigger("BackStep");

    }

    void TouchPointAdd()
    {
        if (clickBlocked == true) // click blocked, no tap
            return ;

        if (EventSystem.current.currentSelectedGameObject.TryGetComponent<Button>(out Button currentButton))
        {
            currentButton.interactable = false;

            for(int i = 1; i < touchPoints.Count; i++)
            {
                touchPoints[i].gameObject.SetActive(true);
            }
        }

        touchCount++;

        if(touchCount == 2)
        {
            //currentOrderIndex ++;

            OneStepFold();

            touchCount = 0;

            if(currentOrderIndex < touchPointsPos.Count)
            {

                for (int i = 0; i < touchPoints.Count; i++)
                {
                    if(i == 0) 
                        touchPoints[i].gameObject.SetActive(true);
                    else 
                        touchPoints[i].gameObject.SetActive(false);

                    touchPoints[i].interactable = true;
                    touchPoints[i].transform.position = Camera.main.WorldToScreenPoint(OrigamiSystem.Instance.origamiSpawnPos.position + touchPointsPos[currentOrderIndex].pos[i]);
                }
            }
            else
            {
                foreach(Button button in touchPoints)
                {
                    button.gameObject.SetActive(false);
                    
                }
            }

        }
    }

    public void OneStepFold()
    {
        if (clickBlocked == false)
        {
            if (foldingSteps[currentOrderIndex].rotated == false)
            {
                //OrigamiSystem.Instance.creaseManager.GenerateCreases(currentOrderIndex);

                foldingSteps[currentOrderIndex].RotatePaper(false);
                //foldPaperReference.FoldAnimation();
                
            }
        }
    }

}
