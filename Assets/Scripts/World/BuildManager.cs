using UnityEngine;
using UnityEngine.InputSystem;

/// <summary>
/// Handles block placement and removal via mouse raycast.
/// LMB = place, RMB = remove, 1-9 = select block type.
/// </summary>
public class BuildManager : MonoBehaviour
{
    #region Public Fields

    [Header("References")]
    [Tooltip("GridManager reference")]
    [SerializeField] private GridManager _gridManager;

    [Header("Settings")]
    [Tooltip("Default block type for placement")]
    [SerializeField] private BlockType _selectedType = BlockType.Dirt;

    #endregion

    #region Private Fields

    private GameObject _highlight;
    private Vector3Int _placementPos;
    private bool _canPlace;

    #endregion

    #region Unity Lifecycle

    private void Awake()
    {
        if (_gridManager != null && _gridManager.GetComponent<MeshCollider>() == null)
            _gridManager.gameObject.AddComponent<MeshCollider>();

        _highlight = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _highlight.name = "BuildHighlight";
        Destroy(_highlight.GetComponent<Collider>());

        Shader shader = Shader.Find("Universal Render Pipeline/Unlit");
        if (shader == null) shader = Shader.Find("Sprites/Default");
        var mat = new Material(shader);
        mat.color = new Color(1, 1, 1, 0.3f);
        _highlight.GetComponent<MeshRenderer>().material = mat;
        _highlight.SetActive(false);
    }

    private void Update()
    {
        if (Keyboard.current == null || Mouse.current == null) return;

        HandleBlockSelection();
        HandleRaycast();
        HandleInput();
    }

    private void OnGUI()
    {
        GUI.Label(new Rect(10, 10, 300, 30), $"Block: {_selectedType} [1-9]");
    }

    #endregion

    #region Input

    private void HandleBlockSelection()
    {
        BlockType? sel = null;
        if (Keyboard.current.digit1Key.wasPressedThisFrame) sel = BlockType.Dirt;
        else if (Keyboard.current.digit2Key.wasPressedThisFrame) sel = BlockType.Grass;
        else if (Keyboard.current.digit3Key.wasPressedThisFrame) sel = BlockType.Stone;
        else if (Keyboard.current.digit4Key.wasPressedThisFrame) sel = BlockType.Wood;
        else if (Keyboard.current.digit5Key.wasPressedThisFrame) sel = BlockType.Glass;
        else if (Keyboard.current.digit6Key.wasPressedThisFrame) sel = BlockType.StoneBrick;
        else if (Keyboard.current.digit7Key.wasPressedThisFrame) sel = BlockType.WoodPlanks;
        else if (Keyboard.current.digit8Key.wasPressedThisFrame) sel = BlockType.Sand;
        else if (Keyboard.current.digit9Key.wasPressedThisFrame) sel = BlockType.Snow;
        if (sel.HasValue) _selectedType = sel.Value;
    }

    private void HandleInput()
    {
        if (Mouse.current.leftButton.wasPressedThisFrame) TryPlace();
        if (Mouse.current.rightButton.wasPressedThisFrame) TryRemove();
    }

    #endregion

    #region Raycast & Highlight

    private void HandleRaycast()
    {
        _canPlace = false;
        _highlight.SetActive(false);

        if (_gridManager == null || Camera.main == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (!Physics.Raycast(ray, out RaycastHit hit, 500f)) return;

        float half = _gridManager.BlockSize * 0.5f;
        Vector3 placePos = hit.point + hit.normal * half;
        _placementPos = _gridManager.WorldToGrid(placePos);

        if (!_gridManager.InBounds(_placementPos.x, _placementPos.y, _placementPos.z)) return;
        if (_gridManager.GetBlock(_placementPos.x, _placementPos.y, _placementPos.z) != BlockType.Air) return;

        _canPlace = true;
        _highlight.transform.position = _gridManager.GridToWorld(
            _placementPos.x, _placementPos.y, _placementPos.z);
        _highlight.transform.localScale = Vector3.one * _gridManager.BlockSize;
        _highlight.SetActive(true);
    }

    #endregion

    #region Build Actions

    private void TryPlace()
    {
        if (!_canPlace) return;
        _gridManager.SetBlock(_placementPos.x, _placementPos.y, _placementPos.z, _selectedType);
    }

    private void TryRemove()
    {
        if (_gridManager == null || Camera.main == null) return;

        Vector2 mousePos = Mouse.current.position.ReadValue();
        Ray ray = Camera.main.ScreenPointToRay(mousePos);

        if (!Physics.Raycast(ray, out RaycastHit hit, 500f)) return;

        float half = _gridManager.BlockSize * 0.5f;
        Vector3Int pos = _gridManager.WorldToGrid(hit.point - hit.normal * half);

        if (!_gridManager.InBounds(pos.x, pos.y, pos.z)) return;
        if (_gridManager.GetBlock(pos.x, pos.y, pos.z) == BlockType.Air) return;

        _gridManager.RemoveBlock(pos.x, pos.y, pos.z);
    }

    #endregion
}
