using System.Collections;
using UnityEngine;

public class AnimationLooper : MonoBehaviour
{
    public Animator animator;
    public string stateName;
    public float waitTime = 2f;
    private Coroutine loopCoroutine;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        loopCoroutine = StartCoroutine(PlayLoop());
    }

    IEnumerator PlayLoop()
    {
        while (true)
        {
            //Debug.Log(1);
            animator.Play(stateName, 0,0f);

            yield return new WaitForSeconds(animator.GetCurrentAnimatorStateInfo(0).length);

            yield return new WaitForSeconds(waitTime);
        }
    }

    public void StartLoopFromBeginning()
    {
        StopCoroutine(loopCoroutine);
        loopCoroutine = StartCoroutine(PlayLoop());
    }
}
