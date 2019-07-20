using UnityEngine;

public class AnimationTester : MonoBehaviour
{
    [SerializeField]Animator _a;
    [SerializeField]Animator _b;

    void Update()
    {
        if(Input.GetKeyDown(KeyCode.Space))
        {
            _a.SetTrigger("attack");
            _b.SetTrigger("defend");
        }
    }
}
