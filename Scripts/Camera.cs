using UnityEngine;
using UnityEngine.InputSystem;

public class CameraController : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Orbit")]
    public float distance = 6f;
    public float height = 2f;
    public float mouseSensitivity = 0.25f;
    public float joystickSensitivity = 90f;

    [Header("Pitch limits")]
    public float minPitch = -20f;
    public float maxPitch = 60f;

    [Header("Smoothing")]
    public float positionLag = 4f;
    public float rotationLag = 6f;

    private float _yawOffset;             // offset ON TOP of the player's rotation
    private float _pitch = 15f;

    private Vector3 _currentPosition;
    private Quaternion _currentRotation;

    void Start()
    {
        _yawOffset = 0f;
        UpdateOrbit(instant: true);

        if (target != null)
        {
            var pc = target.GetComponent<PlayerController>();
            if (pc != null) pc.cameraTransform = transform;
        }

        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
    }

    void LateUpdate()
    {
        Vector2 look = Vector2.zero;

        if (Mouse.current != null && Mouse.current.leftButton.isPressed)
            look += Mouse.current.delta.ReadValue() * mouseSensitivity;

        if (Gamepad.current != null)
            look += Gamepad.current.rightStick.ReadValue() * joystickSensitivity * Time.deltaTime;

        _yawOffset += look.x;
        _pitch -= look.y;
        _pitch = Mathf.Clamp(_pitch, minPitch, maxPitch);

        UpdateOrbit(instant: false);
    }

    void UpdateOrbit(bool instant)
    {
        if (target == null) return;

        // Base yaw = player's current Y rotation + any manual offset
        float yaw = target.eulerAngles.y + _yawOffset;

        Quaternion orbitRot = Quaternion.Euler(_pitch, yaw, 0f);
        Vector3 offset = orbitRot * new Vector3(0f, 0f, -distance);
        Vector3 desiredPos = target.position + Vector3.up * height + offset;
        Quaternion desiredRot = Quaternion.LookRotation(
            (target.position + Vector3.up * height) - desiredPos
        );

        if (instant)
        {
            _currentPosition = desiredPos;
            _currentRotation = desiredRot;
        }
        else
        {
            _currentPosition = Vector3.Lerp(_currentPosition, desiredPos, positionLag * Time.deltaTime);
            _currentRotation = Quaternion.Slerp(_currentRotation, desiredRot, rotationLag * Time.deltaTime);
        }

        transform.position = _currentPosition;
        transform.rotation = _currentRotation;
    }
}