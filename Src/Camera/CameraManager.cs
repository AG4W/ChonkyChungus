using UnityEngine;
using UnityEngine.Rendering;

using System.Collections;
using UnityEngine.Experimental.Rendering.HDPipeline;

public class CameraManager : MonoBehaviour
{
    [SerializeField]float _translationSpeed;
    [SerializeField]float _rotationSpeed;
    [SerializeField]float _zoomSpeed = .01f;

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

    Camera _camera;

    GameObject _target;

    bool _isMoving = false;

    void Awake()
    {
        _camera = Camera.main;
        _post.profile.TryGet(out _dof);

        GlobalEvents.Subscribe(GlobalEvent.ActorAdded, (object[] args) => 
        {
            if((Actor)args[0] == Player.actor)
                TeleportTo(((Actor)args[0]).transform.position);
        });
        GlobalEvents.Subscribe(GlobalEvent.JumpCameraTo, (object[] args) => 
        {
            MoveTo((Vector3)args[0], (float)args[1]);
        });
        GlobalEvents.Subscribe(GlobalEvent.SetCameraTrackingTarget, SetTrackingTarget);
    }
    void LateUpdate()
    {
        _yRot = Input.GetAxis("Camera Rotation");
        _zoom = Mathf.Clamp(_zoom - Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed, 0f, 1f);

        this.transform.Rotate(0f, _yRot * _rotationSpeed * Time.deltaTime, 0f);

        _camera.transform.localPosition = Vector3.Lerp(_minZoom, _maxZoom, _zoom);
        _camera.transform.localEulerAngles = Vector3.Lerp(_minEuler, _maxEuler, _zoom);
        _dof.focusDistance.value = Vector3.Distance(_camera.transform.position, this.transform.position + Vector3.up * 2);

        if (_isMoving)
            return;

        if(_target == null)
        {
            _x = Input.GetAxis("Horizontal");
            _z = Input.GetAxis("Vertical");

            this.transform.position += (this.transform.forward * _z + this.transform.right * _x) * _translationSpeed * Time.deltaTime;
        }
        else
            this.transform.position = Vector3.Lerp(this.transform.position, _target.transform.position, 2.5f * Time.deltaTime);
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
    void SetTrackingTarget(object[] args)
    {
        if (_isMoving)
            this.StopAllCoroutines();

        _target = args == null ? null : (GameObject)args[0];
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
            this.transform.position = Vector3.Lerp(origin, target, t);
            yield return new WaitForFixedUpdate();
        }

        _isMoving = false;
    }
}
