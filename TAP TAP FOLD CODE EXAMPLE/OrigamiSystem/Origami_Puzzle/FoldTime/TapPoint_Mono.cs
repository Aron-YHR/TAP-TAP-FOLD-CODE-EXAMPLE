using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(CircleCollider2D))]
public class TapPoint_Mono : MonoBehaviour,IDragHandler,IBeginDragHandler,IEndDragHandler//, IPointerClickHandler
{
    public bool isSelected;
    public bool isHightlighted;

    [SerializeField]SpriteRenderer spriteRenderer;
    

    public Vector3 posOffset;

    public Coroutine currentCoroutine;

    public float transparency = 0.3f;

    //object currentPuzzleState;

    public void HightLightPoint()
    {
        Color c = spriteRenderer.color;
        c.a = 1f;
        spriteRenderer.color = c;

        isHightlighted = true;
        EventSender.SendSFXChange(MusicType.Tap_Point_Connected);
        Debug.Log("Snap");
    }

    public void LowLightPoint()
    {
        if (!isSelected)
        {
            Color c = spriteRenderer.color;
            c.a = transparency;
            spriteRenderer.color = c;
            isHightlighted = false;
        }
    }

    private void OnMouseDown()
    {
        var currentPuzzleState = OrigamiController_Puzzle.Instance.GetComponent<FoldTimeState>();

        if (currentPuzzleState.selectFoldPointsTime)
            SelectedThisPoint(true);
    }

    public void SelectedThisPoint(bool selected)
    {
        //if(isSelected) return;
        

        var currentPuzzleState = OrigamiController_Puzzle.Instance.GetComponent<FoldTimeState>();

        

        if (currentPuzzleState.selectFoldPointsTime)
        {
            isSelected = selected;
            HightLightPoint();
            currentPuzzleState.selectedCount++;

            //if(currentPuzzleState.selectedButtons.Count > 0 && currentPuzzleState.selectedButtons.Count % 2 == 0)//(currentPuzzleState.selectedButtons.Count == currentPuzzleState.foldTimeAnswer.pointChoices.Count)
            if (currentPuzzleState.selectedCount > 0 && currentPuzzleState.selectedCount % 2 == 0)
            {
                //currentPuzzleState.consistSpline.gameObject.SetActive(true);
                //currentPuzzleState.consistSpline.UpdateUISpline(OrigamiSystem_Puzzle.Instance.gameplayCamera.WorldToScreenPoint(currentPuzzleState.selectedButtons[0].transform.position), OrigamiSystem_Puzzle.Instance.gameplayCamera.WorldToScreenPoint(currentPuzzleState.selectedButtons[1].transform.position));

                //Create ui spline using tapping
                //if(currentCoroutine != null)
                currentPuzzleState.selectedButtons[^1].StopCurrentCoroutine();

                //currentPuzzleState.CreateSpline(OrigamiSystem_Puzzle.Instance.gameplayCamera.WorldToScreenPoint(OrigamiController_Puzzle.Instance.origamiSpawnPos.TransformPoint(currentPuzzleState.selectedButtons[0].posOffset)), OrigamiSystem_Puzzle.Instance.gameplayCamera.WorldToScreenPoint(OrigamiController_Puzzle.Instance.origamiSpawnPos.TransformPoint(currentPuzzleState.selectedButtons[1].posOffset)));

                Vector3 endPos = OrigamiSystem_Puzzle.Instance.gameplayCamera.WorldToScreenPoint(OrigamiController_Puzzle.Instance.origamiSpawnPos.TransformPoint(posOffset));

                endPos = endPos * OrigamiSystem_Puzzle.Instance.ratio;

                currentPuzzleState.uISplineList[^1].UpdateUISpline(currentPuzzleState.uISplineList[^1].GetStartAndEndPoint()[0], endPos);

                //currentPuzzleState.uISplineList[^1].startOffset = currentPuzzleState.selectedButtons[0].posOffset;
                currentPuzzleState.uISplineList[^1].endOffest = posOffset;//currentPuzzleState.selectedButtons[1].posOffset;

                //show spline
                currentPuzzleState.uISplineList[^1].enabled = true;
                currentPuzzleState.uISplineList[^1].GetComponentInChildren<Image>().enabled = true;

                //currentPuzzleState.selectFoldPointsTime=false;

                /*for (int i = 0; i < currentPuzzleState.selectedButtons.Count; i++)
                {

                        //currentPuzzleState.selectedButtons[i].gameObject.SetActive(false);

                }*/

                if(currentPuzzleState.uISplineList[^1].startOffset == currentPuzzleState.uISplineList[^1].endOffest)
                {
                    ResetPoint();


                    //int index = currentPuzzleState.uISplineList.Count - 1;
                    Destroy(currentPuzzleState.uISplineList[^1].gameObject);
                    currentPuzzleState.uISplineList.Remove(currentPuzzleState.uISplineList[^1]);

                    EventSender.SendTapMistake();
                    

                    return;
                }

                if(currentPuzzleState.uISplineList.Count >= currentPuzzleState.foldTimeAnswer.linePos.Count)
                    currentPuzzleState.selectFoldPointsTime = false;

                StartCoroutine(FinishOneLine(currentPuzzleState));
            }
            else
            {
                currentPuzzleState.GetComponent<FoldTimeState>().selectedButtons.Add(this);
               

                Vector3 start = OrigamiSystem_Puzzle.Instance.gameplayCamera.WorldToScreenPoint(OrigamiController_Puzzle.Instance.origamiSpawnPos.TransformPoint(currentPuzzleState.selectedButtons[^1].posOffset));
                Debug.Log(start);
                currentPuzzleState.CreateSpline(start, start);

                currentPuzzleState.uISplineList[^1].startOffset = currentPuzzleState.selectedButtons[^1].posOffset;
                currentPuzzleState.uISplineList[^1].enabled = false;

                currentCoroutine = StartCoroutine(FollowMouse(currentPuzzleState));
            }
        }
    }

