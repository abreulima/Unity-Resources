using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;
    public float distance = 8f;
    public float height = 3f;

    void LateUpdate()
    {
        if (target == null) return;

        transform.position = target.position - target.forward * distance + Vector3.up * height;
        transform.LookAt(target.position + Vector3.up * 1f);
    }
}