
using UnityEngine;

public class CharacterActionAnimation : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform characterVisual;
    [SerializeField] private ManualInteraction manualInteraction;

    [Header("Animation Settings")]
    [SerializeField] private float bounceHeight = 0.12f;
    [SerializeField] private float squashAmount = 0.08f;
    [SerializeField] private float animationSpeed = 18f;

    [Header("Auto Mode")]
    [SerializeField] private float autoActionRadius = 1.2f;

    private UtilityAI utilityAI;
    private ControlModeManager controlMode;
    private NeedAction[] actions;

    private Vector3 originalPosition;
    private Vector3 originalScale;
    private bool initialized;

    private void Awake()
    {
        utilityAI = GetComponent<UtilityAI>();
        controlMode = GetComponent<ControlModeManager>();
        actions = GetComponents<NeedAction>();
    }

    private void Start()
    {
        if (characterVisual == null)
        {
            Debug.LogError(
                "CharacterVisual is not assigned!"
            );

            enabled = false;
            return;
        }

        originalPosition = characterVisual.localPosition;
        originalScale = characterVisual.localScale;

        initialized = true;
    }

    private void LateUpdate()
    {
        if (!initialized)
            return;

        bool manualActing =
            manualInteraction != null &&
            manualInteraction.IsBusy;

        bool autoActing = IsAutoActing();

        bool isActing = manualActing || autoActing;

        if (isActing)
        {
            float wave =
                Mathf.Sin(Time.time * animationSpeed);

            characterVisual.localPosition =
                originalPosition +
                Vector3.up *
                Mathf.Abs(wave) *
                bounceHeight;

            characterVisual.localScale = new Vector3(
                originalScale.x * (1f + squashAmount * wave),
                originalScale.y * (1f - squashAmount * wave),
                originalScale.z * (1f + squashAmount * wave)
            );
        }
        else
        {
            characterVisual.localPosition =
                Vector3.Lerp(
                    characterVisual.localPosition,
                    originalPosition,
                    Time.deltaTime * 12f
                );

            characterVisual.localScale =
                Vector3.Lerp(
                    characterVisual.localScale,
                    originalScale,
                    Time.deltaTime * 12f
                );
        }
    }

    private bool IsAutoActing()
    {
        if (controlMode == null ||
            utilityAI == null ||
            !controlMode.IsAutoMode)
        {
            return false;
        }

        string currentAction = utilityAI.CurrentAction;

        if (string.IsNullOrEmpty(currentAction) ||
            currentAction == "Ожидание")
        {
            return false;
        }

        foreach (NeedAction action in actions)
        {
            if (action.ActionName != currentAction ||
                action.InteractionPoint == null)
            {
                continue;
            }

            Vector3 distance =
                action.InteractionPoint.position -
                transform.position;

            distance.y = 0f;

            return distance.sqrMagnitude <=
                autoActionRadius * autoActionRadius;
        }

        return false;
    }

    private void OnDisable()
    {
        if (!initialized || characterVisual == null)
            return;

        characterVisual.localPosition = originalPosition;
        characterVisual.localScale = originalScale;
    }
}
