using UnityEngine;

public class ShowFirstTap : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        OrigamiController_Ani.Instance.touchPoints[0].gameObject.SetActive(false);

        //Audio_Manager.Instance.Fold_Audio_Play();

        //if()
        //OrigamiSystem.Instance.creaseManager.GenerateCreases(OrigamiController_Ani.Instance.currentOrderIndex-1);
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //if (stateInfo.normalizedTime <= 0f)
        //{
        //    OrigamiController_Ani.Instance.touchPoints[0].gameObject.SetActive(false);
        //}

        

        if (stateInfo.normalizedTime >= 1f)
        {
            if (OrigamiController_Ani.Instance.currentOrderIndex < OrigamiController_Ani.Instance.touchPointsPos.Count)
            {
                OrigamiController_Ani.Instance.touchPoints[0].gameObject.SetActive(true);
                if (OrigamiController_Ani.Instance.isGuessing)
                {
                    for (int i = 1; i < OrigamiController_Ani.Instance.touchPoints.Count; i++)
                    {
                        OrigamiController_Ani.Instance.touchPoints[i].gameObject.SetActive(true);
                    }
                }
            }
            
            //OrigamiSystem.Instance.creaseManager.HideCreases();
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    //override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{  
    //
    //}

    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}
