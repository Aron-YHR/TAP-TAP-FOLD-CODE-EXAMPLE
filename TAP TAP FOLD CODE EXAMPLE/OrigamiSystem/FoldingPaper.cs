using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using DG.Tweening;

public class FoldingPaper : MonoBehaviour
{
    public int orderIndex = -1;
    public List<GameObject> corners; // rotate with the main part

    public List<GameObject> subFoldingCorners; // the corners fold with the main part with its own rotation

    public List<GameObject> subFoldingPieces; // the other folds along with this step

    public FoldingPaper mainFold; // the piece that this fold follows

    public OrigamiController_Code gameControllerRef;

    public Vector3 rotateAxis;
    public float rotationDuration;
    public float angleChange;

    public bool rotated = false;

    public int count = 0;

    public GameObject touchButton;

   /* public GameObject mainLine; // the horizontal one

    public List<LineRenderer> extraLines; // the vertical ones - those inside of the folding
    public List<int> extraLinesIndexes; // the indexes for the line start point or end point
    public List<float> extraLinesStartPosition; // the original point
    public List<float> extraLinesEndPosition; // the line point after folding*/

    public List<int> rotationOrderIndexes;
    //public bool rightOrder = false;

    public void RotatePaper(bool back) // prepare for rotation
    {
        /*foreach (GameObject g in corners)  // attach corners to the rotation
        {
            g.transform.parent = gameObject.transform;
        }*/

        /*if(corners.Count>5)
        { for (int i = 0; i < 5; i++)
            {
                corners[i].transform.parent = gameObject.transform;
            }
        }
        else
        {
            foreach (GameObject g in corners)  // attach corners to the rotation
            {
                g.transform.parent = gameObject.transform;
            }
        }*/
        if (corners != null)
        {
            foreach (GameObject g in corners)  // attach corners to the rotation
            {
                g.transform.parent = gameObject.transform;
                //g.GetComponent<FoldingPaper>().mainFold = this;
            }
        }

        if (subFoldingCorners != null)
        {
            foreach (GameObject g in subFoldingCorners)
            {
                g.transform.parent = gameObject.transform;
                g.GetComponent<FoldingPaper>().mainFold = this;
            }
        }

        if (subFoldingPieces != null)
        {
            foreach (GameObject g in subFoldingPieces)
            {
                g.GetComponent<FoldingPaper>().mainFold = this;
            }
        }


        if (back == false) // first time rotation
        {
            orderIndex = gameControllerRef.currentOrderIndex;
            if(mainFold == null)
            gameControllerRef.foldPapers.Add(this);
            //StartCoroutine(FoldAroundAxis(gameObject.transform, rotateAxis, angleChange, transform.position + new Vector3(0, orderIndex * 0.001f + 0.001f, 0), rotationDuration, back));

            GetEulerAnglesFromAxisAngle(angleChange, rotateAxis);

            FoldAroundAxis(gameObject.transform, rotateAxis, angleChange, rotationDuration, back); // main part folding

            if (subFoldingCorners != null)
            {
                foreach(GameObject g in subFoldingCorners)
                {
                    g.GetComponent<FoldingPaper>().RotatePaper(back);
                }

            }

            if(subFoldingPieces != null)
            {
                foreach (GameObject g in subFoldingPieces)
                {
                    g.GetComponent<FoldingPaper>().RotatePaper(back);
                }
            }

            

            rotated = true;

            //mainLine.SetActive(false); // hide the dashed lines after fodling

            //if(touchButton != null) 
                //touchButton.SetActive(false);

            /*for (int i = 0; i < extraLines.Count; i++)
            {
                extraLines[i].SetPosition(extraLinesIndexes[i], new Vector3(0, 0, extraLinesEndPosition[i]));
            }*/

            /*foreach(int orderIndex in rotationOrderIndexes) // check the order o show
            {
                if(orderIndex == gameControllerRef.currentOrderIndex)
                {
                    rightOrder = true;
                }
            }*/
        }
        else
        {
            orderIndex = -1;
            //StartCoroutine(FoldAroundAxis(gameObject.transform, rotateAxis, -angleChange, new Vector3(gameObject.transform.position.x, 0.003f, gameObject.transform.position.z), rotationDuration, back));

            if (subFoldingCorners != null)
            {
                foreach (GameObject g in subFoldingCorners)
                {
                    g.GetComponent<FoldingPaper>().RotatePaper(back);
                }

            }

            if (subFoldingPieces != null)
            {
                foreach (GameObject g in subFoldingPieces)
                {
                    g.GetComponent<FoldingPaper>().RotatePaper(back);
                }
            }

            FoldAroundAxis(gameObject.transform, rotateAxis, 0, rotationDuration, back);

            if (mainFold == null)
                gameControllerRef.foldPapers.Remove(this);
            rotated = false;

            //rightOrder = false; // fold back, reset the order
        }

        
    }

