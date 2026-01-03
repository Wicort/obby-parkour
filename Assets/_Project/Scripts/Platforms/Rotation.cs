using UnityEngine;

public class Rotation : MonoBehaviour
{
    [Header("Rotation Speed (degrees per second)")]
    [Tooltip("Скорость вращения вокруг локальной оси X")]
    public float rotationX = 0f;

    [Tooltip("Скорость вращения вокруг локальной оси Y")]
    public float rotationY = 0f;

    [Tooltip("Скорость вращения вокруг локальной оси Z")]
    public float rotationZ = 0f;

    [Header("Rotation Space")]
    [SerializeField] private bool _useLocalRotation = true;

    void Update()
    {
        if (rotationX == 0f && rotationY == 0f && rotationZ == 0f)
            return;

        Vector3 rotation = new Vector3(rotationX, rotationY, rotationZ) * Time.deltaTime;

        if (_useLocalRotation)
        {
            // Вращение относительно локальных осей объекта
            transform.Rotate(rotation, Space.Self);
        }
        else
        {
            // Вращение относительно мировых осей
            transform.Rotate(rotation, Space.World);
        }
    }
}
