using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private bool _isActivated = false;
    [SerializeField] private MeshRenderer _renderer;

    void OnTriggerEnter(Collider other)
    {
        if (_isActivated) return;

        if (other.CompareTag("Player"))
        {
            _isActivated = true;
            // ѕередаЄм и позицию, и поворот
            CheckpointManager.Instance.SetCurrentCheckpoint(transform.position, transform.rotation);

            if (_renderer != null)
            {
                _renderer.material.color = Color.green;
            }
        }
    }
}
