
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(NPCNeeds))]
[RequireComponent(typeof(NPCMovement))]
public class UtilityAI : MonoBehaviour
{
    [Header("AI Settings")]
    [SerializeField, Range(0.25f, 1f)]
    private float decisionInterval = 0.5f;

    private NPCNeeds needs;
    private NPCMovement movement;

    private readonly List<IUtilityAction> actions =
        new List<IUtilityAction>();

    private readonly ActionSelector selector =
        new ActionSelector();

    private Coroutine aiRoutine;

    private void Awake()
    {
        needs = GetComponent<NPCNeeds>();
        movement = GetComponent<NPCMovement>();

        NeedAction[] foundActions =
            GetComponents<NeedAction>();

        foreach (NeedAction action in foundActions)
        {
            actions.Add(action);
        }
    }

    public bool StartAutonomy()
    {
        if (aiRoutine != null)
            return true;

        if (actions.Count == 0)
        {
            Debug.LogError("No AI actions configured!");
            return false;
        }

        aiRoutine = StartCoroutine(AILoop());
        return true;
    }

    public void StopAutonomy()
    {
        if (aiRoutine != null)
        {
            StopCoroutine(aiRoutine);
            aiRoutine = null;
        }

        movement.StopMovement();
    }

    private IEnumerator AILoop()
    {
        // Wait for NavMeshAgent to initialize.
        yield return null;

        while (true)
        {
            IUtilityAction bestAction =
                selector.SelectBestAction(actions, needs);

            if (bestAction != null)
            {
                float utility =
                    bestAction.GetUtility(needs);

                Debug.Log(
                    "AI selected: " + bestAction.ActionName +
                    " | Utility: " + utility.ToString("F1")
                );

                yield return bestAction.Execute(
                    needs, movement
                );
            }
            else
            {
                Debug.LogWarning(
                    "No available actions for NPC"
                );
            }

            yield return new WaitForSeconds(decisionInterval);
        }
    }
}
