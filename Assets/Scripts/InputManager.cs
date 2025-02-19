using UnityEngine;
using UnityEngine.InputSystem;

public class InputManager : MonoBehaviour
{
    private PlayerInput playerinput;

    public static Vector3 Movement;
    
    public static bool JumpWasPressed;
    public static bool JumpIsHeld;
    public static bool JumpWasReleased;

    public static bool ShootWasPressed;

    private InputAction _moveAction;
    private InputAction _jumpAction;
    private InputAction _shootAction;

    private void Awake()
    {
        playerinput = GetComponent<PlayerInput>();

        _moveAction = playerinput.actions["Move"];
        _jumpAction = playerinput.actions["Jump"];
        _shootAction = playerinput.actions["Fire"];
    }

    private void FixedUpdate()
    {
        Movement = _moveAction.ReadValue<Vector2>();

        JumpWasPressed = _jumpAction.WasPressedThisFrame();
        JumpIsHeld = _jumpAction.IsPressed();
        JumpWasReleased = _jumpAction.WasReleasedThisFrame();

        ShootWasPressed = _shootAction.WasPressedThisFrame();
    }
}