    public void FoldAnimation()
    {
        gameControllerRef.animatorController.SetTrigger("NextStep");

    }

    void FoldAroundAxis(Transform paperTransform, Vector3 axis, float changeInAngle, float duration, bool back)
    {
        gameControllerRef.clickBlocked = true;

        //paperTransform.DOLocalRotate(GetEulerAnglesFromAxisAngle(changeInAngle, axis), duration).SetEase(Ease.InOutSine);
        paperTransform.DOLocalRotateQuaternion(Quaternion.AngleAxis(changeInAngle, axis.normalized), duration).SetEase(Ease.InOutSine).OnComplete(() =>
        {
            if (back == false)
            {
                if (mainFold != null)
                {
                    mainFold.count++;

                }
                else count++;

                if (mainFold != null)
                {
                    if (mainFold.count == (mainFold.subFoldingCorners.Count + mainFold.subFoldingPieces.Count + 1)) //Reset the layer relationship when the final rotation is done.
                    {
                        Debug.Log(1);
                        ResetParent(back);
                        gameControllerRef.clickBlocked = false; // rotation finishes, be able to click again
                    }
                }
                else
                {
                    if (count == (subFoldingCorners.Count + subFoldingPieces.Count + 1)) //Reset the layer relationship when the final rotation is done.
                    {
                        Debug.Log(1);
                        ResetParent(back);
                        gameControllerRef.clickBlocked = false; // rotation finishes, be able to click again
                    }
                }
 
            }
            else
            {
                if (mainFold != null)
                {
                    mainFold.count--;

                }
                else count--;

                if (mainFold != null)
                {
                    if (mainFold.count == 0) //Reset the layer relationship when the final rotation is done.
                    {
                        Debug.Log("back1");
                        ResetParent(back);
                        gameControllerRef.clickBlocked = false; // rotation finishes, be able to click again
                    }
                }
                else
                {
                    if (count == 0) //Reset the layer relationship when the final rotation is done.
                    {
                        Debug.Log("back2");
                        ResetParent(back);
                        gameControllerRef.clickBlocked = false; // rotation finishes, be able to click again
                    }
                }
            }

        });

        if (mainFold == null)
        {
            if (back == true)
            {
                gameControllerRef.currentOrderIndex--;

                //mainLine.SetActive(true); // return dashed lines back
                //if (touchButton != null)
                    //touchButton.SetActive(true);

                /*for (int i = 0; i < extraLines.Count; i++)
                {
                    extraLines[i].SetPosition(extraLinesIndexes[i], new Vector3(0, 0, extraLinesStartPosition[i]));
                }*/
            }
            else
            {
                gameControllerRef.currentOrderIndex++;

                /*if(gameControllerRef.maxNumberOfRotations == gameControllerRef.currentOrderIndex)
                {
                    gameControllerRef.CheckIfRotatedEverythingCorrectly();
                }*/
            }
        }

       // gameControllerRef.clickBlocked = false; // rotation finishes, be able to click again
    }

