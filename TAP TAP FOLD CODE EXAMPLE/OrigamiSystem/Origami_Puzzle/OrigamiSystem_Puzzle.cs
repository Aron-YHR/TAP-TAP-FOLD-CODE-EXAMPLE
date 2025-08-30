using TeamNotMuch.UI;
using Unity.VisualScripting;
using UnityEngine;

public class OrigamiSystem_Puzzle : Singleton<OrigamiSystem_Puzzle>
{
    //public GameObject origamiUI;
    public Camera gameplayCamera;
    public Textures_SO textures_SO;
    public OrigamiPatterns_SO origamiPatterns_SO;
    public ScoresAndStars_SO scoresAndStars_SO;
    [SerializeField] private Vector3 canvasResolution;
    public float ratio = 1f;

    [SerializeField] private int currentTextureChoice = 0;
    [SerializeField] private Texture2D currentTexture;
    [SerializeField] private Material customisedMaterial;
    [SerializeField] private Material newMaterial;

    private void Start()
    {
        newMaterial = new( customisedMaterial);
        //Debug.Log(Screen.width+ " "+ Screen.height);
        ratio = ratio / OrigamiController_Puzzle.Instance.uiParent.localScale.x;
        Debug.Log(ratio);

        SelectTexture(0);
    }
    //public GameObject currentOrigami;

    //public UISpline spline;

    private void Update()
    {
        //Debug.Log(Camera.main.WorldToScreenPoint(transform.position));
        //spline.UpdateUISpline(Camera.main.WorldToScreenPoint(transform.position), Input.mousePosition);
    }

    public void SelectOrigami(int ID)
    {
        var origami = LoadOrigami(ID);

        UIManager.Instance.ShowCollectionConfirmScreen(origami.origamiType.ToString(),origami.scoreScreenImg);

    }

    public void OnCreateOrigami()
    {
        //TODO: Do add some checks to ensure the puzzle was loaded properly

        OrigamiController_Puzzle.Instance.InitialiseOrigami();
        UIManager.Instance.ShowGameScreen();
    }

    public Origami_Pattern LoadOrigami(int ID) // load data
    {
        foreach(Origami_Pattern origami_Pattern in origamiPatterns_SO.origamiPatterns)
        {
            if(origami_Pattern.origamiID == ID)
            {
                OrigamiController_Puzzle.Instance.currentSprite = origami_Pattern.scoreScreenImg;

                OrigamiController_Puzzle.Instance.currentOrigamiType = origami_Pattern.origamiType;

                OrigamiController_Puzzle.Instance.origamiModelForEachStepList = origami_Pattern.origamiModelForEachStepList;

                OrigamiController_Puzzle.Instance.animationForEachStepList = origami_Pattern.animationForEachStepList;

                OrigamiController_Puzzle.Instance.puzzleForEachSteps = origami_Pattern.puzzleForEachSteps;

                OrigamiController_Puzzle.Instance.StepsImgList = origami_Pattern.StepsImgList;

                OrigamiController_Puzzle.Instance.tutorial = origami_Pattern.tutorial;

                return origami_Pattern;
            }
        }

        return null;
    }

    public void SelectTexture(int index)
    {
        currentTextureChoice = index;

        currentTexture = textures_SO.textures[index];
        newMaterial.SetTexture("_OverlayTexure", currentTexture);
    }

    public Material GetOverlayMaterial()
    {
        return newMaterial;
    }

    /*public void EnterOrigami()
    {
        origamiUI.SetActive(true);
    }*/

    /*public void ExitOrigami()
    {
        origamiUI.SetActive(false);
        if(OrigamiController_Puzzle.Instance.currentOrigami != null)
            Destroy(OrigamiController_Puzzle.Instance.currentOrigami);
    }*/
}
