using UnityEngine;

public class HeadbobController : MonoBehaviour
{
    [Header("Configuracion General")]
    [SerializeField] private bool enableHeadbob = true;
    [SerializeField] private Transform cameraHolder;

    [Header("Caminar")]
    [SerializeField] private float walkBobSpeed = 10f;
    [SerializeField] private float walkBobAmountX = 0.05f;
    [SerializeField] private float walkBobAmountY = 0.03f;

    [Header("Correr")]
    [SerializeField] private float sprintBobSpeed = 16f;
    [SerializeField] private float sprintBobAmountX = 0.1f;
    [SerializeField] private float sprintBobAmountY = 0.07f;

    [Header("Suavizado")]
    [SerializeField] private float returnSpeed = 8f;

    [Header("Sonido de Pisadas")]
    [SerializeField] private AudioSource footstepAudioSource;
    [SerializeField] private AudioClip[] walkSounds;
    [SerializeField] private AudioClip[] sprintSounds;
    [SerializeField] private float walkStepInterval = 0.5f;
    [SerializeField] private float sprintStepInterval = 0.3f;

    private float timer = 0f;
    private float footstepTimer = 0f;
    private Vector3 initialCameraPosition;
    private CharacterController characterController;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        initialCameraPosition = cameraHolder.localPosition;
    }

    private void Update()
    {
        if (!enableHeadbob) return;

        HandleHeadbob();
    }

    private void HandleHeadbob()
    {
        bool isMoving = new Vector2(Input.GetAxis("Horizontal"),
                        Input.GetAxis("Vertical")).magnitude > 0.1f;
        bool isGrounded = characterController.isGrounded;
        bool isSprinting = Input.GetKey(KeyCode.LeftShift) ||
                           Input.GetKey(KeyCode.RightShift);

        if (isMoving && isGrounded)
        {
            float bobSpeed = isSprinting ? sprintBobSpeed : walkBobSpeed;
            float bobAmountX = isSprinting ? sprintBobAmountX : walkBobAmountX;
            float bobAmountY = isSprinting ? sprintBobAmountY : walkBobAmountY;

            timer += Time.deltaTime * bobSpeed;

            float newX = initialCameraPosition.x + Mathf.Sin(timer) * bobAmountX;
            float newY = initialCameraPosition.y + Mathf.Sin(timer * 2f) * bobAmountY;
            cameraHolder.localPosition = new Vector3(newX, newY, initialCameraPosition.z);

            // Manejamos el timer de pisadas
            float stepInterval = isSprinting ? sprintStepInterval : walkStepInterval;
            footstepTimer += Time.deltaTime;

            if (footstepTimer >= stepInterval)
            {
                PlayFootstep(isSprinting);
                footstepTimer = -0.2f;
            }
        }
        else
        {
            // Reseteamos el timer de pisadas al detenerse
            // NO tocamos el AudioSource, dejamos que el sonido actual termine solo
            footstepTimer = 0f;

            timer = 0f;
            cameraHolder.localPosition = Vector3.Lerp(
                cameraHolder.localPosition,
                initialCameraPosition,
                returnSpeed * Time.deltaTime
            );
        }
    }

    private void PlayFootstep(bool isSprinting)
    {
        AudioClip[] sounds = isSprinting ? sprintSounds : walkSounds;

        if (sounds == null || sounds.Length == 0) return;

        int randomIndex = Random.Range(0, sounds.Length);
        AudioClip clipToPlay = sounds[randomIndex];

        footstepAudioSource.PlayOneShot(clipToPlay);
    }
}