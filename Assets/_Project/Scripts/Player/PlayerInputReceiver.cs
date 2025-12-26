using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInputReceiver : MonoBehaviour
{
    public Vector2 Move { get; private set; }
    public Vector2 LookDelta { get; private set; }
    public bool JumpPressed { get; private set; }

    private PlayerInputActions _inputActions;

    void Awake()
    {
        _inputActions = new PlayerInputActions();
        _inputActions.Player.Move.performed += ctx => Move = ctx.ReadValue<Vector2>();
        _inputActions.Player.Move.canceled += _ => Move = Vector2.zero;
        _inputActions.Player.Look.performed += ctx => LookDelta = ctx.ReadValue<Vector2>();
        _inputActions.Player.Jump.performed += _ => JumpPressed = true;
    }

    void OnEnable() => _inputActions?.Player.Enable();
    void OnDisable() => _inputActions?.Player.Disable();

    void OnDestroy() => _inputActions?.Dispose();

    public void ConsumeJump() => JumpPressed = false;
    public void ResetLookThisFrame() => LookDelta = Vector2.zero;
}