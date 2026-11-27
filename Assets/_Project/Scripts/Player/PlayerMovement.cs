using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerMovement : MonoBehaviour
{
    [Header("Movimento")]
    public float speed = 5f;
    public float jumpHeight = 1.2f;
    public float gravity = -20f;

    [Header("Referencias")]
    public Camera cam;
    public Animator animator;

    private const float RotationSpeed = 720f;
    private const float AnimatorDampTime = 0.1f;

    private CharacterController characterController;
    private InputAction moveAction;
    private InputAction jumpAction;
    private float verticalVelocity;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
            Debug.LogError("PlayerMovement precisa de um CharacterController.", this);

        if (InputSystem.actions == null)
        {
            Debug.LogError("Nenhum asset de Input Actions esta configurado.", this);
        }
        else
        {
            moveAction = InputSystem.actions.FindAction("Move", false);
            jumpAction = InputSystem.actions.FindAction("Jump", false);

            if (moveAction == null)
                Debug.LogError("A acao Move nao foi encontrada.", this);
            if (jumpAction == null)
                Debug.LogError("A acao Jump nao foi encontrada.", this);
        }

        if (cam == null)
        {
            cam = Camera.main;
            if (cam == null)
                Debug.LogWarning("Camera.main nao foi encontrada; o movimento usara os eixos do mundo.", this);
        }
    }

    private void OnEnable()
    {
        if (moveAction != null)
            moveAction.Enable();
        if (jumpAction != null)
            jumpAction.Enable();
    }

    private void OnDisable()
    {
        if (moveAction != null)
            moveAction.Disable();
        if (jumpAction != null)
            jumpAction.Disable();
    }

    private void Update()
    {
        if (characterController == null)
        {
            characterController = GetComponent<CharacterController>();
            if (characterController == null)
                return;
        }

        float deltaTime = Time.deltaTime;
        Vector2 input = moveAction != null ? moveAction.ReadValue<Vector2>() : Vector2.zero;
        float cameraYaw = cam != null ? cam.transform.eulerAngles.y : 0f;
        Vector3 moveDirection = Quaternion.Euler(0f, cameraYaw, 0f) *
                                new Vector3(input.x, 0f, input.y);

        if (moveDirection.sqrMagnitude > 1f)
            moveDirection.Normalize();

        if (moveDirection.sqrMagnitude > 0.001f)
        {
            Quaternion targetRotation = Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                RotationSpeed * deltaTime);
        }

        if (animator != null)
            animator.SetFloat("Speed", Mathf.Clamp01(input.magnitude), AnimatorDampTime, deltaTime);

        if (characterController.isGrounded)
        {
            if (verticalVelocity < 0f)
                verticalVelocity = -2f;

            if (jumpAction != null && jumpAction.WasPressedThisFrame())
                verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        verticalVelocity += gravity * deltaTime;
        Vector3 velocity = moveDirection * speed + Vector3.up * verticalVelocity;
        characterController.Move(velocity * deltaTime);
    }
}
