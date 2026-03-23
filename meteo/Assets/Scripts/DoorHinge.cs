using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;
using UnityEngine.XR.Interaction.Toolkit.Interactables;
using UnityEngine.XR.Interaction.Toolkit.Interactors;

public class DoorHinge : MonoBehaviour
{
    [Header("Limites d'angle")]
    public float minAngle = -90f;
    public float maxAngle = 90f;

    [Header("Sensibilité")]
    public float sensitivity = 1.5f;

    private bool isGrabbed = false;
    private Transform interactorTransform;
    private float lastInteractorAngle;
    private float currentDoorAngle;

    private XRGrabInteractable grabInteractable;

    void Start()
    {
        grabInteractable = GetComponentInChildren<XRGrabInteractable>();
        grabInteractable.selectEntered.AddListener(OnGrab);
        grabInteractable.selectExited.AddListener(OnRelease);
        grabInteractable.trackPosition = false;
        grabInteractable.trackRotation = false;

        currentDoorAngle = transform.localEulerAngles.y;
    }

    void OnGrab(SelectEnterEventArgs args)
    {
        isGrabbed = true;
        interactorTransform = args.interactorObject.transform;

        // Enregistre l'angle initial du contrôleur
        Vector3 dir = interactorTransform.forward;
        dir.y = 0f;
        lastInteractorAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

        currentDoorAngle = transform.localEulerAngles.y;
        if (currentDoorAngle > 180f) currentDoorAngle -= 360f;
    }

    void OnRelease(SelectExitEventArgs args)
    {
        isGrabbed = false;
        interactorTransform = null;
    }

    void Update()
    {
        if (isGrabbed && interactorTransform != null)
        {
            // Angle actuel du contrôleur
            Vector3 dir = interactorTransform.forward;
            dir.y = 0f;
            float currentInteractorAngle = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

            // Delta de rotation du contrôleur
            float delta = Mathf.DeltaAngle(lastInteractorAngle, currentInteractorAngle);
            lastInteractorAngle = currentInteractorAngle;

            // Applique le delta à la porte avec sensibilité
            currentDoorAngle += delta * sensitivity;
            currentDoorAngle = Mathf.Clamp(currentDoorAngle, minAngle, maxAngle);

            transform.localEulerAngles = new Vector3(0f, currentDoorAngle, 0f);
        }
    }
}