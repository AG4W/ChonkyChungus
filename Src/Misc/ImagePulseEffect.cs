using UnityEngine;
using UnityEngine.UI;

public class ImagePulseEffect : MonoBehaviour
{
    [SerializeField]Image _image;

    void Update()
    {
        if (!_image.gameObject.activeSelf)
            return;

        _image.color = new Color(_image.color.r, _image.color.g, _image.color.b, Mathf.Abs(Mathf.Sin(Time.time)));
    }
}
