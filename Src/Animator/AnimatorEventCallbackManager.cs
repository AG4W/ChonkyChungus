using UnityEngine;

public class AnimatorEventCallbackManager : MonoBehaviour
{
    public void RaiseEvent(AnimationEvent ae)
    {
        OnAnimationEventCalled?.Invoke(ae);
    }

    public delegate void AnimationCallbackEvent(AnimationEvent ae);
    public event AnimationCallbackEvent OnAnimationEventCalled;
}
