using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoldPaper : MonoBehaviour // each paper that can be rotated
{
    public int orderIndex = -1;
    public List<GameObject> corners; // rotate with the main part

    public GameController gameControllerRef;

    public Vector3 roteAxis;
    public float rotationDuration;
    public float angleChange;

    public bool rotated = false;

    public GameObject mainLine; // the horizontal one

    public List<LineRenderer> extraLines; // the vertical ones - those inside of the folding
    public List<int> extraLinesIndexes; // the indexes for the line start point or end point
    public List<float> extraLinesStartPosition; // the original point
    public List<float> extraLinesEndPosition; // the line point after folding

    //public List<int> rotationOrderIndexes;
    //public bool rightOrder = false;

    public void RotatePaper(bool back) // prepare for rotation
    {
        foreach(GameObject g in corners)  // attach corners to the rotation
        {
            g.transform.parent = gameObject.transform;
        }

        if(back == false) // first time rotation
        {
            orderIndex = gameControllerRef.currentOrderIndex;
            gameControllerRef.foldPapers.Add(this);
            StartCoroutine(FoldAroundAxis(gameObject.transform, roteAxis, angleChange, transform.position + new Vector3(0, orderIndex * 0.001f + 0.001f, 0), rotationDuration, back));
            rotated = true;

            mainLine.SetActive(false); // hide the dashed lines after fodling
            for (int i = 0; i < extraLines.Count; i++)
            {
                extraLines[i].SetPosition(extraLinesIndexes[i], new Vector3(0, 0, extraLinesEndPosition[i]));
            }

            /*foreach(int orderIndex in rotationOrderIndexes)
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
            StartCoroutine(FoldAroundAxis(gameObject.transform, roteAxis, -angleChange, new Vector3(gameObject.transform.position.x, 0.003f, gameObject.transform.position.z), rotationDuration, back));
            gameControllerRef.foldPapers.Remove(this);
            rotated = false;

            //rightOrder = false; // fold back, reset the order
        }
    }

    IEnumerator FoldAroundAxis(Transform paperTransform, Vector3 axis, float changeInAngle, Vector3 endPosition, float duration, bool back)
    {
        gameControllerRef.clickBlocked = true;

        Quaternion startRotation = paperTransform.rotation;
        Vector3 startPosition = paperTransform.position;

        float t = 0;

        while (t < duration)
        {
            paperTransform.rotation = startRotation * Quaternion.AngleAxis(changeInAngle * gameControllerRef.rotationCurve.Evaluate(t/duration), axis);
            paperTransform.position = Vector3.Lerp(startPosition, endPosition, t/duration);

            t += Time.deltaTime;
            yield return null;
        }

        paperTransform.rotation = startRotation * Quaternion.AngleAxis(changeInAngle, axis); // endRotation
        paperTransform.position = endPosition; // endPosition

        if(back == true)
        {
            gameControllerRef.currentOrderIndex--;

            mainLine.SetActive(true); // return dashed lines back

            for (int i = 0; i < extraLines.Count; i++)
            {
                extraLines[i].SetPosition(extraLinesIndexes[i], new Vector3(0, 0, extraLinesStartPosition[i]));
            }
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


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
