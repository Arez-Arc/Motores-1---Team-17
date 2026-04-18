using UnityEngine;

public class CameraFOV : MonoBehaviour
{
    [Header("FOV Base")]
    [SerializeField] private Camera playerCamera;
    [SerializeField] private float baseFOV = 90f;

    [Header("FOV al Correr")]
    [SerializeField] private bool changeFOVOnSprint = true;
    [SerializeField] private float sprintFOV = 100f;
    [SerializeField] private float fovSmoothTime = 0.15f;

    private float fovSmoothVelocity;

    private void Awake()
    {
        // Si no asignamos la camara en el Inspector la buscamos automaticamente
        if (playerCamera == null)
            playerCamera = GetComponentInChildren<Camera>();

        playerCamera.fieldOfView = baseFOV;
    }

    private void Update()
    {
        HandleFOV();
    }

    private void HandleFOV()
    {
        float targetFOV = baseFOV;

        if (changeFOVOnSprint)
        {
            bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
            targetFOV = isSprinting ? sprintFOV : baseFOV;
        }

        // Interpolamos suavemente igual que hicimos con la velocidad del sprint
        playerCamera.fieldOfView = Mathf.SmoothDamp(
            playerCamera.fieldOfView,
            targetFOV,
            ref fovSmoothVelocity,
            fovSmoothTime
        );
    }
}