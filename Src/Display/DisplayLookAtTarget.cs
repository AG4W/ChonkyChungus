using UnityEngine;

public class DisplayLookAtTarget : MonoBehaviour
{
    Vector3 _origin;
    Vector3 _targetPosition;

    [SerializeField]float _speed = 5f;

    float _threshold;
    float _timestamp;

    void Awake()
    {
        _origin = this.transform.localPosition;
        _targetPosition = _origin + (Random.insideUnitSphere * 1.5f);
        _threshold = Random.Range(1f, 5f);
        _timestamp = 0f;
    }
    void Update()
    {
        _timestamp += Time.deltaTime;

        if(_timestamp >= _threshold)
        {
            _targetPosition = _origin + (Random.insideUnitSphere * 1.5f);
            _threshold = Random.Range(1f, 5f);
            _timestamp = 0f;
        }

        this.transform.localPosition = Vector3.Lerp(this.transform.localPosition, _targetPosition, _speed * Time.deltaTime);
    }
}
