using UnityEngine;

public class Parantable : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log($"OnTriggerEnter: {other.name}");

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player detected! Parenting to: " + transform.name);
            other.gameObject.transform.SetParent(transform);
            Debug.Log($"Parent set to: {other.gameObject.transform.parent}");
        }
        else
        {
            Debug.LogWarning($"No PlayerMovement on: {other.name}");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log($"OnTriggerExit: {other.name}");

        if (other.CompareTag("Player"))
        {
            Debug.Log("Player leaving! Unparenting...");
            other.gameObject.transform.SetParent(null);
            Debug.Log($"Parent cleared: {other.gameObject.transform.parent}");
        }
    }
}
