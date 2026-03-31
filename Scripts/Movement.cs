using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float speed = 6f;
    public float jumpHeight = 2f;
    public float gravity = -20f;
    public float rotationSpeed = 3f;

    [HideInInspector] public Transform cameraTransform;

    private CharacterController _cc;
    private Vector3 _velocity;

    void Start()
    {
        _cc = GetComponent<CharacterController>();
    }

    public void Move(Vector2 input)
    {
        // Movement is relative to camera's flat forward
        Vector3 forward = Vector3.ProjectOnPlane(cameraTransform.forward, Vector3.up).normalized;
        Vector3 right = Vector3.ProjectOnPlane(cameraTransform.right, Vector3.up).normalized;
        Vector3 moveDir = (forward * input.y + right * input.x).normalized;

        _cc.Move(moveDir * speed * Time.deltaTime);

        // Rotate player body to face movement direction
        if (moveDir.sqrMagnitude > 0.01f)
        {
            Quaternion targetRot = Quaternion.LookRotation(moveDir);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotationSpeed * Time.deltaTime);
        }
    }

    public void ApplyJump()
    {
        _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
    }

    void Update()
    {
        bool grounded = _cc.isGrounded;

        if (grounded && _velocity.y < 0)
            _velocity.y = -2f;

        // --- Movement input ---
        Vector2 move = Vector2.zero;

        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) move.x -= 1;
            if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) move.x += 1;
            if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) move.y += 1;
            if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) move.y -= 1;
        }

        if (Gamepad.current != null)
            move += Gamepad.current.leftStick.ReadValue();

        if (move.sqrMagnitude > 1f) move.Normalize();
        Move(move);

        // --- Jump ---
        bool jumpPressed =
            (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame) ||
            (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame);

        if (jumpPressed && grounded)
            ApplyJump();

        // --- Gravity ---
        _velocity.y += gravity * Time.deltaTime;
        _cc.Move(_velocity * Time.deltaTime);
    }
}