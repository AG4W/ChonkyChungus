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

    Vector3 _cameraOffset;
    Camera _camera;

    GameObject _target;

    bool _isMoving = false;
    bool _inDynamicMode = false;

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
            MoveTo((Vector3)args[0], args.Length < 2 ? 5f : (float)args[1]);
        });
        GlobalEvents.Subscribe(GlobalEvent.SetCameraTrackingTarget, SetTrackingTarget);
        GlobalEvents.Subscribe(GlobalEvent.ExitDynamicMode, ExitDynamicMode);
        GlobalEvents.Subscribe(GlobalEvent.CutToCameraTargeteeTargetShot, CreateTargeteeTargetShot);
    }
    void LateUpdate()
    {
        if (_inDynamicMode)
        {

        }
        else
        {
            _yRot = Input.GetAxis("Camera Rotation");
            _zoom = Mathf.Clamp(_zoom - Input.GetAxis("Mouse ScrollWheel") * _zoomSpeed, 0f, 1f);

            this.transform.Rotate(0f, _yRot * _rotationSpeed * Time.deltaTime, 0f);

            _camera.transform.localPosition = Vector3.Lerp(_minZoom, _maxZoom, _zoom);
            _camera.transform.localEulerAngles = Vector3.Lerp(_minEuler, _maxEuler, _zoom);
            _dof.focusDistance.value = Vector3.Distance(_camera.transform.position, this.transform.position + Vector3.up * 2);

            if (_isMoving)
                return;

            if (_target == null)
            {
                _x = Input.GetAxis("Horizontal");
                _z = Input.GetAxis("Vertical");

                this.transform.position += (this.transform.forward * _z + this.transform.right * _x) * _translationSpeed * Time.deltaTime;
            }
            else
                this.transform.position = Vector3.Lerp(this.transform.position, _target.transform.position, 2.5f * Time.deltaTime);
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
    void SetTrackingTarget(object[] args)
    {
        if (_isMoving)
            this.StopAllCoroutines();

        _target = args == null ? null : (GameObject)args[0];
    }

    void CreateTargeteeTargetShot(object[] args)
    {
        if (_inDynamicMode)
            ExitDynamicMode();

        Entity targetee = args[0] as Entity;
        Entity target = args[1] as Entity;

        Vector3 heading = (target.tile.position - targetee.tile.position).normalized;

        //create shot offsets and pick a random lerp between them
        Vector3 offsetLeft = targetee.transform.right.normalized * 2.25f;
        Vector3 offsetRight = -targetee.transform.right.normalized * 2.25f;
        Vector3 offset = Vector3.Lerp(offsetLeft, offsetRight, Random.Range(0f, 1f));

        Vector3 cameraPos = targetee.tile.position + (heading * -Random.Range(2.5f, 3f)) + offset;
        Vector3 cameraHeading = (target.tile.position - cameraPos).normalized;

        _inDynamicMode = true;
        _cameraOffset = _camera.transform.localPosition;
        _camera.transform.localPosition = new Vector3(0f, 1.5f, 0f);
        _camera.transform.localEulerAngles = Vector3.zero;

        StartCoroutine(JigLookAt(cameraPos, Quaternion.LookRotation(cameraHeading, Vector3.up), 1f));
    }
    void ExitDynamicMode(params object[] args)
    {
        _camera.transform.localPosition = _cameraOffset;
        _camera.transform.localEulerAngles = new Vector3(45f, 0f, 0f);

        _inDynamicMode = false;
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

    IEnumerator JigLookAt(Vector3 position, Quaternion rotation, float duration)
    {
        Vector3 originPos = this.transform.position;
        Quaternion originRot = this.transform.rotation;
        float t = 0f;

        while (t <= duration)
        {
            t += Time.fixedDeltaTime;
            this.transform.position = Vector3.Lerp(originPos, position, (t / duration).CubicEaseOut());
            this.transform.rotation = Quaternion.Lerp(originRot, rotation, (t / duration).CubicEaseOut());
            yield return new WaitForFixedUpdate();
        }
    }
}
