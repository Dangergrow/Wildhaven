using UnityEngine;
using UnityEngine.AI;

/// <summary>
/// AI controller for colonists. Handles movement, task execution, and state transitions.
/// Uses Unity NavMesh for pathfinding.
/// </summary>
[RequireComponent(typeof(Colonist), typeof(NeedsSystem), typeof(NavMeshAgent))]
public class ColonistAI : MonoBehaviour
{
    [Header("Movement")]
    [Tooltip("Normal movement speed (m/s)")]
    public float walkSpeed = 3f;

    [Tooltip("Running speed when drafted or urgent")]
    public float runSpeed = 6f;

    [Header("Work")]
    [Tooltip("Current assigned task")]
    public ColonistTask currentTask;

    [Tooltip("Target position for current task")]
    public Vector3 taskTarget;

    [Tooltip("Job priorities (1-4, lower = more important). Index matches SkillType enum.")]
    public int[] jobPriorities = new int[14] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };

    // Component references
    private Colonist _colonist;
    private NeedsSystem _needs;
    private NavMeshAgent _agent;

    private void Awake()
    {
        _colonist = GetComponent<Colonist>();
        _needs = GetComponent<NeedsSystem>();
        _agent = GetComponent<NavMeshAgent>();

        _agent.speed = walkSpeed;
        _agent.acceleration = 8f;
        _agent.angularSpeed = 360f;
        _agent.stoppingDistance = 0.2f;
    }

    private void Update()
    {
        if (_colonist == null || _colonist.currentState == ColonistState.Dead) return;

        UpdateAISpeed();
        EvaluateState();
    }

    /// <summary>
    /// Sets movement speed based on state.
    /// </summary>
    private void UpdateAISpeed()
    {
        _agent.speed = (_colonist.isDrafted || _colonist.isPriorityTask) ? runSpeed : walkSpeed;
    }

    /// <summary>
    /// Main AI evaluation loop. Checks needs and transitions between states.
    /// Priority order: dying > eating > sleeping > fighting > working > recreation > idling.
    /// </summary>
    private void EvaluateState()
    {
        // If incapacitated or dead, do nothing
        if (_colonist.currentState == ColonistState.Incapacitated ||
            _colonist.currentState == ColonistState.Dead)
            return;

        // Priority 1: Drafted/fighting — controlled by player, skip needs
        if (_colonist.isDrafted && _colonist.currentState == ColonistState.Fighting)
            return;

        // Priority 2: Critical needs override everything
        if (_colonist.hunger < 10f) // starving — go eat NOW
        {
            TransitionTo(ColonistState.Eating);
            return;
        }

        if (_colonist.fatigue > 95f) // about to collapse
        {
            TransitionTo(ColonistState.Sleeping);
            return;
        }

        // Priority 3: Regular needs during idle/work states
        if (_colonist.hunger < 30f)
        {
            TransitionTo(ColonistState.Eating);
            return;
        }

        if (_colonist.fatigue > 70f)
        {
            TransitionTo(ColonistState.Sleeping);
            return;
        }

        // Priority 4: Recreation if bored
        if (_colonist.recreation < 15f && _colonist.currentState != ColonistState.Working)
        {
            TransitionTo(ColonistState.Recreation);
            return;
        }

        // Default: work or idle
        if (HasWork())
            TransitionTo(ColonistState.Working);
        else
            TransitionTo(ColonistState.Idle);
    }

    /// <summary>
    /// Transitions colonist to a new state.
    /// </summary>
    private void TransitionTo(ColonistState newState)
    {
        if (_colonist.currentState == newState) return;
        _colonist.currentState = newState;
        OnStateEnter(newState);
    }

    /// <summary>
    /// State entry logic.
    /// </summary>
    private void OnStateEnter(ColonistState state)
    {
        switch (state)
        {
            case ColonistState.Sleeping:
                _agent.isStopped = true;
                _colonist.comfort += 20f; // bed comfort bonus
                break;
            case ColonistState.Eating:
                _agent.isStopped = true;
                break;
            case ColonistState.Idle:
            case ColonistState.Working:
                _agent.isStopped = false;
                break;
        }
    }

    /// <summary>
    /// Orders colonist to move to a position.
    /// </summary>
    public void MoveTo(Vector3 destination)
    {
        _agent.isStopped = false;
        _agent.SetDestination(destination);
        _colonist.currentState = ColonistState.Moving;
    }

    /// <summary>
    /// Assigns a task to the colonist.
    /// </summary>
    public void AssignTask(ColonistTask task, Vector3 target)
    {
        currentTask = task;
        taskTarget = target;
        MoveTo(target);
    }

    /// <summary>
    /// Cancels current task.
    /// </summary>
    public void CancelTask()
    {
        currentTask = ColonistTask.None;
        _agent.isStopped = true;
        TransitionTo(ColonistState.Idle);
    }

    /// <summary>
    /// Toggle combat/draft mode.
    /// </summary>
    public void ToggleDraft()
    {
        _colonist.isDrafted = !_colonist.isDrafted;
        if (_colonist.isDrafted)
        {
            _colonist.currentState = ColonistState.Fighting;
            _agent.speed = runSpeed;
        }
        else
        {
            TransitionTo(ColonistState.Idle);
            _agent.speed = walkSpeed;
        }
    }

    /// <summary>
    /// Checks if colonist has work assigned.
    /// </summary>
    private bool HasWork()
    {
        return currentTask != ColonistTask.None;
    }
}

/// <summary>
/// Types of tasks colonists can perform.
/// </summary>
public enum ColonistTask
{
    None,
    Build,
    Mine,
    Harvest,
    Cook,
    Craft,
    Haul,
    Hunt,
    Research,
    Heal,
    Bury,
    Plant,
    CutWood,
    Fish,
}
