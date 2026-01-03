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
    [SerializeField] private Transform _groundPoint;
    [SerializeField, Min(0f)] private float _groundCheckDistance = 0.1f;
    [SerializeField] private LayerMask _groundLayer; 

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
    private bool _isGrounded;
    private Vector3 _accumulatedImpulse = Vector3.zero;

    public bool IsGrounded => CheckGround();
    public Transform CameraTransform => _cameraTransform;

    public void ApplyImpulse(Vector3 impulse)
    {
        _verticalVelocity += impulse.y;
        _accumulatedImpulse += new Vector3(impulse.x, 0f, impulse.z);
    }

    private void Awake()
    {
        _characterController = GetComponent<CharacterController>();
        _inputReceiver = GetComponent<PlayerInputReceiver>();
        if (_inputReceiver == null)
        {
            Debug.LogError("PlayerMovement requires PlayerInputReceiver on the same GameObject!");
            enabled = false;
            return;
        }

        // Убедитесь, что слой Ground выбран в Inspector
        if (_groundLayer == 0)
        {
            Debug.LogWarning("Ground Layer not assigned! Please set it in Inspector.");
        }

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        _isGrounded = CheckGround(); 

        HandleMovement();
        HandleCamera();
        UpdateAnimator();

        _inputReceiver.ResetJump();
        _inputReceiver.ResetDash();
        _inputReceiver.ResetLookThisFrame();
        _accumulatedImpulse = Vector3.zero;
    }

    private bool CheckGround()
    {
        Collider[] colliders = Physics.OverlapSphere(_groundPoint.position, _groundCheckDistance, _groundLayer);

        foreach(Collider collider in colliders)
        {
            if (collider.TryGetComponent(out Checkpoint checkpoint))
            {
                checkpoint.Activate();
            }
        }

        return colliders.Length > 0;
    }

    private void HandleMovement()
    {
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

        horizontalMovement += _accumulatedImpulse;

        if (_inputReceiver.JumpPressed && _isGrounded)
        {
            _verticalVelocity = Mathf.Sqrt(2f * _jumpHeight * Mathf.Abs(Physics.gravity.y));
        }

        const float enhancedGravityMultiplier = 2.0f;
        float gravity = Physics.gravity.y * enhancedGravityMultiplier;
        _verticalVelocity += gravity * Time.deltaTime;

        if (_isGrounded && _verticalVelocity < 0f)
            _verticalVelocity = 0f;

        Vector3 verticalMovement = Vector3.up * _verticalVelocity * Time.deltaTime;
        Vector3 totalMovement = horizontalMovement + verticalMovement;
        _characterController.Move(totalMovement);
    }

    private void HandleCamera()
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

    private void UpdateAnimator()
    {
        if (_animator == null) return;

        _animator.SetFloat(StrafeHash, _inputReceiver.Move.x);
        _animator.SetFloat(ForwardHash, _inputReceiver.Move.y);

        if (_animator.GetBool(IsGroundedHash) != _isGrounded)
            _animator.SetBool(IsGroundedHash, _isGrounded);
    }

    private void OnDrawGizmos()
    {
        if (_characterController == null) return;
        _isGrounded = CheckGround();
        Gizmos.color = _isGrounded ? Color.green : Color.red;
        Gizmos.DrawWireSphere(_groundPoint.position, _groundCheckDistance);
    }
}