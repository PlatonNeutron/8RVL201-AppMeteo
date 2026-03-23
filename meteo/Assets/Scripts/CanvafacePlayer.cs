using UnityEngine;

public class CanvasFacePlayer : MonoBehaviour
{
    private Transform cameraTransform;

    void Start()
    {
        cameraTransform = Camera.main.transform;
    }

    void Update()
    {
        // Le canvas regarde toujours vers le joueur
        transform.LookAt(transform.position + cameraTransform.rotation * Vector3.forward,
                         cameraTransform.rotation * Vector3.up);
    }
}
