using UnityEngine;
using UnityEngine.UI;

using System.Collections;

public class PopupEntity : MonoBehaviour
{
    [SerializeField]Text _text;

    float _lifetime;
    float _speed;

    Vector3 _position;
    Camera _camera;

    public void Initialize(string text, float lifetime, float speed, Vector3 position)
    {
        _text.text = text;

        _lifetime = lifetime;
        _speed = speed;

        _position = position;
        _camera = Camera.main;

        StartCoroutine(MovePopup());
    }

    IEnumerator MovePopup()
    {
        float t = 0f;

        while (t <= _lifetime)
        {
            t += Time.deltaTime;
            this.transform.position = _camera.WorldToScreenPoint(_position + (Vector3.up * _speed * t));

            yield return null;
        }

        Destroy(this.gameObject);
    }
}
