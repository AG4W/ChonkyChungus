using UnityEngine;

public class SetRandomOnEnter : StateMachineBehaviour
{
    [SerializeField]string _parameter = "random";

    [SerializeField]int _min;
    [SerializeField]int _max;

    [SerializeField]bool _useFloatingPointPrecision = false;
    [SerializeField]bool _useNetworkedRandom = false;

    float _value;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_useNetworkedRandom)
            _value = _useFloatingPointPrecision ? Synched.Next((float)_min, _max + 1) : Synched.Next(_min, _max + 1);
        else
            _value = _useFloatingPointPrecision ? Random.Range((float)_min, _max + 1) : Random.Range(_min, _max + 1);

        animator.SetFloat(_parameter, _value);
    }
    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    //override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    
    //}
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
