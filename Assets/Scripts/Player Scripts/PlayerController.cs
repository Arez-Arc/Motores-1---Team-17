using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Movimiento")]
    [SerializeField] private float walkSpeed = 5f;
    [SerializeField] private float sprintSpeed = 9f;
    [SerializeField] private float speedSmoothTime = 0.1f;
    [SerializeField] private float gravity = -9.81f;

    [Header("Camara")]
    [SerializeField] private Transform cameraHolder;
    [SerializeField] private float mouseSensitivity = 100f;
    [SerializeField] private float maxLookAngle = 80f;

    private CharacterController characterController;
    private Vector3 velocity;
    private float xRotation = 0f;

    private float currentSpeed;
    private float speedSmoothVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        currentSpeed = walkSpeed;    
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
  
    }

    private void Update()
    {
        
        HandleMovement();
        HandleMouseLook();
    }

    private void HandleMovement()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Detectamos si el jugador quiere correr
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
     
        // Velocidad objetivo segun si corre o camina
        float targetSpeed = isSprinting ? sprintSpeed : walkSpeed;

        // Interpolamos suavemente entre la velocidad actual y la objetivo
        currentSpeed = Mathf.SmoothDamp(currentSpeed, targetSpeed, ref speedSmoothVelocity, speedSmoothTime);

        Vector3 moveDirection = transform.right * horizontal + transform.forward * vertical;
        characterController.Move(moveDirection * currentSpeed * Time.deltaTime);

        if (characterController.isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        velocity.y += gravity * Time.deltaTime;
        characterController.Move(velocity * Time.deltaTime);
    }

    private void HandleMouseLook()
    {
        float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -maxLookAngle, maxLookAngle);
        cameraHolder.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        transform.Rotate(Vector3.up * mouseX);
    }
}