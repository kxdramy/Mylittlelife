
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class CharacterActionSounds : MonoBehaviour
{
    [Header("Action Sounds")]
    [SerializeField] private AudioClip eatSound;
    [SerializeField] private AudioClip washSound;
    [SerializeField] private AudioClip sleepSound;
    [SerializeField] private AudioClip workSound;
    [SerializeField] private AudioClip relaxSound;

    [Header("Distance")]
    [SerializeField] private float manualRadius = 1.5f;
    [SerializeField] private float autoRadius = 1.2f;

    private AudioSource audioSource;
    private ManualInteraction manualInteraction;
    private ControlModeManager controlMode;
    private UtilityAI utilityAI;
    private NeedAction[] actions;

    private string previousAction;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();

        manualInteraction = GetComponent<ManualInteraction>();
        controlMode = GetComponent<ControlModeManager>();
        utilityAI = GetComponent<UtilityAI>();

        actions = GetComponents<NeedAction>();
    }

    private void Update()
    {
        string currentAction = null;

        if (controlMode != null && controlMode.IsAutoMode)
        {
            string aiAction = utilityAI != null
                ? utilityAI.CurrentAction
                : null;

            if (!string.IsNullOrEmpty(aiAction) &&
                aiAction != "Ожидание")
            {
                NeedAction nearby =
                    FindNearbyAction(autoRadius, aiAction);

                if (nearby != null)
                    currentAction = nearby.ActionName;
            }
        }
        else if (manualInteraction != null &&
                 manualInteraction.IsBusy)
        {
            NeedAction nearby =
                FindNearbyAction(manualRadius, null);

            if (nearby != null)
                currentAction = nearby.ActionName;
        }

        // Запускаем звук один раз в начале действия.
        if (currentAction != previousAction)
        {
            if (!string.IsNullOrEmpty(currentAction))
                PlayActionSound(currentAction);

            previousAction = currentAction;
        }
    }

    private NeedAction FindNearbyAction(
        float radius, string requiredName)
    {
        NeedAction nearest = null;
        float bestDistance = radius * radius;

        foreach (NeedAction action in actions)
        {
            if (action == null ||
                action.InteractionPoint == null)
                continue;

            if (requiredName != null &&
                action.ActionName != requiredName)
                continue;

            Vector3 difference =
                action.InteractionPoint.position - transform.position;

            difference.y = 0f;

            float distance = difference.sqrMagnitude;

            if (distance <= bestDistance)
            {
                bestDistance = distance;
                nearest = action;
            }
        }

        return nearest;
    }

    private void PlayActionSound(string actionName)
    {
        AudioClip clip = null;

        switch (actionName)
        {
            case "Eat": clip = eatSound; break;
            case "Wash": clip = washSound; break;
            case "Sleep": clip = sleepSound; break;
            case "Work": clip = workSound; break;
            case "Relax": clip = relaxSound; break;
        }

        if (clip != null)
        {
            audioSource.PlayOneShot(clip);
            Debug.Log("Action sound: " + actionName);
        }
    }
}
