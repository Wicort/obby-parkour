using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Animator _animator;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float _movementSpeed = 5f;
    [SerializeField, Min(0f)] private float _jumpHeight = 4f;
    [SerializeField, Min(0f)] private float _groundedBufferSeconds = 0.15f;

    [Header("Camera")]
    [SerializeField, Range(0.01f, 1f)] private float _mouseSensitivity = 0.15f;
    [SerializeField] private Vector2 _verticalAngleLimits = new Vector2(-60f, 60f);

    private static readonly int StrafeHash = Animator.StringToHash("Strafe");
    private static readonly int ForwardHash = Animator.StringToHash("Forward");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");

    private CharacterController _characterController;
    private PlayerInputReceiver _inputReceiver;

    private float _verticalRotation;
    private float _verticalVelocity;
    private float _timeSinceLastGrounded;
    private Vector3 _externalImpulse = Vector3.zero;

    public bool IsGrounded => _characterController.isGrounded;
    public Transform CameraTransform => _cameraTransform;

    public void ApplyImpulse(Vector3 impulse)
    {
        _verticalVelocity = impulse.y;
        _externalImpulse += new Vector3(impulse.x, 0f, impulse.z);
    }

    void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _inputReceiver = GetComponent<PlayerInputReceiver>();
        if (_inputReceiver == null)
        {
            Debug.LogError("PlayerMovement requires PlayerInputReceiver on the same GameObject!");
            enabled = false;
            return;
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (_characterController.isGrounded)
            _timeSinceLastGrounded = 0f;
        else
            _timeSinceLastGrounded += Time.deltaTime;

        HandleMovement();
        HandleCamera();
        UpdateAnimator();

        _inputReceiver.ConsumeJump();
        _inputReceiver.ConsumeDash();
        _inputReceiver.ResetLookThisFrame();
    }

    void HandleMovement()
    {
        // Горизонтальное движение (без изменений)
        Vector3 horizontalMovement = Vector3.zero;
        if (_inputReceiver.Move != Vector2.zero && _cameraTransform != null)
        {
            Vector3 forward = _cameraTransform.forward;
            Vector3 right = _cameraTransform.right;
            forward.y = right.y = 0f;
            forward.Normalize();
            right.Normalize();

            Vector3 moveDirection = forward * _inputReceiver.Move.y + right * _inputReceiver.Move.x;
            if (moveDirection.magnitude > 1f)
                moveDirection.Normalize();

            horizontalMovement = moveDirection * _movementSpeed * Time.deltaTime;
        }

        horizontalMovement += _externalImpulse;
        _externalImpulse = Vector3.zero;

        // === ПРЫЖОК ===
        bool canJump = _characterController.isGrounded ||
                       _timeSinceLastGrounded <= _groundedBufferSeconds;

        if (_inputReceiver.JumpPressed && canJump)
        {
            // Резкий взлёт: высокая начальная скорость
            _verticalVelocity = Mathf.Sqrt(2f * _jumpHeight * Mathf.Abs(Physics.gravity.y));
            _timeSinceLastGrounded = float.MaxValue;
        }

        // === ГРАВИТАЦИЯ ===
        // Увеличенная гравитация для резкого падения
        const float enhancedGravityMultiplier = 2.0f; // ← КЛЮЧЕВОЙ ПАРАМЕТР
        float gravity = Physics.gravity.y * enhancedGravityMultiplier;

        _verticalVelocity += gravity * Time.deltaTime;

        // Сброс скорости при приземлении
        if (_characterController.isGrounded && _verticalVelocity < 0f)
            _verticalVelocity = 0f;

        // Вертикальное перемещение
        Vector3 verticalMovement = Vector3.up * _verticalVelocity * Time.deltaTime;
        Vector3 totalMovement = horizontalMovement + verticalMovement;
        _characterController.Move(totalMovement);
    }

    void HandleCamera()
    {
        if (_inputReceiver.LookDelta == Vector2.zero || _cameraTransform == null) return;

        transform.Rotate(0f, _inputReceiver.LookDelta.x * _mouseSensitivity, 0f);
        _verticalRotation = Mathf.Clamp(
            _verticalRotation - _inputReceiver.LookDelta.y * _mouseSensitivity,
            _verticalAngleLimits.x,
            _verticalAngleLimits.y
        );
        _cameraTransform.localRotation = Quaternion.Euler(_verticalRotation, 0f, 0f);
    }

    void UpdateAnimator()
    {
        if (_animator == null) return;

        _animator.SetFloat(StrafeHash, _inputReceiver.Move.x);
        _animator.SetFloat(ForwardHash, _inputReceiver.Move.y);
        _animator.SetBool(IsGroundedHash, _characterController.isGrounded);
    }
}