using UnityEngine;

/// <summary>
/// AI controller for colonists. Manages state transitions and needs evaluation.
/// Movement via NavMeshAgent will be added when NavMesh is stable.
/// </summary>
[RequireComponent(typeof(Colonist), typeof(NeedsSystem))]
public class ColonistAI : MonoBehaviour
{
    [Header("Movement")]
    public float walkSpeed = 3f;
    public float runSpeed = 6f;

    [Header("Work")]
    public ColonistTask currentTask;
    public Vector3 taskTarget;
    public int[] jobPriorities = new int[14] { 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3, 3 };

    private Colonist _colonist;
    private NeedsSystem _needs;
    private float _speed;

    private void Awake()
    {
        _colonist = GetComponent<Colonist>();
        _needs = GetComponent<NeedsSystem>();
        _speed = walkSpeed;
    }

    private void Update()
    {
        if (_colonist == null || _colonist.currentState == ColonistState.Dead) return;
        EvaluateState();
    }

    /// <summary>
    /// AI state evaluation. Checks needs and transitions.
    /// </summary>
    private void EvaluateState()
    {
        if (_colonist.currentState == ColonistState.Incapacitated || _colonist.currentState == ColonistState.Dead)
            return;

        if (_colonist.isDrafted && _colonist.currentState == ColonistState.Fighting)
            return;

        if (_colonist.hunger < 10f) { TransitionTo(ColonistState.Eating); return; }
        if (_colonist.fatigue > 95f) { TransitionTo(ColonistState.Sleeping); return; }
        if (_colonist.hunger < 30f) { TransitionTo(ColonistState.Eating); return; }
        if (_colonist.fatigue > 70f) { TransitionTo(ColonistState.Sleeping); return; }
        if (_colonist.recreation < 15f && _colonist.currentState != ColonistState.Working)
        { TransitionTo(ColonistState.Recreation); return; }

        if (HasWork())
            TransitionTo(ColonistState.Working);
        else
            TransitionTo(ColonistState.Idle);
    }

    private void TransitionTo(ColonistState newState)
    {
        if (_colonist.currentState == newState) return;
        _colonist.currentState = newState;
        OnStateEnter(newState);
    }

    private void OnStateEnter(ColonistState state)
    {
        switch (state)
        {
            case ColonistState.Sleeping: _colonist.comfort += 20f; break;
            case ColonistState.Eating: break;
        }
    }

    public void AssignTask(ColonistTask task, Vector3 target)
    {
        currentTask = task;
        taskTarget = target;
        _colonist.currentState = ColonistState.Working;
    }

    public void CancelTask()
    {
        currentTask = ColonistTask.None;
        TransitionTo(ColonistState.Idle);
    }

    public void ToggleDraft()
    {
        _colonist.isDrafted = !_colonist.isDrafted;
        _colonist.currentState = _colonist.isDrafted ? ColonistState.Fighting : ColonistState.Idle;
        _speed = _colonist.isDrafted ? runSpeed : walkSpeed;
    }

    private bool HasWork() => currentTask != ColonistTask.None;
}

public enum ColonistTask
{
    None, Build, Mine, Harvest, Cook, Craft, Haul, Hunt, Research, Heal, Bury, Plant, CutWood, Fish,
}
