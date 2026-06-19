using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Wildhaven camera: WASD/arrows move, Q/E rotate 45, scroll zoom, MMB free rotate, Home reset.
/// </summary>
public class CameraController : MonoBehaviour
{
    #region Public Fields

    [Header("Movement")]
    [Tooltip("Speed in units/second")]
    public float moveSpeed = 60f;

    [Tooltip("Speed multiplier when holding Shift")]
    public float fastMultiplier = 3f;

    [Header("Rotation")]
    [Tooltip("Degrees per Q/E press")]
    public float rotationStep = 45f;

    [Tooltip("MMB drag sensitivity")]
    public float freeRotateSpeed = 3f;

    [Header("Zoom")]
    [Tooltip("Min camera height")]
    public float minY = 5f;

    [Tooltip("Max camera height")]
    public float maxY = 80f;

    [Tooltip("Scroll speed")]
    public float zoomSpeed = 120f;

    #endregion

    #region Private Fields

    private float _rotY = 45f;
    private float _rotX = 65f;
    private bool _isRotating;
    private Vector2 _lastMouse;

    #endregion

    #region Unity Lifecycle

    private void Start()
    {
        if (Keyboard.current == null || Mouse.current == null) return;
        ApplyRotation();
    }

    private void Update()
    {
        if (Keyboard.current == null || Mouse.current == null) return;

        HandleMovement();
        HandleQERotation();
        HandleZoom();
        HandleFreeRotate();
        HandleReset();
    }

    #endregion

    #region Input Handlers

    private void HandleMovement()
    {
        float h = 0, v = 0;
        if (Keyboard.current.dKey.isPressed || Keyboard.current.rightArrowKey.isPressed) h = 1;
        if (Keyboard.current.aKey.isPressed || Keyboard.current.leftArrowKey.isPressed) h = -1;
        if (Keyboard.current.wKey.isPressed || Keyboard.current.upArrowKey.isPressed) v = 1;
        if (Keyboard.current.sKey.isPressed || Keyboard.current.downArrowKey.isPressed) v = -1;

        float speed = moveSpeed;
        if (Keyboard.current.leftShiftKey.isPressed) speed *= fastMultiplier;

        Vector3 fwd = transform.forward; fwd.y = 0; fwd.Normalize();
        Vector3 rgt = transform.right; rgt.y = 0; rgt.Normalize();

        transform.position += (fwd * v + rgt * h) * (speed * Time.deltaTime);
    }

    private void HandleQERotation()
    {
        if (Keyboard.current.qKey.wasPressedThisFrame) _rotY -= rotationStep;
        if (Keyboard.current.eKey.wasPressedThisFrame) _rotY += rotationStep;

        if (Keyboard.current.qKey.wasPressedThisFrame || Keyboard.current.eKey.wasPressedThisFrame)
            ApplyRotation();
    }

    private void HandleZoom()
    {
        float scroll = Mouse.current.scroll.ReadValue().y;
        if (Mathf.Approximately(scroll, 0f)) return;

        scroll /= 120f;
        scroll *= 5f; // boost
        Vector3 pos = transform.position + transform.forward * (scroll * zoomSpeed);
        pos.y = Mathf.Clamp(pos.y, minY, maxY);
        transform.position = pos;
    }

    private void HandleFreeRotate()
    {
        if (Mouse.current.middleButton.wasPressedThisFrame)
        {
            _isRotating = true;
            _lastMouse = Mouse.current.position.ReadValue();
        }
        if (Mouse.current.middleButton.wasReleasedThisFrame)
            _isRotating = false;

        if (!_isRotating) return;

        Vector2 delta = Mouse.current.position.ReadValue() - _lastMouse;
        _lastMouse = Mouse.current.position.ReadValue();
        _rotY += delta.x * freeRotateSpeed * 0.05f;
        _rotX -= delta.y * freeRotateSpeed * 0.05f;
        _rotX = Mathf.Clamp(_rotX, 10f, 85f);
        ApplyRotation();
    }

    private void HandleReset()
    {
        if (!Keyboard.current.homeKey.wasPressedThisFrame) return;
        _rotY = 45f;
        _rotX = 65f;
        ApplyRotation();
        transform.position = new Vector3(50f, 50f, 50f);
    }

    #endregion

    #region Helpers

    private void ApplyRotation()
    {
        transform.rotation = Quaternion.Euler(_rotX, _rotY, 0f);
    }

    #endregion
}