    IEnumerator FollowMouse(FoldTimeState currentPuzzleState)
    {
        while (true)
        {
            Vector3 mousePos = Input.mousePosition;

            //mousePos = 

            currentPuzzleState.uISplineList[^1].UpdateUISpline(currentPuzzleState.uISplineList[^1].GetStartAndEndPoint()[0], mousePos * OrigamiSystem_Puzzle.Instance.ratio);

            yield return null;
        }
    }

    IEnumerator FinishOneLine(FoldTimeState currentPuzzleState)
    {
        yield return new WaitForSeconds(0.5f);
        currentPuzzleState.OnPuzzleAction();
        
    }

    public void StopCurrentCoroutine()
    {
        StopCoroutine(currentCoroutine);
    }

    public void ResetPoint()
    {
        Color c = spriteRenderer.color;
        c.a = transparency;
        spriteRenderer.color = c;
        isHightlighted = false;
        isSelected = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (Input.touchCount > 1) return;

        var currentPuzzleState = OrigamiController_Puzzle.Instance.GetComponent<FoldTimeState>();
        if (currentPuzzleState.drawLineTime)
        {

            Vector3 vector3 = WorldPosConverter(eventData.position);
            //Debug.Log(eventData.position);
            //Debug.Log(vector3 - OrigamiController_Puzzle.Instance.origamiSpawnPos.transform.position);
            //Line renderer adjustment
            vector3 = OrigamiController_Puzzle.Instance.origamiSpawnPos.transform.InverseTransformPoint(vector3) - OrigamiController_Puzzle.Instance.origamiSpawnPos.transform.localPosition;
            vector3.z = 0;

            //Debug.Log(vector3);

            for (int i = 0; i < currentPuzzleState.touch_Point_Positions.Count; i++)
            {
                if (Vector3.Distance(vector3, currentPuzzleState.touch_Point_Positions[i]) < currentPuzzleState.minDistance)
                {
                    vector3 = currentPuzzleState.touch_Point_Positions[i];

                    //change transparency
                    if (!currentPuzzleState.touchPoints[i].isHightlighted)
                        currentPuzzleState.touchPoints[i].HightLightPoint();

                }
                else
                {
                    if (currentPuzzleState.touchPoints[i] != this)
                        currentPuzzleState.touchPoints[i].LowLightPoint();
                }
            }
                //currentPuzzleState.lineRendererList[^1].SetPosition(1, vector3);

                //adjust uISpline

                currentPuzzleState.uISplineList[^1].endOffest = vector3;

                vector3 = OrigamiSystem_Puzzle.Instance.gameplayCamera.WorldToScreenPoint(OrigamiController_Puzzle.Instance.origamiSpawnPos.transform.TransformPoint(vector3));

                //vector3 = 

                currentPuzzleState.uISplineList[^1].UpdateUISpline(currentPuzzleState.uISplineList[^1].GetStartAndEndPoint()[0], vector3 * OrigamiSystem_Puzzle.Instance.ratio);
                
            
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (Input.touchCount > 1) return;

        //Debug.Log(eventData.pointerCurrentRaycast);
        var currentPuzzleState = OrigamiController_Puzzle.Instance.GetComponent<FoldTimeState>();
        Debug.Log("BeginDrag");

        if (currentPuzzleState.drawLineTime)
        {
            
            HightLightPoint();
            isSelected = true;

            EventSender.SendSFXChange(MusicType.Tap_Point_Pressed);

            //create line renderer
            /*Vector3 vector3 = posOffset;
            currentPuzzleState.CreateCreases(vector3);*/

            //Create ui spline
            Vector3 vector31 = OrigamiController_Puzzle.Instance.origamiSpawnPos.TransformPoint(posOffset);

            vector31 = OrigamiSystem_Puzzle.Instance.gameplayCamera.WorldToScreenPoint(vector31);

            currentPuzzleState.CreateSpline(vector31,vector31);
            currentPuzzleState.uISplineList[^1].startOffset = posOffset;
            currentPuzzleState.uISplineList[^1].GetComponentInChildren<Image>().enabled = true;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        Debug.Log("EndDrag");

        if (Input.touchCount > 1) return;

        var currentPuzzleState = OrigamiController_Puzzle.Instance.GetComponent<FoldTimeState>();

        if (currentPuzzleState.drawLineTime)
        {

            Vector3 vector3 = WorldPosConverter(eventData.position);
            bool isValidLine = false;

            vector3 = OrigamiController_Puzzle.Instance.origamiSpawnPos.transform.InverseTransformPoint(vector3) - OrigamiController_Puzzle.Instance.origamiSpawnPos.transform.localPosition;
            

            for (int i = 0; i < currentPuzzleState.touch_Point_Positions.Count; i++)
            {
                if (Vector3.Distance(vector3, currentPuzzleState.touch_Point_Positions[i]) < currentPuzzleState.minDistance)
                {
                    if (currentPuzzleState.touchPoints[i].posOffset == currentPuzzleState.uISplineList[^1].startOffset)
                    {
                        isValidLine = false; 
                        break;
                    }
                    isValidLine = true;

                }
            }

            for (int i = 0; i < currentPuzzleState.uISplineList.Count-1; i++)
            {
                if(currentPuzzleState.uISplineList[^1].startOffset == currentPuzzleState.uISplineList[i].startOffset && currentPuzzleState.uISplineList[^1].endOffest == currentPuzzleState.uISplineList[i].endOffest)
                {
                    isValidLine = false;
                    break;
                }
            }

            if (!isValidLine)
            {
                //currentPuzzleState.ResetPuzzle(true);
                ResetPoint();

                // remove invalid crease line
                /*int index = currentPuzzleState.lineRendererList.Count - 1;
                Destroy(currentPuzzleState.lineRendererList[index].gameObject);
                currentPuzzleState.lineRendererList.Remove(currentPuzzleState.lineRendererList[index]);*/
                //Destroy(lineG);
                if (currentPuzzleState.uISplineList.Count > 0)
                //int index = currentPuzzleState.uISplineList.Count - 1;
                {
                    Destroy(currentPuzzleState.uISplineList[^1].gameObject);
                    currentPuzzleState.uISplineList.Remove(currentPuzzleState.uISplineList[^1]);
                }

                EventSender.SendTapMistake();
                

                return;
            }

            for (int i = 0; i < currentPuzzleState.touchPoints.Count; i++)
            {
                if (currentPuzzleState.touchPoints[i].isHightlighted)
                {
                    
                    //currentPuzzleState.touchPoints[i].SelectedThisPoint(true);
                    //currentPuzzleState.touchPoints[i].isSelected = true;

                    //currentPuzzleState.touchPoints[i].gameObject.SetActive(false);

                    currentPuzzleState.GetComponent<FoldTimeState>().selectedButtons.Add(this);
                }
            }
            currentPuzzleState.OnPuzzleAction();
        }
    }

    public Vector3 WorldPosConverter(Vector3 screenPos)
    {
        screenPos.z = Mathf.Abs(OrigamiSystem_Puzzle.Instance.gameplayCamera.transform.position.z - OrigamiController_Puzzle.Instance.origamiSpawnPos.transform.position.z);
        Vector3 vector3 = OrigamiSystem_Puzzle.Instance.gameplayCamera.ScreenToWorldPoint(screenPos);

        //vector3.z = 0;
        return vector3;
    }

    //public Vector3 SetUIFollowWorld(Vector3 target, Canvas canvas)
    //{
    //    /*Vector3 screenPos = OrigamiSystem_Puzzle.Instance.gameplayCamera.WorldToScreenPoint(target);
    //    Debug.Log(screenPos);
    //    Vector2 localPoint;
    //    RectTransformUtility.ScreenPointToLocalPointInRectangle(
    //        canvas.transform as RectTransform,
    //        screenPos,
    //        canvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : canvas.worldCamera,
    //        out localPoint);*/

    //    Vector2 localPoint = RectTransformUtility.WorldToScreenPoint(OrigamiSystem_Puzzle.Instance.gameplayCamera,target);

    //    Debug.Log(localPoint);
    //    return (Vector3)localPoint;
    //}

}


