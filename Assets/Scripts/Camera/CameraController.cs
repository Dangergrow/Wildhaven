using UnityEngine;

/// <summary>
/// Camera controller for Wildhaven.
/// WASD/arrows = move, Q/E = rotate (45 deg), Scroll = zoom, MMB drag = free rotate, Home = reset.
/// </summary>
public class CameraController : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Camera movement speed (units per second)")]
    public float moveSpeed = 20f;

    [Tooltip("Speed multiplier when holding Shift")]
    public float fastMoveMultiplier = 3f;

    [Header("Rotation")]
    [Tooltip("Rotation angle per Q/E press (degrees)")]
    public float rotationStep = 45f;

    [Tooltip("Free rotation speed with MMB drag")]
    public float freeRotateSpeed = 2f;

    [Header("Zoom")]
    [Tooltip("Minimum zoom distance from ground")]
    public float minZoom = 5f;

    [Tooltip("Maximum zoom distance from ground")]
    public float maxZoom = 50f;

    [Tooltip("Zoom speed (scroll wheel)")]
    public float zoomSpeed = 10f;

    [Header("Position Limits")]
    [Tooltip("Minimum X position")]
    public float minX = -50f;

    [Tooltip("Maximum X position")]
    public float maxX = 150f;

    [Tooltip("Minimum Z position")]
    public float minZ = -50f;

    [Tooltip("Maximum Z position")]
    public float maxZ = 150f;

    [Tooltip("Minimum camera height")]
    public float minY = 5f;

    [Tooltip("Maximum camera height")]
    public float maxY = 60f;

    // Rotation state
    private float _currentRotationY = 45f; // default isometric-ish angle
    private float _currentRotationX = 45f; // pitch

    // MMB state
    private bool _isRotating;
    private Vector3 _lastMousePos;

    private void Start()
    {
        ApplyRotation();
    }

    private void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
        HandleFreeRotate();
        HandleReset();
    }

    /// <summary>
    /// WASD / arrow key movement.
    /// </summary>
    private void HandleMovement()
    {
        float speed = moveSpeed;
        if (Input.GetKey(KeyCode.LeftShift)) speed *= fastMoveMultiplier;

        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 forward = transform.forward;
        forward.y = 0f;
        forward.Normalize();

        Vector3 right = transform.right;
        right.y = 0f;
        right.Normalize();

        Vector3 move = (forward * v + right * h) * speed * Time.deltaTime;
        Vector3 newPos = transform.position + move;

        // Clamp position
        newPos.x = Mathf.Clamp(newPos.x, minX, maxX);
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        newPos.z = Mathf.Clamp(newPos.z, minZ, maxZ);

        transform.position = newPos;
    }

    /// <summary>
    /// Q/E — step rotation.
    /// </summary>
    private void HandleRotation()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            _currentRotationY -= rotationStep;
            ApplyRotation();
        }
        if (Input.GetKeyDown(KeyCode.E))
        {
            _currentRotationY += rotationStep;
            ApplyRotation();
        }
    }

    /// <summary>
    /// Mouse wheel zoom.
    /// </summary>
    private void HandleZoom()
    {
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Approximately(scroll, 0f)) return;

        Vector3 forward = transform.forward;
        Vector3 newPos = transform.position + forward * scroll * zoomSpeed;
        newPos.y = Mathf.Clamp(newPos.y, minY, maxY);
        transform.position = newPos;
    }

    /// <summary>
    /// Hold MMB for free rotation.
    /// </summary>
    private void HandleFreeRotate()
    {
        if (Input.GetMouseButtonDown(2))
        {
            _isRotating = true;
            _lastMousePos = Input.mousePosition;
        }
        if (Input.GetMouseButtonUp(2))
        {
            _isRotating = false;
        }

        if (_isRotating)
        {
            Vector3 delta = Input.mousePosition - _lastMousePos;
            _lastMousePos = Input.mousePosition;

            _currentRotationY += delta.x * freeRotateSpeed * 0.1f;
            _currentRotationX -= delta.y * freeRotateSpeed * 0.1f;
            _currentRotationX = Mathf.Clamp(_currentRotationX, 10f, 80f);
            ApplyRotation();
        }
    }

    /// <summary>
    /// Home key resets camera to default position.
    /// </summary>
    private void HandleReset()
    {
        if (Input.GetKeyDown(KeyCode.Home))
        {
            _currentRotationY = 45f;
            _currentRotationX = 45f;
            ApplyRotation();
            transform.position = new Vector3(worldCenterX, 30f, worldCenterZ);
        }
    }

    /// <summary>
    /// Applies current rotation angles to camera transform.
    /// </summary>
    private void ApplyRotation()
    {
        transform.rotation = Quaternion.Euler(_currentRotationX, _currentRotationY, 0f);
    }

    // World center offsets — set these to match your GridManager world size
    private float worldCenterX => 50f;
    private float worldCenterZ => 50f;
}
