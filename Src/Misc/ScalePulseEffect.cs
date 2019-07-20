using UnityEngine;

public class ScalePulseEffect : MonoBehaviour
{
    [SerializeField]float _intensity = .25f;
    [SerializeField]float _speed = .5f;
    float _originalSpeed;

    [SerializeField]int _step = 4;

    [SerializeField]IntegerDirection _direction = IntegerDirection.Negative;

    Vector3 _originalScale;
    Vector3 _targetScale;

    float _t;

    public float speed { get { return _speed; } }

    void Start()
    {
        _originalScale = this.transform.localScale;
        _originalSpeed = _speed;

        _targetScale = _originalScale * (1 + (_direction == IntegerDirection.Negative ? -_intensity : _intensity));
    }
    void Update()
    {
        _t = Mathf.Abs(Mathf.Sin(Time.time * _speed));

        this.transform.localScale = Vector3.Lerp(_originalScale, _targetScale, Mathf.Pow(_t, _step));
    }

    public void OnInteractable()
    {
        _speed = _originalSpeed * 2f;
    }
    public void OnTraversable()
    {
        _speed = _originalSpeed;
    }
}
