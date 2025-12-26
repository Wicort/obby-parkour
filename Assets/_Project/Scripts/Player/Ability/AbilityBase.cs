using UnityEngine;

public class AbilityBase : MonoBehaviour
{
    [Tooltip("Можно ли использовать способность?")]
    [SerializeField] protected bool _isEnabled = true;

    protected PlayerMovement _playerMovement;
    protected PlayerInputReceiver _inputReceiver;

    protected virtual void Awake()
    {
        _playerMovement = GetComponent<PlayerMovement>();
        _inputReceiver = GetComponent<PlayerInputReceiver>();
    }

    public void SetEnabled(bool enabled) => _isEnabled = enabled;
    public bool IsEnabled() => _isEnabled;
}
