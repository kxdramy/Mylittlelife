
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NeedsUI : MonoBehaviour
{
    [Header("Character")]
    [SerializeField] private NPCNeeds needs;

    [Header("Needs Sliders")]
    [SerializeField] private Slider satietySlider;
    [SerializeField] private Slider hygieneSlider;
    [SerializeField] private Slider sleepSlider;
    [SerializeField] private Slider workSlider;
    [SerializeField] private Slider leisureSlider;

    [Header("Need Values")]
    [SerializeField] private TMP_Text satietyValue;
    [SerializeField] private TMP_Text hygieneValue;
    [SerializeField] private TMP_Text sleepValue;
    [SerializeField] private TMP_Text workValue;
    [SerializeField] private TMP_Text leisureValue;

    private void Update()
    {
        if (needs == null)
            return;

        UpdateNeed(satietySlider, satietyValue,
            NeedType.Satiety);

        UpdateNeed(hygieneSlider, hygieneValue,
            NeedType.Hygiene);

        UpdateNeed(sleepSlider, sleepValue,
            NeedType.Sleep);

        UpdateNeed(workSlider, workValue,
            NeedType.Work);

        UpdateNeed(leisureSlider, leisureValue,
            NeedType.Leisure);
    }

    private void UpdateNeed(
        Slider slider,
        TMP_Text valueText,
        NeedType type)
    {
        float value = needs.GetValue(type);

        if (slider != null)
        {
            slider.SetValueWithoutNotify(value);
        }

        if (valueText != null)
        {
            valueText.text =
                Mathf.RoundToInt(value) + "/100";
        }
    }
}
