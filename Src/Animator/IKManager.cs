using UnityEngine;

public class IKManager : MonoBehaviour
{
    [SerializeField]float _rightHandWeight = 1f;
    [SerializeField]float _leftHandWeight = 1f;

    [SerializeField]Transform _rightFoot;
    [SerializeField]Transform _leftFoot;

    float[] _weights = new float[5];

    Transform _rightHandIKTarget;
    Transform _leftHandIKTarget;

    Vector3 _rightIKPos;
    Vector3 _leftIKPos;

    Vector3 _lookAtPos;

    Animator _animator;

    bool[] _ikStatus;
    bool _lookAtStatus = false;

    void Awake()
    {
        _ikStatus = new bool[4];
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

        if (_leftHandIKTarget != null && this.GetIKStatus(AvatarIKGoal.LeftHand))
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

        if (_lookAtStatus)
        {
            _animator.SetLookAtPosition(_lookAtPos);
            _animator.SetLookAtWeight(_weights[0], _weights[1], _weights[2], _weights[3], _weights[4]);
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
    public void SetLookAtTarget(Vector3 target, float weight, float body = .2f, float head = 1f, float eyes = 1f, float clamp = 1f)
    {
        _lookAtPos = target;

        _weights[0] = weight;
        _weights[1] = body;
        _weights[2] = head;
        _weights[3] = eyes;
        _weights[4] = clamp;
    }

    public bool GetIKStatus(AvatarIKGoal ik)
    {
        return _ikStatus[(int)ik];
    }
    public void SetIKStatus(AvatarIKGoal ik, bool status)
    {
        _ikStatus[(int)ik] = status;
    }

    public void SetLookAtStatus(bool status)
    {
        _lookAtStatus = status;
    }
}
