using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    [SerializeField]GameObject _root;

    Text _info;
    RectTransform _icon;
    Actor _target;

    Camera _camera;

    void Awake()
    {
        _camera = Camera.main;

        _icon = _root.transform.Find("icon").GetComponent<RectTransform>();
        _info = _root.transform.Find("info").GetComponent<Text>();
        _root.SetActive(false);

        GlobalEvents.Subscribe(GlobalEvent.ShowCrosshair, Show);
        GlobalEvents.Subscribe(GlobalEvent.HideCrosshair, Hide);
    }
    void Update()
    {
        _icon.transform.eulerAngles += new Vector3(0f, 0f, -1f) * 10f * Time.deltaTime;

        //Entities should probably define some kind of "GetVisualCenter()" or "GetCrosshairPoint()"
        if(_target != null)
            _root.transform.position = _camera.WorldToScreenPoint(_target.transform.position + Vector3.up);
    }

    void Show(object[] args)
    {
        ActionContext context = args[0] as ActionContext;

        _target = args[1] as Actor;

        _info.text = _target.data.name + "\n\n" + context.action.ToString() + "\n\n" + "<i><color=grey>bestiary snippet here</color></i>";
        _root.SetActive(true);
    }
    void Hide(object[] args)
    {
        _target = null;
        _root.SetActive(false);
    }
}
