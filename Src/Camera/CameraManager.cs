using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.HighDefinition;

using System.Collections;

public class CameraManager : MonoBehaviour
{
    [SerializeField]float _translationSpeed;
    [SerializeField]float _rotationSpeed;
    [SerializeField]float _zoomSpeed = .01f;

    [SerializeField]float _dofOffset = 2f;

    [SerializeField]Vector3 _maxZoom = new Vector3(0, 15, -15);
    [SerializeField]Vector3 _minZoom = new Vector3(0, 5, -5);

    [SerializeField]Vector3 _maxEuler = new Vector3(25, 0, 0);
    [SerializeField]Vector3 _minEuler = new Vector3(65, 0, 0);

    [SerializeField]Volume _post;
    DepthOfField _dof;

    float _x;
    float _z;

    float _yRot;
    float _zoom = .5f;

    Camera _mainCamera;

    [SerializeField]bool _isMoving = false;
    [SerializeField]bool _inTargetingMode = false;

    void Awake()
    {
        _mainCamera = Camera.main;
        _post.profile.TryGet(out _dof);

        GlobalEvents.Subscribe(GlobalEvent.ActorAdded, (object[] args) => 
        {
            if((Actor)args[0] == Player.selectedActor)
                TeleportTo(((Actor)args[0]).transform.position);
        });
        GlobalEvents.Subscribe(GlobalEvent.JumpCameraTo, (object[] args) => 
        {
            MoveTo((Vector3)args[0], args.Length < 2 ? 7.5f : (float)args[1]);
        });

        GlobalEvents.Subscribe(GlobalEvent.EnterTargetingMode, (object[] args) => _inTargetingMode = true);
        GlobalEvents.Subscribe(GlobalEvent.ExitTargetingMode, (object[] args) => _inTargetingMode = false);
    }
    void Update()
    {
        UpdateFocusDistance();

        _yRot = Input.GetAxis("Camera Rotation");
        _zoom = Mathf.Clamp(_zoom - Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed, 0f, 1f);

        this.transform.Rotate(0f, _yRot * _rotationSpeed * Time.deltaTime, 0f);

        _mainCamera.transform.localPosition = Vector3.Lerp(_minZoom, _maxZoom, _zoom);
        _mainCamera.transform.localEulerAngles = Vector3.Lerp(_minEuler, _maxEuler, _zoom);

        if (_isMoving)
            return;

        if (_inTargetingMode)
        {
        }
        else
        {
            _x = Input.GetAxis("Horizontal") * _translationSpeed;
            _z = Input.GetAxis("Vertical") * _translationSpeed;

            this.transform.position += (this.transform.right * _x + this.transform.forward * _z) * Time.deltaTime;
        }
    }

    void TeleportTo(Vector3 target)
    {
        if (_isMoving)
            this.StopAllCoroutines();

        this.transform.position = target;
    }
    void MoveTo(Vector3 target, float speed)
    {
        if (_isMoving)
            this.StopAllCoroutines();

        StartCoroutine(MoveToTarget(target, speed));
    }

    void UpdateFocusDistance()
    {
        _dof.focusDistance.value = Vector3.Distance(_mainCamera.transform.position, this.transform.position) - _dofOffset;
    }

    IEnumerator MoveToTarget(Vector3 target, float speed)
    {
        _isMoving = true;

        Vector3 origin = this.transform.position;
    
        float s = (speed / (origin - target).magnitude * Time.fixedDeltaTime);
        float t = 0f;

        while (t <= 1f)
        {
            t += s;
            this.transform.position = Vector3.Lerp(origin, target, t * t * t);
            yield return new WaitForFixedUpdate();
        }

        _isMoving = false;
    }
}
