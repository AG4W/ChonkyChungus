using UnityEngine;

public class DisplayIKManager : MonoBehaviour
{
    Animator _animator;

    GameObject _lookAt;

    void Awake()
    {
        _lookAt = GameObject.Find("lookAt");
        _animator = this.GetComponent<Animator>();
    }
    void OnAnimatorIK(int layerIndex)
    {
        _animator.SetLookAtPosition(_lookAt.transform.position);
        _animator.SetLookAtWeight(1f, 0f, .5f, 1f, .4f);
    }
}
