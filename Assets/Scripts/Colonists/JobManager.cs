using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Manages job assignments for all colonists.
/// Finds idle colonists and assigns pending work.
/// </summary>
public class JobManager : MonoBehaviour
{
    [Header("References")]
    public ColonistSpawner colonistSpawner;

    [Header("Job Queue")]
    [Tooltip("Maximum number of active jobs")]
    public int maxActiveJobs = 10;

    // Pending jobs awaiting assignment
    private Queue<Job> _pendingJobs = new Queue<Job>();

    // Currently assigned jobs
    private List<Job> _activeJobs = new List<Job>();

    private void Update()
    {
        if (colonistSpawner == null) return;

        AssignPendingJobs();
    }

    /// <summary>
    /// Creates a new job and queues it.
    /// </summary>
    public void CreateJob(ColonistTask taskType, Vector3 targetPosition, BlockType blockType = BlockType.Air, int priority = 3)
    {
        if (_pendingJobs.Count >= maxActiveJobs * 2) return; // don't overflow queue

        Job job = new Job
        {
            taskType = taskType,
            targetPosition = targetPosition,
            blockType = blockType,
            priority = priority,
        };

        _pendingJobs.Enqueue(job);
    }

    /// <summary>
    /// Assigns pending jobs to the best available colonists.
    /// </summary>
    private void AssignPendingJobs()
    {
        if (_pendingJobs.Count == 0) return;
        if (_activeJobs.Count >= maxActiveJobs) return;

        // Find idle colonists
        foreach (Colonist colonist in colonistSpawner.Colonists)
        {
            if (colonist == null) continue;

            ColonistAI ai = colonist.GetComponent<ColonistAI>();
            if (ai == null) continue;

            if (colonist.currentState != ColonistState.Idle &&
                colonist.currentState != ColonistState.Working) continue;

            if (_pendingJobs.Count == 0) break;

            Job job = _pendingJobs.Dequeue();

            // Match job to colonist skill
            SkillType requiredSkill = GetSkillForTask(job.taskType);
            if (colonist.GetSkill(requiredSkill) < 1) continue; // unskilled — skip

            ai.AssignTask(job.taskType, job.targetPosition);
            _activeJobs.Add(job);
        }
    }

    /// <summary>
    /// Marks a job as completed or cancelled.
    /// </summary>
    public void CompleteJob(ColonistAI colonistAI)
    {
        _activeJobs.RemoveAll(j => j.targetPosition == colonistAI.taskTarget);
        colonistAI.CancelTask();
    }

    /// <summary>
    /// Maps task type to required skill.
    /// </summary>
    private SkillType GetSkillForTask(ColonistTask task)
    {
        return task switch
        {
            ColonistTask.Build => SkillType.Construction,
            ColonistTask.Mine => SkillType.Mining,
            ColonistTask.Harvest => SkillType.Farming,
            ColonistTask.Cook => SkillType.Cooking,
            ColonistTask.Craft => SkillType.Crafting,
            ColonistTask.Haul => SkillType.Construction,
            ColonistTask.Hunt => SkillType.Hunting,
            ColonistTask.Research => SkillType.Intellectual,
            ColonistTask.Heal => SkillType.Medicine,
            ColonistTask.Bury => SkillType.Construction,
            ColonistTask.Plant => SkillType.Farming,
            ColonistTask.CutWood => SkillType.Mining,
            ColonistTask.Fish => SkillType.Hunting,
            _ => SkillType.Construction,
        };
    }
}

/// <summary>
/// Represents a pending or active job.
/// </summary>
public class Job
{
    public ColonistTask taskType;
    public Vector3 targetPosition;
    public BlockType blockType;
    public int priority;
    public float workDuration;
    public float workProgress;
}
