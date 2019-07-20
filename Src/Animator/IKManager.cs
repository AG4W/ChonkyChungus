using UnityEngine;

public class IKManager : MonoBehaviour
{
    [SerializeField]float _rightHandWeight = 1f;
    [SerializeField]float _leftHandWeight = 1f;

    [SerializeField]Transform _rightFoot;
    [SerializeField]Transform _leftFoot;

    Transform _rightHandIKTarget;
    Transform _leftHandIKTarget;

    Vector3 _rightIKPos;
    Vector3 _leftIKPos;

    Animator _animator;

    bool _doLeftHandIK = true;

    void Start()
    {
        _animator = this.GetComponent<Animator>();
    }
    void OnAnimatorIK(int layerIndex)
    {
        if (_rightHandIKTarget != null)
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, _rightHandWeight);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, _rightHandWeight);
            _animator.SetIKPosition(AvatarIKGoal.RightHand, _rightHandIKTarget.position);
            _animator.SetIKRotation(AvatarIKGoal.RightHand, _rightHandIKTarget.rotation);
        }
        else
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
            _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 0);
        }

        if (_leftHandIKTarget != null && _doLeftHandIK)
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, _leftHandWeight);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, _leftHandWeight);
            _animator.SetIKPosition(AvatarIKGoal.LeftHand, _leftHandIKTarget.position);
            _animator.SetIKRotation(AvatarIKGoal.LeftHand, _leftHandIKTarget.rotation);
        }
        else
        {
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 0);
            _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 0);
        }
    }

    public void SetIKTarget(AvatarIKGoal ikTarget, Transform transform)
    {
        switch (ikTarget)
        {
            case AvatarIKGoal.LeftFoot:
                break;
            case AvatarIKGoal.RightFoot:
                break;
            case AvatarIKGoal.LeftHand:
                _leftHandIKTarget = transform;
                break;
            case AvatarIKGoal.RightHand:
                _rightHandIKTarget = transform;
                break;
            default:
                break;
        }
    }

    public void SetIKStatus(AvatarIKGoal ik, bool status)
    {
        switch (ik)
        {
            case AvatarIKGoal.LeftFoot:
                break;
            case AvatarIKGoal.RightFoot:
                break;
            case AvatarIKGoal.LeftHand:
                _doLeftHandIK = status;
                break;
            case AvatarIKGoal.RightHand:
                break;
            default:
                break;
        }
    }
}
