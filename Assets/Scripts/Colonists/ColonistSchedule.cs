using UnityEngine;

/// <summary>24-hour schedule: Sleep/Work/Recreation/Anything per colonist.</summary>
public class ColonistSchedule : MonoBehaviour
{
    public enum Block { Anything, Work, Recreation, Sleep }
    public Block[] schedule = new Block[24]; // 0-23 hours

    void Awake()
    {
        // Default schedule: 0-5 sleep, 6-7 anything, 8-17 work, 18-21 recreation, 22-23 sleep
        for (int h = 0; h < 24; h++)
        {
            if (h < 6 || h >= 22) schedule[h] = Block.Sleep;
            else if (h < 8) schedule[h] = Block.Anything;
            else if (h < 18) schedule[h] = Block.Work;
            else schedule[h] = Block.Recreation;
        }
    }

    /// <summary>Get what colonist should do at current hour.</summary>
    public Block GetCurrentBlock(int hour) => schedule[Mathf.Clamp(hour, 0, 23)];

    /// <summary>Apply schedule to colonist state.</summary>
    public void ApplySchedule(Colonist colonist, int hour)
    {
        if (colonist.currentState == ColonistState.Dead || colonist.currentState == ColonistState.Incapacitated) return;
        if (colonist.currentState == ColonistState.Fighting) return; // combat overrides
        if (colonist.isDrafted) return;

        Block b = GetCurrentBlock(hour);
        switch (b)
        {
            case Block.Sleep:
                if (colonist.currentState != ColonistState.Sleeping)
                    colonist.currentState = ColonistState.Sleeping;
                break;
            case Block.Recreation:
                if (colonist.recreation < 50 && colonist.currentState != ColonistState.Sleeping)
                    colonist.currentState = ColonistState.Idle; // auto-recreate via RecreationManager
                break;
            case Block.Work:
                if (colonist.currentState != ColonistState.Sleeping)
                {
                    var ai = GetComponent<ColonistAI>();
                    if (ai != null) ai.TryAutoWork(); // work by priorities
                }
                break;
        }
    }
}
