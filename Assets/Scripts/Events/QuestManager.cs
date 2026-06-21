using UnityEngine;
using System.Collections.Generic;

/// <summary>Quest system: rescue, deliver, destroy, build quests with rewards.</summary>
public class QuestManager : MonoBehaviour
{
    public List<Quest> activeQuests = new();
    public List<Quest> completedQuests = new();
    private float _timer;

    public enum QuestType { Rescue, Deliver, Destroy, Build, FactionRequest }
    public enum QuestState { Active, Completed, Failed }

    [System.Serializable]
    public class Quest
    {
        public string title;
        public string description;
        public QuestType type;
        public QuestState state;
        public int rewardCopper;
        public int timeLimit; // in game days, 0 = no limit
        public float startDay;
        public Vector3Int targetPos; // where to go
        public int targetFactionId; // which faction
        public BlockType buildType; // for Build quests
        public int buildCount; // how many to build
        public int progress; // current progress
    }

    private string[] _questTitles = { "Rescue the Refugee", "Deliver Supplies", "Destroy Bandit Camp", "Build a Monument", "Faction Favor" };
    private string[] _questDescs = {
        "A survivor is stranded nearby. Send a colonist to rescue them before they perish.",
        "A trade caravan needs supplies delivered. Transport the goods to the marked location.",
        "Bandits have set up camp nearby. Destroy their base to make the region safe.",
        "The local faction wants a monument built. Construct it at the marked location.",
        "A faction requests your help. Complete the task to improve relations."
    };

    void Update()
    {
        _timer += Time.unscaledDeltaTime;
        if (_timer > 30f) { _timer = 0f; CheckQuestTimeouts(); TryGenerateQuest(); }
    }

    void TryGenerateQuest()
    {
        if (activeQuests.Count >= 3) return; // max 3 active quests
        int type = Random.Range(0, _questTitles.Length);
        var q = new Quest
        {
            title = _questTitles[type],
            description = _questDescs[type],
            type = (QuestType)type,
            state = QuestState.Active,
            rewardCopper = Random.Range(50, 500),
            timeLimit = Random.Range(3, 10),
            startDay = FindObjectOfType<DayCycle>()?.day ?? 1,
            targetPos = new Vector3Int(Random.Range(5, 95), 10, Random.Range(5, 95)),
            buildType = BlockType.StoneBrick,
            buildCount = Random.Range(3, 10),
        };
        activeQuests.Add(q);
        Debug.Log($"[Quest] New: {q.title} — {q.description} Reward: {q.rewardCopper}c");
    }

    void CheckQuestTimeouts()
    {
        var day = FindObjectOfType<DayCycle>();
        if (day == null) return;
        for (int i = activeQuests.Count - 1; i >= 0; i--)
        {
            var q = activeQuests[i];
            if (q.timeLimit > 0 && day.day - q.startDay > q.timeLimit)
            {
                q.state = QuestState.Failed;
                completedQuests.Add(q);
                activeQuests.RemoveAt(i);
                Debug.Log($"[Quest] Failed: {q.title}");
            }
        }
    }

    /// <summary>Colonist arrived at quest location — try to complete.</summary>
    public bool TryCompleteAt(Vector3Int pos, Colonist colonist)
    {
        for (int i = activeQuests.Count - 1; i >= 0; i--)
        {
            var q = activeQuests[i];
            if (Vector3Int.Distance(pos, q.targetPos) > 2) continue;

            switch (q.type)
            {
                case QuestType.Rescue:
                    // Spawn a new colonist
                    var spawner = FindObjectOfType<ColonistSpawner>();
                    if (spawner != null && spawner.colonistPrefab != null)
                    {
                        var go = Instantiate(spawner.colonistPrefab, colonist.transform.position + Vector3.right, Quaternion.identity);
                        var c = go.GetComponent<Colonist>();
                        c.colonistName = "Refugee";
                        spawner.Colonists.Add(c);
                    }
                    break;
                case QuestType.Deliver:
                    var inv = colonist.GetComponent<Inventory>();
                    if (inv != null) inv.AddItem(ItemType.RationPack, 3);
                    break;
                case QuestType.Destroy:
                    // Destroy blocks around target
                    var gm = FindObjectOfType<GridManager>();
                    if (gm != null)
                    {
                        for (int dx = -2; dx <= 2; dx++)
                        for (int dz = -2; dz <= 2; dz++)
                        for (int dy = -1; dy <= 1; dy++)
                            gm.RemoveBlock(q.targetPos.x + dx, q.targetPos.y + dy, q.targetPos.z + dz);
                    }
                    break;
                case QuestType.Build:
                    q.progress++;
                    if (q.progress >= q.buildCount) break;
                    return false; // not done yet
            }

            // Complete quest!
            var econ = FindObjectOfType<EconomyManager>();
            if (econ != null) econ.ModifyMoney(q.rewardCopper);
            q.state = QuestState.Completed;
            completedQuests.Add(q);
            activeQuests.RemoveAt(i);
            Debug.Log($"[Quest] Completed: {q.title} +{q.rewardCopper}c");
            return true;
        }
        return false;
    }
}