    IEnumerator FoldAroundAxis(Transform paperTransform, Vector3 axis, float changeInAngle, Vector3 endPosition, float duration, bool back)
    {
        gameControllerRef.clickBlocked = true;

        Quaternion startRotation = paperTransform.rotation;
        Vector3 startPosition = paperTransform.position;

        float t = 0;

        while (t < duration)
        {
            paperTransform.rotation = startRotation * Quaternion.AngleAxis(changeInAngle * gameControllerRef.rotationCurve.Evaluate(t / duration), axis);
            paperTransform.position = Vector3.Lerp(startPosition, endPosition, t / duration);

            t += Time.deltaTime;
            yield return null;
        }


        paperTransform.rotation = startRotation * Quaternion.AngleAxis(changeInAngle, axis); // endRotation
        paperTransform.position = endPosition; // endPosition

        if (back == true)
        {
            gameControllerRef.currentOrderIndex--;

            //mainLine.SetActive(true); // return dashed lines back

            /*for (int i = 0; i < extraLines.Count; i++)
            {
                extraLines[i].SetPosition(extraLinesIndexes[i], new Vector3(0, 0, extraLinesStartPosition[i]));
            }*/
        }
        else
        {
            gameControllerRef.currentOrderIndex++;

            /*if(gameControllerRef.maxNumberOfRotations == gameControllerRef.currentOrderIndex)
            {
                gameControllerRef.CheckIfRotatedEverythingCorrectly();
            }*/
        }

        gameControllerRef.clickBlocked = false; // rotation finishes, be able to click again

    }

    public Vector3 GetEulerAnglesFromAxisAngle( float angle,Vector3 axis)
    {
        Quaternion rotation = Quaternion.AngleAxis(angle, axis);
        Debug.Log(rotation.eulerAngles);
        return rotation.eulerAngles;
    }

    public void GetAxisAngleFromEuler(Vector3 eulerAngles, out Vector3 axis, out float angle)
    {
        // Change euler into quaternion 
        Quaternion rotation = Quaternion.Euler(eulerAngles);

        // get axis based on angle
        rotation.ToAngleAxis(out angle, out axis);

        Debug.Log(angle+" | "+axis);
    }

    public void ResetParent(bool back)
    {
        if (corners != null)
        {

            foreach (GameObject g in corners)
            {
                if (back)
                {
                    g.transform.SetParent(gameControllerRef.transform, true);
                }
                else
                {
                    if (g.GetComponent<FoldingPaper>() == null)
                        g.transform.SetParent(gameControllerRef.transform, true);
                }
            }
        }

        if (subFoldingCorners != null)
        {
            //reset specific pieces' parents
            /*corners[3].transform.SetParent(null, true);
            corners[4].transform.SetParent(null, true);
            corners[5].transform.SetParent(null, true);
            corners[6].transform.SetParent(null, true);*/

            foreach(GameObject g in subFoldingCorners)
            {
                g.transform.SetParent(gameControllerRef.transform, true);
                g.GetComponent<FoldingPaper>()?.ResetParent(back);
            }

            //reset specific pieces and their children
            /*corners[3].GetComponent<FoldingPaper>()?.ResetParent();
            corners[4].GetComponent<FoldingPaper>()?.ResetParent();
            corners[5].GetComponent<FoldingPaper>()?.ResetParent();
            corners[6].GetComponent<FoldingPaper>()?.ResetParent();*/
        }
        
        if(subFoldingPieces != null)
        {
            foreach (GameObject g in subFoldingPieces)
            {
                g.transform.SetParent(gameControllerRef.transform, true);
                g.GetComponent<FoldingPaper>()?.ResetParent(back);
            }
        }
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        //GetAxisAngleFromEuler(new Vector3(0, 90, 180), out Vector3 axis, out float angle);
    }

    
}
