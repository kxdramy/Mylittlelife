
using UnityEngine;

public enum NeedType
{
    Satiety,
    Hygiene,
    Sleep,
    Work,
    Leisure
}

public interface INeedReader
{
    float GetValue(NeedType type);
}

public interface INeedWriter
{
    void ChangeNeed(NeedType type, float amount);
}

public class NPCNeeds : MonoBehaviour, INeedReader, INeedWriter
{
    [Header("Needs (0-100)")]
    [SerializeField, Range(0f, 100f)]
    private float satiety = 50f;

    [SerializeField, Range(0f, 100f)]
    private float hygiene = 50f;

    [SerializeField, Range(0f, 100f)]
    private float sleep = 50f;

    [SerializeField, Range(0f, 100f)]
    private float work = 50f;

    [SerializeField, Range(0f, 100f)]
    private float leisure = 50f;

    [Header("Decay Rates")]
    [SerializeField] private float satietyDecay = 0.30f;
    [SerializeField] private float hygieneDecay = 0.12f;
    [SerializeField] private float sleepDecay = 0.20f;
    [SerializeField] private float workDecay = 0.08f;
    [SerializeField] private float leisureDecay = 0.25f;

    private void Awake()
    {
        foreach (NeedType type in System.Enum.GetValues(typeof(NeedType)))
        {
            ChangeNeed(type, 0f);
        }
    }

    
    private void Update()
    {
        ChangeNeed(NeedType.Satiety,
            -Mathf.Max(0f, satietyDecay) * Time.deltaTime);

        ChangeNeed(NeedType.Hygiene,
            -Mathf.Max(0f, hygieneDecay) * Time.deltaTime);

        ChangeNeed(NeedType.Sleep,
            -Mathf.Max(0f, sleepDecay) * Time.deltaTime);

        ChangeNeed(NeedType.Work,
            -Mathf.Max(0f, workDecay) * Time.deltaTime);

        ChangeNeed(NeedType.Leisure,
            -Mathf.Max(0f, leisureDecay) * Time.deltaTime);
    }


    public float GetValue(NeedType type)
    {
        switch (type)
        {
            case NeedType.Satiety: return satiety;
            case NeedType.Hygiene: return hygiene;
            case NeedType.Sleep: return sleep;
            case NeedType.Work: return work;
            case NeedType.Leisure: return leisure;
            default: return 0f;
        }
    }

    public void ChangeNeed(NeedType type, float amount)
    {
        float value = Mathf.Clamp(GetValue(type) + amount, 0f, 100f);

        switch (type)
        {
            case NeedType.Satiety: satiety = value; break;
            case NeedType.Hygiene: hygiene = value; break;
            case NeedType.Sleep: sleep = value; break;
            case NeedType.Work: work = value; break;
            case NeedType.Leisure: leisure = value; break;
        }
    }
}
