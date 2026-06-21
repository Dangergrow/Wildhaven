using UnityEngine;

/// <summary>Food spoilage: items decay over time. Cold storage slows it.</summary>
public class FoodSpoilage : MonoBehaviour
{
    private DayCycle _day;
    private TemperatureLight _temp;
    private float _timer;

    void Awake()
    {
        _day = FindObjectOfType<DayCycle>();
        _temp = FindObjectOfType<TemperatureLight>();
    }

    void Update()
    {
        _timer += Time.unscaledDeltaTime;
        if (_timer < 60f) return; // check every minute
        _timer = 0f;

        var spawner = FindObjectOfType<ColonistSpawner>();
        if (spawner == null) return;

        foreach (var c in spawner.Colonists)
        {
            if (c.currentState == ColonistState.Dead) continue;
            var inv = c.GetComponent<Inventory>();
            if (inv == null) continue;

            float temp = _temp != null ? _temp.GetTemperature(c.transform.position) : 20f;
            float spoilRate = GetSpoilRate(temp);

            foreach (var slot in inv.Slots)
            {
                if (slot.amount > 0 && IsPerishable(slot.itemType))
                {
                    // 1% chance per minute to lose 1 item
                    if (Random.value < spoilRate * 0.01f)
                        slot.amount = Mathf.Max(0, slot.amount - 1);
                }
            }
        }
    }

    bool IsPerishable(ItemType t) => t == ItemType.RawMeat || t == ItemType.CookedMeat
        || t == ItemType.Bread || t == ItemType.Berries || t == ItemType.Fish
        || t == ItemType.Mushroom || t == ItemType.Potato || t == ItemType.Wheat;

    float GetSpoilRate(float temp)
    {
        if (temp < 0) return 0.2f; // frozen — very slow
        if (temp < 10) return 0.5f; // cold — slow
        if (temp < 25) return 1f; // room temp — normal
        return 3f; // hot — fast
    }
}
