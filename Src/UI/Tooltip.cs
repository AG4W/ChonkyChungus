using UnityEngine;
using UnityEngine.UI;

public class Tooltip : MonoBehaviour
{
    public static Tooltip instance { get; private set; }

    float _x;
    float _y;

    RectTransform _tooltipRect;
    Vector3 _position;

    [SerializeField]GameObject _tooltip;
    [SerializeField]Text _text;

    void Awake()
    {
        instance = this;

        _tooltipRect = _tooltip.GetComponent<RectTransform>();
        _tooltip.SetActive(false);
    }
    void Update()
    {
        if (_tooltip.activeSelf)
            UpdatePosition();
    }

    public static void Open(string text)
    {
        instance.OpenInternal(text);
    }
    public static void Close()
    {
        instance.CloseInternal();
    }

    void OpenInternal(string text)
    {
        _text.text = text;
        _tooltip.SetActive(true);
    }
    void UpdatePosition()
    {
        _position.x = Mathf.Clamp(Input.mousePosition.x, 0f, Screen.width - _tooltipRect.sizeDelta.x);
        _position.y = Mathf.Clamp(Input.mousePosition.y, _tooltipRect.sizeDelta.y, Screen.height);
        _position.z = Input.mousePosition.z;

        _tooltip.transform.position = _position;
    }
    void CloseInternal()
    {
        _tooltip.SetActive(false);
    }
}
