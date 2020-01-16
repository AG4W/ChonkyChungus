using UnityEngine;

public class IKToggler : StateMachineBehaviour
{
    [SerializeField]AvatarIKGoal _ik;

    bool _shouldToggle;

    //OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _shouldToggle = animator.GetComponent<IKManager>().GetIKStatus(_ik);

        if(_shouldToggle)
            animator.GetComponent<IKManager>().SetIKStatus(_ik, false);
    }
    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_shouldToggle)
            animator.GetComponent<IKManager>().SetIKStatus(_ik, true);
    }
}
