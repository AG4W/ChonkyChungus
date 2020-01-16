using UnityEngine;

public class DisplayLightsController : MonoBehaviour
{
    [SerializeField]float _rotationSpeed = 5f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.Mouse1))
            this.transform.eulerAngles += new Vector3(0f, Input.GetAxisRaw("Mouse X") * _rotationSpeed * Time.fixedDeltaTime, 0f);
    }
}
