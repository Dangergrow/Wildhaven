using UnityEngine;
using System.Collections.Generic;

/// <summary>Medicine system: healing, diseases, surgery.</summary>
public class MedicineSystem : MonoBehaviour
{
    public MedicineLevel activeMedicine = MedicineLevel.Herbal;

    public enum MedicineLevel { None, Herbal, Standard, Advanced, Experimental }
    public enum Disease { None, Cold, Flu, Infection, Plague, Dysentery, Malaria, Tetanus }

    [System.Serializable]
    public struct DiseaseData
    {
        public Disease type;
        public float severity; // 0-100, higher = worse
        public float healChance; // per treatment
    }

    /// <summary>Treat a colonist. Returns healing amount (0-50).</summary>
    public float Treat(Colonist colonist, Inventory inventory)
    {
        if (colonist.currentState == ColonistState.Dead) return 0;

        float heal = activeMedicine switch
        {
            MedicineLevel.Herbal => 15f,
            MedicineLevel.Standard => 30f,
            MedicineLevel.Advanced => 50f,
            MedicineLevel.Experimental => Random.value < 0.3f ? 80f : 5f, // risky!
            _ => 5f,
        };

        colonist.health = Mathf.Min(colonist.maxHealth, colonist.health + heal);
        return heal;
    }

    /// <summary>Attempt surgery. Higher risk, higher reward.</summary>
    public bool Surgery(Colonist colonist, string operation)
    {
        float successChance = activeMedicine == MedicineLevel.Advanced ? 0.8f : 0.5f;
        if (Random.value < successChance)
        {
            colonist.health = colonist.maxHealth;
            return true;
        }
        colonist.health -= 20;
        if (colonist.health <= 0)
        {
            colonist.health = 0;
            colonist.currentState = ColonistState.Dead;
        }
        return false;
    }

    /// <summary>Check if a disease progresses.</summary>
    public static float GetDiseaseProgress(Disease d, float severity) => d switch
    {
        Disease.Plague => 2f * severity / 100f,
        Disease.Infection => 1.5f * severity / 100f,
        Disease.Flu => 0.5f * severity / 100f,
        _ => 0.3f * severity / 100f,
    };
}
