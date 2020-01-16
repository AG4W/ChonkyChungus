using UnityEngine;

[ExecuteInEditMode]
public class AnimateInEditorView : MonoBehaviour
{
    [SerializeField]float _speedDivider = .5f;

    [SerializeField]Vector3 _a;
    [SerializeField]Vector3 _b;

    // Update is called once per frame
    void Update()
    {
        this.transform.position = Vector3.Lerp(_a, _b, Mathf.Sin(Time.time) / _speedDivider);
    }
}
