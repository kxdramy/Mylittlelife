
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class InteractionHintUI : MonoBehaviour
{
    [Header("Player")]
    [SerializeField]
    private ManualInteraction manualInteraction;

    [Header("UI")]
    [SerializeField]
    private GameObject hintPanel;

    [SerializeField]
    private TMP_Text hintText;

    [SerializeField]
    private Slider progressSlider;

    private void Update()
    {
        if (manualInteraction == null ||
            hintPanel == null ||
            hintText == null)
        {
            return;
        }

        // Пока персонаж выполняет действие,
        // показываем полоску прогресса.
        if (manualInteraction.IsBusy)
        {
            hintPanel.SetActive(true);

            hintText.text = "Выполняется действие...";

            if (progressSlider != null)
            {
                progressSlider.gameObject.SetActive(true);
                progressSlider.SetValueWithoutNotify(
                    manualInteraction.ActionProgress
                );
            }

            return;
        }

        // Если действие не выполняется,
        // прячем полоску прогресса.
        if (progressSlider != null)
        {
            progressSlider.gameObject.SetActive(false);
        }

        NeedAction action =
            manualInteraction.GetAvailableAction();

        if (action == null)
        {
            hintPanel.SetActive(false);
            return;
        }

        string description;

        switch (action.ActionName)
        {
            case "Eat":
                description = "поесть";
                break;

            case "Wash":
                description = "принять душ";
                break;

            case "Sleep":
                description = "поспать";
                break;

            case "Work":
                description = "поработать";
                break;

            case "Relax":
                description = "отдохнуть";
                break;

            default:
                description = "взаимодействовать";
                break;
        }

        hintText.text =
            "Нажмите E, чтобы " + description;

        hintPanel.SetActive(true);
    }
}
