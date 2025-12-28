using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 100;
    private int _currentHealth;

    void Awake()
    {
        _currentHealth = _maxHealth;
    }

    public void TakeDamage(int damage)
    {
        _currentHealth -= damage;
        if (_currentHealth <= 0)
        {
            Die();
        }
    }

    public void KillInstantly()
    {
        _currentHealth = 0;
        Die();
    }

    void Die()
    {
        _currentHealth = _maxHealth; // восстановление при возрождении
        CheckpointManager.OnPlayerDeath();
    }

}
