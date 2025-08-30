using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OrigamiSystem : Singleton<OrigamiSystem>
{
    public GameObject origamiUI;

    public Pattern_SO origamiPatterns_SO;

    public CreaseManager creaseManager;

    public GameObject currentOrigami;

    public Transform origamiSpawnPos;

    public Image nextStepImage;

    public List<Sprite> nextStepImgsList;

    //public LineRenderer line;


    public void SpawnOrigami(int ID)
    {
        if (currentOrigami != null)
        {
            //GameObject tempObject = currentOrigami;
            //currentOrigami = null;
            Destroy(currentOrigami);
        }

        foreach(OrigamiPattern origamiPattern in origamiPatterns_SO.origamiPatterns)
        {
            if (origamiPattern.origamiID == ID)
            {

                currentOrigami = Instantiate(origamiPattern.origamiModel, origamiSpawnPos.position, Quaternion.identity);

                if (origamiPattern.touchPointPositions.Count>0)
                {
                    if (currentOrigami.TryGetComponent<OrigamiController_Ani>(out OrigamiController_Ani origamiController))
                    {
                        origamiController.touchPointsPos = origamiPattern.touchPointPositions;
                        origamiController.MaxSteps = origamiPattern.touchPointPositions.Count;
                    }
                    else if (currentOrigami.TryGetComponent<OrigamiController_Code>(out OrigamiController_Code origamicontroller_c))
                    {
                        origamicontroller_c.touchPointsPos = origamiPattern.touchPointPositions;
                        origamicontroller_c.MaxSteps = origamiPattern .touchPointPositions.Count;
                    }

                    
                }

                if(origamiPattern.guessModePoints != null)
                {
                    if (currentOrigami.TryGetComponent<OrigamiController_Ani>(out OrigamiController_Ani origamiController))
                    {
                        origamiController.totalGuessSteps = origamiPattern.totalGuessSteps;
                        origamiController.guessModePoints = origamiPattern.guessModePoints;
                        
                    }
                    else if (currentOrigami.TryGetComponent<OrigamiController_Code>(out OrigamiController_Code origamicontroller_c))
                    {
                        //origamicontroller_c.to
                        origamicontroller_c.guessModePoints = origamiPattern.guessModePoints;
                        
                    }
                }

                if (origamiPattern.origamiAnimationController != null)
                {
                    currentOrigami.GetComponent<OrigamiController_Ani>().animatorController.runtimeAnimatorController = origamiPattern.origamiAnimationController;
                }

                /*if(creaseManager != null)
                {
                    creaseManager.creases = origamiPattern.creaseLinesPoints;
                }*/

                nextStepImgsList = origamiPattern.StepsImgList;
            }
        }
    }

    public void EnterOrigami()
    {
        origamiUI.SetActive(true);
    }

    public void ExitOrigami()
    {
        origamiUI.SetActive(false);
        Destroy(currentOrigami);
    }

    public bool IfOrigamiIsDone()
    {
        if (currentOrigami != null)
        {
            if (currentOrigami.TryGetComponent<OrigamiController_Ani>(out OrigamiController_Ani origamiController))
            {
                if(origamiController.MaxSteps == origamiController.currentOrderIndex)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else if (currentOrigami.TryGetComponent<OrigamiController_Code>(out OrigamiController_Code origamicontroller_c))
            {
                if (origamicontroller_c.MaxSteps == origamicontroller_c.currentOrderIndex)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }
        }

        return false;
    }

    public void TempGuessMode()
    {
        origamiPatterns_SO.origamiPatterns[0].totalGuessSteps = 1;
        SpawnOrigami(0);
        
    }


    override protected void OnDestroy()
    {
        origamiPatterns_SO.origamiPatterns[0].totalGuessSteps = 0;
    }

    public void ShowNextStepImg()
    {
        nextStepImage.sprite = nextStepImgsList[currentOrigami.GetComponent<OrigamiController_Ani>().currentOrderIndex];
        //currentOrigami.GetComponent<OrigamiController_Ani>().currentOrderIndex++;
    }
}
