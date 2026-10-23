
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NeedAction : MonoBehaviour, IUtilityAction
{
    [Header("Action Settings")]
    [SerializeField] private string actionName = "Eat";
    [SerializeField] private NeedType needType = NeedType.Satiety;

    [Header("Target")]
    [SerializeField] private Transform target;

    [Header("Effect")]
    [SerializeField] private float restoreAmount = 40f;
    [SerializeField] private float actionDuration = 2f;

    private NavMeshAgent agent;

    public string ActionName => actionName;
    
    public Transform InteractionPoint => target;

    public NeedType AffectedNeed => needType;

    public float RecoveryAmount => restoreAmount;

    public float Duration => actionDuration;


    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    
    public bool IsAvailable()
    {
        if (target == null)
        {
            Debug.LogWarning(actionName + ": Target не назначен!");
            return false;
        }

        if (!target.gameObject.activeInHierarchy)
        {
            Debug.LogWarning(actionName + ": Target выключен!");
            return false;
        }

        if (agent == null || !agent.enabled || !agent.isOnNavMesh)
        {
            Debug.LogWarning(actionName + ": NavMeshAgent недоступен!");
            return false;
        }

        if (!NavMesh.SamplePosition(
            target.position,
            out NavMeshHit hit,
            1.5f,
            NavMesh.AllAreas))
        {
            Debug.LogWarning(actionName + ": Точка находится вне NavMesh!");
            return false;
        }

        NavMeshPath path = new NavMeshPath();

        bool found = agent.CalculatePath(hit.position, path);

        if (!found || path.status != NavMeshPathStatus.PathComplete)
        {
            Debug.LogWarning(
                actionName + ": Нет полного пути! Статус: " + path.status
            );
            return false;
        }

        return true;
    }


    public float GetUtility(INeedReader needs)
    {
        return 100f - needs.GetValue(needType);
    }

    public IEnumerator Execute(NPCNeeds needs, NPCMovement movement)
    {
        if (!IsAvailable())
            yield break;

        Debug.Log("NPC going to: " + actionName);

        yield return movement.MoveTo(target);

        if (target == null)
            yield break;

        Vector3 difference = transform.position - target.position;
        difference.y = 0f;

        if (difference.magnitude > 1.0f)
        {
            Debug.LogWarning("NPC could not reach " + actionName);
            yield break;
        }

        Debug.Log("NPC performing: " + actionName);

        yield return new WaitForSeconds(actionDuration);

        needs.ChangeNeed(needType, restoreAmount);

        Debug.Log(actionName + " completed! +" + restoreAmount);
    }
}
