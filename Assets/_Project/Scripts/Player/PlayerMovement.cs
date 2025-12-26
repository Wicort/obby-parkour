using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform _cameraTransform;
    [SerializeField] private Animator _animator;

    [Header("Movement")]
    [SerializeField, Min(0f)] private float _movementSpeed = 5f;
    [SerializeField, Min(0f)] private float _jumpHeight = 2f;
    [SerializeField, Min(0f)] private float _doubleJumpHeight = 1.5f; // ← Высота второго прыжка
    [SerializeField, Min(0f)] private float _groundedBufferSeconds = 0.15f;
    [SerializeField] private bool _enableDoubleJump = true; // ← Можно отключить в инспекторе

    [Header("Camera")]
    [SerializeField, Range(0.01f, 1f)] private float _mouseSensitivity = 0.15f;
    [SerializeField] private Vector2 _verticalAngleLimits = new Vector2(-60f, 60f);

    private static readonly int StrafeHash = Animator.StringToHash("Strafe");
    private static readonly int ForwardHash = Animator.StringToHash("Forward");
    private static readonly int IsGroundedHash = Animator.StringToHash("IsGrounded");
    private static readonly int JumpCountHash = Animator.StringToHash("JumpCount"); // ← Для анимаций

    private CharacterController _characterController;
    private PlayerInputReceiver _inputReceiver;

    private float _verticalRotation;
    private float _verticalVelocity;
    private float _timeSinceLastGrounded;
    private int _remainingJumps; // ← Сколько прыжков осталось

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
        // Обновляем состояние "на земле"
        if (_characterController.isGrounded)
        {
            _timeSinceLastGrounded = 0f;
            _remainingJumps = 1; // Сброс: можно прыгать 1 раз (обычный прыжок)
        }
        else
        {
            _timeSinceLastGrounded += Time.deltaTime;
        }

        HandleMovement();
        HandleCamera();
        UpdateAnimator();

        _inputReceiver.ConsumeJump();
        _inputReceiver.ResetLookThisFrame();
    }

    void HandleMovement()
    {
        // Горизонтальное движение
        Vector3 horizontalMovement = Vector3.zero;
        if (_inputReceiver.Move != Vector2.zero && _cameraTransform != null)
        {
            var forward = _cameraTransform.forward;
            var right = _cameraTransform.right;
            forward.y = right.y = 0f;
            forward.Normalize();
            right.Normalize();

            var direction = forward * _inputReceiver.Move.y + right * _inputReceiver.Move.x;
            if (direction.magnitude > 1f) direction.Normalize();

            horizontalMovement = direction * _movementSpeed * Time.deltaTime;
        }

        // === ПРЫЖОК ===
        bool canJumpFromGround = _characterController.isGrounded ||
                                _timeSinceLastGrounded <= _groundedBufferSeconds;

        if (_inputReceiver.JumpPressed)
        {
            if (canJumpFromGround && _remainingJumps >= 1)
            {
                // Первый прыжок
                _verticalVelocity = Mathf.Sqrt(2f * _jumpHeight * Mathf.Abs(Physics.gravity.y));
                _remainingJumps = _enableDoubleJump ? 1 : 0; // Если двойной прыжок включён — остаётся 1 дополнительный
                _timeSinceLastGrounded = float.MaxValue; // блокируем буфер после прыжка
            }
            else if (_enableDoubleJump && _remainingJumps > 0 && !canJumpFromGround)
            {
                // Второй (или последующий) прыжок в воздухе
                _verticalVelocity = Mathf.Sqrt(2f * _doubleJumpHeight * Mathf.Abs(Physics.gravity.y));
                _remainingJumps--;
            }
        }

        // Гравитация
        _verticalVelocity += Physics.gravity.y * Time.deltaTime;
        if (_characterController.isGrounded && _verticalVelocity < 0f)
            _verticalVelocity = 0f;

        // Один вызов Move()
        Vector3 totalMovement = horizontalMovement + Vector3.up * _verticalVelocity * Time.deltaTime;
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
        _animator.SetFloat(JumpCountHash, _enableDoubleJump ? (2 - _remainingJumps) : 0); // 0=на земле, 1=1 прыжок, 2=2 прыжок
    }
}