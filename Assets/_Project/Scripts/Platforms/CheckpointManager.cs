using UnityEngine;

public class CheckpointManager : MonoBehaviour
{
    public static CheckpointManager Instance { get; private set; }

    [SerializeField] private string _playerTag = "Player";
    [SerializeField] private Vector3 _spawnPosition = new Vector3(0f, 1f, 0f);
    [SerializeField] private Quaternion _spawnRotation = Quaternion.identity; // начальный поворот

    private Vector3 _currentCheckpointPosition;
    private Quaternion _currentCheckpointRotation;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            _currentCheckpointPosition = _spawnPosition;
            _currentCheckpointRotation = _spawnRotation;
        }
        RespawnPlayer();
    }

    public void SetCurrentCheckpoint(Vector3 position, Quaternion rotation)
    {
        _currentCheckpointPosition = position;
        _currentCheckpointRotation = rotation;
    }

    public void SetCurrentCheckpoint(Vector3 position)
    {
        SetCurrentCheckpoint(position, Quaternion.identity);
    }

    public void RespawnPlayer()
    {
        GameObject playerObject = GameObject.FindGameObjectWithTag(_playerTag);
        if (playerObject == null)
        {
            return;
        }

        Transform playerTransform = playerObject.transform;
        playerTransform.position = _currentCheckpointPosition;
        playerTransform.rotation = _currentCheckpointRotation;

        CharacterController cc = playerObject.GetComponent<CharacterController>();
        if (cc != null)
        {
            cc.enabled = false;
            playerTransform.position = _currentCheckpointPosition;
            playerTransform.rotation = _currentCheckpointRotation;
            cc.enabled = true;
        }
    }

    public static void OnPlayerDeath()
    {
        Instance?.RespawnPlayer();
    }
}
