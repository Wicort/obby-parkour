using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] private bool _isActivated = false;
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private Color _activatedColor;

/*    void OnTriggerEnter(Collider other)
    {
        if (_isActivated) return;

        if (other.CompareTag("Player"))
        {
            _isActivated = true;
            
            CheckpointManager.Instance.SetCurrentCheckpoint(transform.position, transform.rotation);

            if (_renderer != null)
            {
                _renderer.material.color = _activatedColor;
            }
        }
    }*/

/*    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision");
        if (_isActivated) return;

        if (collision.gameObject.CompareTag("Player"))
        {
            _isActivated = true;

            CheckpointManager.Instance.SetCurrentCheckpoint(transform.position, transform.rotation);

            if (_renderer != null)
            {
                _renderer.material.color = _activatedColor;
            }
        }
    }*/

    public void Activate()
    {
        if (_isActivated) return;

        _isActivated = true;

        CheckpointManager.Instance.SetCurrentCheckpoint(transform.position, transform.rotation);

        if (_renderer != null)
        {
            _renderer.material.color = _activatedColor;
        }
    }
}
