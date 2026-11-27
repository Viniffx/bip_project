using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerCameraController : MonoBehaviour
{
    [Header("Alvo")]
    public Transform target;
    public Vector3 lookOffset = new Vector3(0f, 1.5f, 0f);

    [Header("Camera")]
    public float distance = 6f;
    public float minPitch = -20f;
    public float maxPitch = 65f;

    [Header("Mouse")]
    public float mouseSensitivity = 0.15f;

    [Header("Suavizacao")]
    public float followSmoothTime = 0.08f;

    private float yaw;
    private float pitch;
    private Vector3 followVelocity;

    private void Start()
    {
        Vector3 angles = transform.eulerAngles;
        yaw = angles.y;
        pitch = angles.x > 180f ? angles.x - 360f : angles.x;

        if (Mouse.current != null && Application.isFocused)
            CapturarCursor();
    }

    private void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            LiberarCursor();
            return;
        }

        if (Mouse.current == null)
            return;

        if (Cursor.lockState != CursorLockMode.Locked &&
            Mouse.current.leftButton.wasPressedThisFrame)
            CapturarCursor();

        if (Cursor.lockState != CursorLockMode.Locked)
            return;

        Vector2 mouseDelta = Mouse.current.delta.ReadValue();
        yaw += mouseDelta.x * mouseSensitivity;
        pitch = Mathf.Clamp(
            pitch - mouseDelta.y * mouseSensitivity,
            minPitch,
            maxPitch);
    }

    private void LateUpdate()
    {
        if (target == null)
            return;

        Quaternion rotation = Quaternion.Euler(pitch, yaw, 0f);
        Vector3 focusPoint = target.position + lookOffset;
        Vector3 desiredPosition = focusPoint - rotation * Vector3.forward * distance;

        transform.position = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref followVelocity,
            followSmoothTime);
        transform.rotation = rotation;
    }

    private void OnDisable()
    {
        LiberarCursor();
    }

    private static void CapturarCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private static void LiberarCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
