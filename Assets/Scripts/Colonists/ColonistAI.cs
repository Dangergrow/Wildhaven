using UnityEngine;

/// <summary>
/// AI controller for colonists. Manages state transitions, wandering, and auto-collect.
/// </summary>
[RequireComponent(typeof(Colonist), typeof(NeedsSystem))]
public class ColonistAI : MonoBehaviour
{
    public float walkSpeed = 3f;
    public float runSpeed = 6f;
    public ColonistTask currentTask;
    public Vector3 taskTarget;
    public int[] jobPriorities = new int[14] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };

    private Colonist _colonist;
    private NeedsSystem _needs;
    private float _speed;
    private Vector3 _wanderTarget;
    private float _wanderTimer;
    private bool _isMoving;

    private void Awake()
    {
        _colonist = GetComponent<Colonist>();
        _needs = GetComponent<NeedsSystem>();
        _speed = walkSpeed;
        PickWanderTarget();
    }

    private void Update()
    {
        if (_colonist == null || _colonist.currentState == ColonistState.Dead) return;
        DayCycle day = FindObjectOfType<DayCycle>();
        if (day != null && day.IsPaused) return;
        EvaluateState();
        HandleWandering();
        HandleCombat();
    }

    void HandleWandering()
    {
        if (_colonist.currentState == ColonistState.Dead || _colonist.currentState == ColonistState.Sleeping || _colonist.currentState == ColonistState.Fighting) return;
        if (_colonist.currentState != ColonistState.Idle && _colonist.currentState != ColonistState.Moving) return;

        _wanderTimer -= Time.deltaTime * (day != null ? day.gameSpeed : 1f);
        if (_wanderTimer <= 0f) { PickWanderTarget(); _wanderTimer = Random.Range(1.5f, 4f); }

        float dist = Vector3.Distance(transform.position, _wanderTarget);
        if (dist > 0.3f)
        {
            Vector3 dir = (_wanderTarget - transform.position).normalized;
            Vector3 nextPos = transform.position + dir * _speed * 0.4f * Time.deltaTime * (day != null ? day.gameSpeed : 1f);
            // Only move if next position is air
            GridManager grid = FindObjectOfType<GridManager>();
            bool canMove = true;
            if (grid != null)
            {
                Vector3Int gp = grid.WorldToGrid(nextPos);
                if (grid.GetBlock(gp.x, gp.y, gp.z) != BlockType.Air)
                    canMove = false;
            }
            if (canMove)
            {
                transform.position = nextPos;
                _isMoving = true;
            }
            else PickWanderTarget(); // blocked — find new target
        }
        else _isMoving = false;
    }

    void PickWanderTarget()
    {
        GridManager grid = FindObjectOfType<GridManager>();
        for (int i = 0; i < 10; i++) // try valid positions
        {
            Vector3 candidate = transform.position + new Vector3(Random.Range(-2.5f, 2.5f), 0f, Random.Range(-2.5f, 2.5f));
            if (grid != null)
            {
                Vector3Int gp = grid.WorldToGrid(candidate);
                if (grid.GetBlock(gp.x, gp.y, gp.z) != BlockType.Air ||
                    grid.GetBlock(gp.x, gp.y - 1, gp.z) == BlockType.Air) // need solid ground
                    continue;
            }
            _wanderTarget = candidate;
            return;
        }
        _wanderTarget = transform.position; // stay put
    }

    public bool IsMoving => _isMoving;

    /// <summary>
    /// Attacks nearby enemies if colonist is not dead/sleeping.
    /// Uses equipment stats for damage.
    /// </summary>
    void HandleCombat()
    {
        if (_colonist.currentState == ColonistState.Dead || _colonist.currentState == ColonistState.Sleeping) return;

        Enemy nearest = null;
        float minDist = 3f;
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

            // Face enemy
            Vector3 dir = (nearest.transform.position - transform.position).normalized;
            transform.forward = dir;

            // Attack
            Equipment eq = GetComponent<Equipment>();
            float dmg = 5f + (_colonist.meleeSkill * 0.5f);
            if (eq != null) dmg += eq.GetAttackBonus();

            nearest.TakeDamage(dmg * Time.deltaTime, DamageType.Slash);
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
        else TransitionTo(ColonistState.Idle);
    }

    private void TransitionTo(ColonistState s) { if (_colonist.currentState != s) { _colonist.currentState = s; if (s == ColonistState.Sleeping) _colonist.comfort += 20f; } }
    public void AssignTask(ColonistTask t, Vector3 p) { currentTask = t; taskTarget = p; _colonist.currentState = ColonistState.Working; }
    public void CancelTask() { currentTask = ColonistTask.None; TransitionTo(ColonistState.Idle); }
    public void ToggleDraft() { _colonist.isDrafted = !_colonist.isDrafted; _colonist.currentState = _colonist.isDrafted ? ColonistState.Fighting : ColonistState.Idle; _speed = _colonist.isDrafted ? runSpeed : walkSpeed; }
    private bool HasWork() => currentTask != ColonistTask.None;
}

public enum ColonistTask { None, Build, Mine, Harvest, Cook, Craft, Haul, Hunt, Research, Heal, Bury, Plant, CutWood, Fish }
