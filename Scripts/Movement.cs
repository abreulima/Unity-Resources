using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    public float speed = 6f;
    public float jumpHeight = 2f;
    public float gravity = -20f;

    private CharacterController _cc;
    private Vector3 _velocity;

    void Start()
    {
        _cc = GetComponent<CharacterController>();
    }

    void Update()
    {
        bool grounded = _cc.isGrounded;

        if (grounded && _velocity.y < 0)
            _velocity.y = -2f;

        // Movimento
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

        _cc.Move(transform.TransformDirection(new Vector3(move.x, 0, move.y)) * speed * Time.deltaTime);

        // Salto
        bool jumpPressed = (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
                        || (Gamepad.current != null && Gamepad.current.buttonSouth.wasPressedThisFrame);

        if (jumpPressed && grounded)
            _velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // Gravidade
        _velocity.y += gravity * Time.deltaTime;
        _cc.Move(_velocity * Time.deltaTime);
    }
}