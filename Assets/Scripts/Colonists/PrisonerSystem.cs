using UnityEngine;
using System.Collections.Generic;

/// <summary>Prisoner system: capture enemies, hold in cells, recruit or execute.</summary>
public class PrisonerSystem : MonoBehaviour
{
    private GridManager _grid;
    private ColonistSpawner _spawner;
    private List<Prisoner> _prisoners = new();

    [System.Serializable]
    public class Prisoner
    {
        public string name;
        public Colonist colonist;
        public Vector3Int cellPos;
        public float resistance = 100f; // 0 = recruited
        public float loyalty = 50f; // chance to join
        public int daysCaptured;
    }

    void Awake()
    {
        _grid = FindFirstObjectByType<GridManager>();
        _spawner = FindFirstObjectByType<ColonistSpawner>();
    }

    /// <summary>Capture a downed enemy as prisoner.</summary>
    public bool CaptureEnemy(Enemy enemy, Vector3Int cellPos)
    {
        if (_spawner == null || _spawner.colonistPrefab == null) return false;
        if (enemy == null || enemy.state == CombatState.Dead) return false;

        var go = Object.Instantiate(_spawner.colonistPrefab, _grid.GridToWorld(cellPos.x, cellPos.y, cellPos.z), Quaternion.identity);
        var c = go.GetComponent<Colonist>();
        c.colonistName = $"Prisoner {_prisoners.Count + 1}";
        c.currentState = ColonistState.Incapacitated;
        c.health = 30;

        _prisoners.Add(new Prisoner { name = c.colonistName, colonist = c, cellPos = cellPos });
        Debug.Log($"[Prisoner] Captured {c.colonistName}");
        return true;
    }

    /// <summary>Try to recruit a prisoner. Requires social skill.</summary>
    public bool TryRecruit(Colonist warden, int prisonerIndex)
    {
        if (prisonerIndex < 0 || prisonerIndex >= _prisoners.Count) return false;
        var p = _prisoners[prisonerIndex];
        if (p.resistance <= 0) return false;

        float recruitPower = warden.socialSkill * 2f;
        p.resistance -= recruitPower * Time.unscaledDeltaTime;
        p.daysCaptured++;

        if (p.resistance <= 0 && Random.value < p.loyalty / 100f)
        {
            p.colonist.currentState = ColonistState.Idle;
            _spawner.Colonists.Add(p.colonist);
            _prisoners.RemoveAt(prisonerIndex);
            Debug.Log($"[Prisoner] Recruited {p.name}!");
            return true;
        }
        return false;
    }

    /// <summary>Execute a prisoner.</summary>
    public void Execute(int index)
    {
        if (index < 0 || index >= _prisoners.Count) return;
        var p = _prisoners[index];
        p.colonist.currentState = ColonistState.Dead;
        _prisoners.RemoveAt(index);
    }

    /// <summary>Release a prisoner.</summary>
    public void Release(int index)
    {
        if (index < 0 || index >= _prisoners.Count) return;
        var p = _prisoners[index];
        Object.Destroy(p.colonist.gameObject);
        _prisoners.RemoveAt(index);
    }

    public int PrisonerCount => _prisoners.Count;
    public List<Prisoner> Prisoners => _prisoners;

    void Update()
    {
        // Prisoners slowly lose resistance
        foreach (var p in _prisoners)
        {
            if (p.colonist.currentState == ColonistState.Dead) continue;
            p.resistance -= 0.01f * Time.unscaledDeltaTime; // slowly break
        }
    }
}
