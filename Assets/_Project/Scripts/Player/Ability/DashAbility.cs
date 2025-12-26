using UnityEngine;

public class DashAbility : AbilityBase
{
    [SerializeField] private float _dashDistance = 5f;
    [SerializeField] private float _dashDuration = 0.2f;

    private bool _isDashing;
    private float _dashTimer;
    private Vector3 _dashDirection;

    protected void Update()
    {
        if (!_isEnabled) return;

        if (_inputReceiver.DashPressed && !_isDashing)
        {
            _dashDirection = _playerMovement.CameraTransform.forward;
            _dashDirection.y = 0f;
            _dashDirection.Normalize();

            _isDashing = true;
            _dashTimer = _dashDuration;
        }

        if (_isDashing)
        {
            _dashTimer -= Time.deltaTime;
            if (_dashTimer <= 0f)
            {
                _isDashing = false;
            }
            else
            {
                Vector3 dashImpulse = _dashDirection * (_dashDistance / _dashDuration) * Time.deltaTime;
                _playerMovement.ApplyImpulse(dashImpulse);
            }
        }
    }
}
