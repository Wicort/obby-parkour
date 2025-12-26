using UnityEngine;

public class DoubleJumpAbility : AbilityBase
{
    [SerializeField] private float _doubleJumpHeight = 1.5f;

    private int _remainingJumps;

    protected void Update()
    {
        if (!_isEnabled) return;

        if (_playerMovement.IsGrounded)
        {
            _remainingJumps = 1;
        }

        if (_inputReceiver.JumpPressed && !_playerMovement.IsGrounded && _remainingJumps > 0)
        {
            float verticalVelocity = Mathf.Sqrt(2f * _doubleJumpHeight * Mathf.Abs(Physics.gravity.y));
            _playerMovement.ApplyImpulse(Vector3.up * verticalVelocity);
            _remainingJumps--;
        }
    }
}
