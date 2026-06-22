using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Tab/Shift+Tab = move camera up/down one floor.
/// Attached to the camera or any GameObject.
/// </summary>
public class FloorController : MonoBehaviour
{
    public int currentFloor;
    public float floorHeight = 3f; // how far to jump per floor
    public float minFloor = -2;
    public float maxFloor = 10;

    private Camera _cam;

    void Start()
    {
        _cam = Camera.main ?? FindObjectOfType<Camera>();
        if (_cam != null) currentFloor = Mathf.RoundToInt(_cam.transform.position.y / floorHeight);
    }

    void Update()
    {
        if (Keyboard.current == null) return;

        if (Keyboard.current.tabKey.wasPressedThisFrame)
        {
            int dir = Keyboard.current.leftShiftKey.isPressed ? -1 : 1;
            currentFloor = Mathf.Clamp(currentFloor + dir, (int)minFloor, (int)maxFloor);
            if (_cam != null)
            {
                Vector3 pos = _cam.transform.position;
                pos.y = currentFloor * floorHeight;
                _cam.transform.position = pos;
            }
        }
    }
}
