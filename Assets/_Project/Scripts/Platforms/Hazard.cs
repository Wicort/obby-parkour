using UnityEngine;

public class Hazard : MonoBehaviour
{
    [Tooltip("Наносит ли урон (если у вас HP) или убивает мгновенно")]
    [SerializeField] private bool _instantKill = true;
    [SerializeField] private int _damage = 100;

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log("Hazard trigger");
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null)
            {
                Debug.Log("player healt is not null");
                if (_instantKill)
                {
                    Debug.Log("_instantKill");
                    playerHealth.KillInstantly();
                }
                else
                {
                    Debug.Log($"_damage: {_damage}");
                    playerHealth.TakeDamage(_damage);
                }
            }
            else
            {
                // Если нет PlayerHealth — убиваем напрямую
                CheckpointManager.OnPlayerDeath();
            }
        }
    }
}
