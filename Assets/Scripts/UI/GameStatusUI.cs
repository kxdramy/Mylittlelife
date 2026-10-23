
using UnityEngine;
using TMPro;

public class GameStatusUI : MonoBehaviour
{
    [Header("Player")]
    [SerializeField] private ControlModeManager controlMode;
    [SerializeField] private UtilityAI utilityAI;

    [Header("UI Text")]
    [SerializeField] private TMP_Text modeText;
    [SerializeField] private TMP_Text actionText;

    private void Update()
    {
        if (controlMode == null)
            return;

        bool isAuto = controlMode.IsAutoMode;

        if (modeText != null)
        {
            modeText.text = isAuto
                ? "Режим: AUTO"
                : "Режим: MANUAL";

            modeText.color = isAuto
                ? new Color(0.64f, 0.89f, 0.78f)
                : new Color(0.96f, 0.76f, 0.86f);
        }

        if (actionText != null)
        {
            if (isAuto && utilityAI != null)
            {
                actionText.text =
                    "Действие: " + utilityAI.CurrentAction;
            }
            else
            {
                actionText.text = "Действие: —";
            }
        }
    }
}
