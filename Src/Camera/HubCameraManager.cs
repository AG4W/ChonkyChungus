using UnityEngine;

using System.Collections;

public class HubCameraManager : MonoBehaviour
{
    public static HubCameraManager getInstance { get; private set; }

    [SerializeField]Transform[] _customization;
    [SerializeField]Transform[] _selection;

    Transform[][] _points;

    void Awake()
    {
        _points = new Transform[][] { _customization, _selection };

        getInstance = this;
    }

    public void GoTo(HubCameraMode mode)
    {
        this.StopAllCoroutines();
        StartCoroutine(GoToInternal(_points[(int)mode].Random(), Random.Range(1.5f, 3f)));
    }
    IEnumerator GoToInternal(Transform point, float time)
    {
        float t = 0f;

        while (t <= time)
        {
            t += Time.deltaTime;
            this.transform.position = Vector3.Lerp(this.transform.position, point.position, Mathf.Pow(t / time, 2));
            this.transform.rotation = Quaternion.Slerp(this.transform.rotation, point.rotation, Mathf.Pow(t / time, 2));
            yield return null;
        }
    }
}
public enum HubCameraMode
{
    Customization,
    Selection,
}
