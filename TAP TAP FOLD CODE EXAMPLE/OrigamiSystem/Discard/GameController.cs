using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class GameController : MonoBehaviour
{
    public int currentOrderIndex = 0;

    public List<FoldPaper> foldPapers;

    public AnimationCurve rotationCurve; // better rotation effect

    public bool clickBlocked = false;

    public int maxNumberOfRotations;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame && clickBlocked == false) 
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if(Physics.Raycast(ray, out hit, 100))
            {
                if(hit.transform.GetComponent<ClickPointDetector>().foldPaperReference.rotated == false)
                {
                    hit.transform.GetComponent<ClickPointDetector>().foldPaperReference.RotatePaper(false);
                }
                else // if paper is already rotated, check that if paper is beneath other papers by comparing order indexes
                {
                    if(hit.transform.GetComponent<ClickPointDetector>().foldPaperReference.orderIndex < currentOrderIndex - 1)
                    {
                        StartCoroutine(RotateBackToIndex(hit.transform.GetComponent<ClickPointDetector>().foldPaperReference.orderIndex));
                    }
                    else // otherwise we only need to rotate back once
                    {
                        hit.transform.GetComponent<ClickPointDetector>().foldPaperReference.RotatePaper(true);
                    }
                }
            }
        }
    }

    IEnumerator RotateBackToIndex(int index)
    {
        int tmpIndex = currentOrderIndex;

        while(currentOrderIndex != index)
        {
            if(tmpIndex == currentOrderIndex)
            {
                foldPapers[tmpIndex - 1].RotatePaper(true);
                tmpIndex--;
            }
            yield return new WaitForEndOfFrame();
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
}
