using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// AI controller for colonists. Manages state transitions, wandering, auto-collect, and player orders.
/// </summary>
[RequireComponent(typeof(Colonist), typeof(NeedsSystem))]
public class ColonistAI : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public ColonistTask currentTask;
    public Vector3 taskTarget;
    public int[] jobPriorities = new int[14] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };

    /// <summary>Player-issued order: Move, Mine, or Attack.</summary>
    public enum OrderType { None, Move, Mine, Attack }
    public OrderType currentOrder = OrderType.None;
    public Vector3 orderTarget;

    private Colonist _colonist;
    private NeedsSystem _needs;
    private DayCycle _day;
    private GridManager _grid;
    private float _speed;
    private Vector3 _wanderTarget;
    private float _wanderTimer;
    private List<Vector3Int> _path;
    private int _pathIndex;
    private bool _isMoving;

    /// <summary>Issue a player order. Returns false if colonist can't accept orders.</summary>
    public bool GiveOrder(OrderType type, Vector3 target)
    {
        if (_colonist == null) { Debug.LogWarning($"[Order] {name} — colonist is NULL"); return false; }
        if (_colonist.currentState == ColonistState.Dead) { Debug.LogWarning($"[Order] {name} — DEAD"); return false; }
        if (_colonist.currentState == ColonistState.Incapacitated) { Debug.LogWarning($"[Order] {name} — INCAPACITATED"); return false; }
        if (_colonist.currentState == ColonistState.Sleeping) _colonist.currentState = ColonistState.Idle;
        currentOrder = type;
        orderTarget = target;
        // Compute A* path — lazy find GridManager if needed
        if (_grid == null) _grid = FindObjectOfType<GridManager>();
        if (_grid != null) {
            Vector3Int start = _grid.WorldToGrid(transform.position);
            Vector3Int end = _grid.WorldToGrid(target);
            _path = Pathfinder.FindPath(_grid, start, end);
            _pathIndex = 0;
        }
        return true;
    }

    /// <summary>Cancel current order.</summary>
    public void CancelOrder() { currentOrder = OrderType.None; _path = null; }

    private void Awake()
    {
        _colonist = GetComponent<Colonist>();
        // Prefab may have broken script references (GUID mismatch across PCs)
        if (_colonist == null) _colonist = gameObject.AddComponent<Colonist>();
        _needs = GetComponent<NeedsSystem>();
        if (_needs == null) _needs = gameObject.AddComponent<NeedsSystem>();
        _day = FindObjectOfType<DayCycle>();
        _grid = FindObjectOfType<GridManager>();
        _speed = walkSpeed;
        PickWanderTarget();
    }

    private void Update()
    {
        if (_colonist == null || _colonist.currentState == ColonistState.Dead) return;
        if (_day != null && _day.IsPaused && currentOrder == OrderType.None) return; // allow orders during pause
        SnapToSurface();
        EvaluateState();
        if (HandleOrder()) return; // Player orders take priority
        HandleWandering();
        HandleCombat();
    }

    void HandleWandering()
    {
        if (_colonist.currentState == ColonistState.Dead || _colonist.currentState == ColonistState.Sleeping || _colonist.currentState == ColonistState.Fighting) return;
        if (_colonist.currentState != ColonistState.Idle && _colonist.currentState != ColonistState.Moving) return;

        float dt = Time.deltaTime * (_day != null ? _day.gameSpeed : 1f);
        _wanderTimer -= dt;
        if (_wanderTimer <= 0f) { PickWanderTarget(); _wanderTimer = Random.Range(1.5f, 4f); }

        float dist = Vector3.Distance(transform.position, _wanderTarget);
        if (dist > 0.3f)
        {
            Vector3 dir = (_wanderTarget - transform.position).normalized;
            float step = _speed * 0.4f * dt;
            Vector3 nextPos = transform.position + dir * step;

            // AABB vs grid collision: check character's full width
            if (CanMoveTo(nextPos))
            {
                transform.position = nextPos;
                _isMoving = true;
            }
            else PickWanderTarget();
        }
        else _isMoving = false;
    }

    /// <summary>Executes player orders (Move/Mine/Attack). Returns true if order is active.</summary>
    bool HandleOrder()
    {
        if (currentOrder == OrderType.None) return false;
        if (_colonist.currentState == ColonistState.Dead || _colonist.currentState == ColonistState.Incapacitated) return false;
        _colonist.currentState = ColonistState.Moving;
        float dt = Time.unscaledDeltaTime;
        float spd = _speed > 0 ? _speed : walkSpeed;

        if (_path == null)
        {
            // No path — walk directly toward target
            Vector3 dir = (orderTarget - transform.position).normalized;
            Vector3 next = transform.position + dir * spd * dt;
            float dist = Vector3.Distance(transform.position, orderTarget);

            if (dist < 0.5f)
            {
                if (currentOrder == OrderType.Move) { CancelOrder(); return false; }
                if (currentOrder == OrderType.Mine && _grid != null)
                {
                    Vector3Int gp = _grid.WorldToGrid(orderTarget);
                    if (_grid.InBounds(gp.x, gp.y, gp.z) && _grid.GetBlock(gp.x, gp.y, gp.z) != BlockType.Air)
                        _grid.RemoveBlock(gp.x, gp.y, gp.z);
                }
                CancelOrder(); return false;
            }
            if (CanMoveTo(next, 0.1f, true)) transform.position = next;
            // If blocked, just wait — don't cancel the order
            return true;
        }
        if (_path != null && _pathIndex < _path.Count)
        {
            Vector3 target = _grid.GridToWorld(_path[_pathIndex].x, _path[_pathIndex].y, _path[_pathIndex].z);
            target.y += 0.1f;
            float dist = Vector3.Distance(transform.position, target);
            if (dist < 0.3f) { _pathIndex++; return true; }
            Vector3 dir = (target - transform.position).normalized;
            Vector3 next = transform.position + dir * spd * dt;
            if (CanMoveTo(next, 0.1f, true)) transform.position = next;
            else _pathIndex++; // skip blocked node
            return true;
        }

        // Path finished — execute order
        _path = null;
        if (currentOrder == OrderType.Move) { CancelOrder(); return false; }
        if (currentOrder == OrderType.Mine && _grid != null)
        {
            Vector3Int gp = _grid.WorldToGrid(orderTarget);
            if (_grid.InBounds(gp.x, gp.y, gp.z) && _grid.GetBlock(gp.x, gp.y, gp.z) != BlockType.Air)
                _grid.RemoveBlock(gp.x, gp.y, gp.z);
            CancelOrder(); return false;
        }
        CancelOrder(); return false;
    }

    /// <summary>Keep colonist on terrain surface. Only adjusts when Y off by >0.5.</summary>
    void SnapToSurface()
    {
        if (_grid == null) return;
        Vector3 pos = transform.position;
        int cx = Mathf.FloorToInt(pos.x / _grid.BlockSize);
        int cz = Mathf.FloorToInt(pos.z / _grid.BlockSize);
        for (int y = Mathf.FloorToInt(pos.y / _grid.BlockSize); y >= 0; y--)
        {
            BlockType b = _grid.GetBlock(cx, y, cz);
            if (b != BlockType.Air && b != BlockType.Water)
            {
                float surfaceY = (y + 1) * _grid.BlockSize + 0.1f;
                if (Mathf.Abs(transform.position.y - surfaceY) > 0.5f)
                    transform.position = new Vector3(pos.x, surfaceY, pos.z);
                return;
            }
        }
    }

    /// <summary>
    /// Checks if the character's bounding box can move to the target position.
    /// Tests 5 points (center + 4 corners at half-width) against grid.
    /// </summary>
    static int _blockLogFrame;
    bool CanMoveTo(Vector3 pos, float halfW = 0.35f, bool checkGround = true)
    {
        if (_grid == null) return true;

        Vector3[] checkPoints = new Vector3[]
        {
            pos,                                                   // center
            pos + new Vector3( halfW, 0,  halfW), // front-right
            pos + new Vector3( halfW, 0, -halfW), // back-right
            pos + new Vector3(-halfW, 0,  halfW), // front-left
            pos + new Vector3(-halfW, 0, -halfW), // back-left
        };

        foreach (Vector3 pt in checkPoints)
        {
            Vector3Int gp = _grid.WorldToGrid(pt);
            BlockType block = _grid.GetBlock(gp.x, gp.y, gp.z);
            if (block != BlockType.Air && block != BlockType.Water)
                return false;
        }

        // Also check ground exists below (don't walk off cliffs)
        if (checkGround)
        {
            Vector3Int below = _grid.WorldToGrid(pos + Vector3.down * 1f);
            BlockType ground = _grid.GetBlock(below.x, below.y, below.z);
            if (ground == BlockType.Air || ground == BlockType.Water)
                return false;
        }

        return true;
    }

    void PickWanderTarget()
    {
        for (int i = 0; i < 10; i++)
        {
            Vector3 candidate = transform.position + new Vector3(Random.Range(-2.5f, 2.5f), 0f, Random.Range(-2.5f, 2.5f));
            if (_grid != null)
            {
                Vector3Int gp = _grid.WorldToGrid(candidate);
                if (_grid.GetBlock(gp.x, gp.y, gp.z) != BlockType.Air ||
                    _grid.GetBlock(gp.x, gp.y - 1, gp.z) == BlockType.Air)
                    continue;
            }
            _wanderTarget = candidate;
            return;
        }
        _wanderTarget = transform.position;
    }

    public bool IsMoving => _isMoving;

    /// <summary>
    /// Attacks nearby enemies if colonist is not dead/sleeping.
    /// Uses equipment stats for damage.
    /// </summary>
    void HandleCombat()
    {
        if (_colonist.currentState == ColonistState.Dead || _colonist.currentState == ColonistState.Sleeping) return;

        // Check if colonist has ranged weapon
        Equipment eq = GetComponent<Equipment>();
        bool hasRanged = eq != null && eq.HasRangedWeapon();

        Enemy nearest = null;
        float range = hasRanged ? 10f : 3f;
        float minDist = range;
        Enemy[] enemies = FindObjectsOfType<Enemy>();
        foreach (Enemy e in enemies)
        {
            if (e == null || e.state == CombatState.Dead) continue;
            float d = Vector3.Distance(transform.position, e.transform.position);
            if (d < minDist) { minDist = d; nearest = e; }
        }

        if (nearest != null)
        {
            _colonist.currentState = ColonistState.Fighting;
            _colonist.isDrafted = true;

            Vector3 dir = (nearest.transform.position - transform.position).normalized;
            transform.forward = dir;

            float dist = Vector3.Distance(transform.position, nearest.transform.position);
            float dmg = 5f + (_colonist.meleeSkill * 0.5f);

            if (hasRanged && dist > 2f)
            {
                // Ranged attack
                dmg = 8f + (_colonist.meleeSkill * 0.3f); // ranged uses melee skill for now
                nearest.TakeDamage(dmg * Time.deltaTime * 0.5f, DamageType.Pierce);
            }
            else if (dist <= 2f)
            {
                // Melee attack
                if (eq != null) dmg += eq.GetAttackBonus();
                nearest.TakeDamage(dmg * Time.deltaTime, DamageType.Slash);
            }
        }
        else if (_colonist.isDrafted)
        {
            _colonist.isDrafted = false;
            _colonist.currentState = ColonistState.Idle;
        }
    }

    /// <summary>
    /// Eats food from inventory to restore hunger.
    /// </summary>
    void EatFood()
    {
        Inventory inv = GetComponent<Inventory>();
        if (inv != null)
        {
            // Try to eat any food item
            ItemType[] foods = { ItemType.Berries, ItemType.Wheat, ItemType.Potato, ItemType.Bread, ItemType.RawMeat, ItemType.CookedMeat, ItemType.RationPack };
            foreach (ItemType food in foods)
            {
                if (inv.Has(food, 1))
                {
                    inv.RemoveItem(food, 1);
                    _needs.Eat(30f, 5f);
                    TransitionTo(ColonistState.Eating);
                    return;
                }
            }
        }
        // No food — just wait (hunger keeps dropping)
        TransitionTo(ColonistState.Idle);
    }

    private void EvaluateState()
    {
        if (_colonist.currentState == ColonistState.Incapacitated || _colonist.currentState == ColonistState.Dead) return;
        if (_colonist.isDrafted && _colonist.currentState == ColonistState.Fighting) return;
        if (_colonist.hunger < 10f) { EatFood(); return; }
        if (_colonist.fatigue > 95f) { TransitionTo(ColonistState.Sleeping); return; }
        if (_colonist.hunger < 30f) { EatFood(); return; }
        if (_colonist.fatigue > 70f) { TransitionTo(ColonistState.Sleeping); return; }
        if (_colonist.recreation < 15f && _colonist.currentState != ColonistState.Working) { TransitionTo(ColonistState.Recreation); return; }
        if (HasWork()) TransitionTo(ColonistState.Working);
        else if (_colonist.hunger > 30f && _colonist.fatigue < 70f) TryAutoWork(); // do something useful
        else TransitionTo(ColonistState.Idle);
    }

    private float _workCooldown;

    /// <summary>
    /// Finds a nearby solid block and mines it. Runs every ~2 seconds.
    /// </summary>
    public void TryAutoWork()
    {
        TransitionTo(ColonistState.Idle); // Auto-work disabled — use player RMB orders
        return;
        // OLD AUTO-MINE CODE (destroys blocks around colonist):
        _workCooldown -= Time.deltaTime * (_day != null ? _day.gameSpeed : 1f);
        if (_workCooldown > 0f) { TransitionTo(ColonistState.Idle); return; }
        _workCooldown = Random.Range(1.5f, 3f);

        if (_grid == null) { TransitionTo(ColonistState.Idle); return; }
        Vector3Int myPos = _grid.WorldToGrid(transform.position);
        for (int dx = -2; dx <= 2; dx++)
        {
            for (int dz = -2; dz <= 2; dz++)
            {
                for (int dy = -1; dy <= 1; dy++)
                {
                    int gx = myPos.x + dx, gy = myPos.y + dy, gz = myPos.z + dz;
                    if (!_grid.InBounds(gx, gy, gz)) continue;
                    BlockType b = _grid.GetBlock(gx, gy, gz);
                    if (b != BlockType.Air && b != BlockType.Water && b != BlockType.Bedrock)
                    {
                        _grid.RemoveBlock(gx, gy, gz);
                        _colonist.currentState = ColonistState.Working;
                        currentTask = ColonistTask.Mine;
                        return;
                    }
                }
            }
        }
        TransitionTo(ColonistState.Idle);
    }

    private void TransitionTo(ColonistState s) { if (_colonist.currentState != s) { _colonist.currentState = s; if (s == ColonistState.Sleeping) _colonist.comfort += 20f; } }
    public void AssignTask(ColonistTask t, Vector3 p) { currentTask = t; taskTarget = p; _colonist.currentState = ColonistState.Working; }
    public void CancelTask() { currentTask = ColonistTask.None; TransitionTo(ColonistState.Idle); }
    public void ToggleDraft() { _colonist.isDrafted = !_colonist.isDrafted; _colonist.currentState = _colonist.isDrafted ? ColonistState.Fighting : ColonistState.Idle; _speed = _colonist.isDrafted ? runSpeed : walkSpeed; }
    private bool HasWork() => currentTask != ColonistTask.None;
}

public enum ColonistTask { None, Build, Mine, Harvest, Cook, Craft, Haul, Hunt, Research, Heal, Bury, Plant, CutWood, Fish }
