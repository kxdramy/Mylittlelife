
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class NPCMovement : MonoBehaviour
{
    private NavMeshAgent agent;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    public IEnumerator MoveTo(Transform target)
    {
        if (target == null || !agent.enabled || !agent.isOnNavMesh)
        {
            Debug.LogWarning("NPC cannot start moving.");
            yield break;
        }

        if (!NavMesh.SamplePosition(
            target.position, out NavMeshHit hit, 1f, NavMesh.AllAreas))
        {
            Debug.LogWarning("Target is not near NavMesh.");
            yield break;
        }

        agent.isStopped = false;

        if (!agent.SetDestination(hit.position))
        {
            Debug.LogWarning("Could not set destination.");
            yield break;
        }

        float elapsed = 0f;

        while (elapsed < 20f)
        {
            if (!agent.pathPending)
            {
                if (agent.pathStatus != NavMeshPathStatus.PathComplete)
                {
                    Debug.LogWarning("Path is unavailable.");
                    yield break;
                }

                if (agent.remainingDistance <=
                    agent.stoppingDistance + 0.15f)
                {
                    StopMovement();
                    Debug.Log("NPC reached the target!");
                    yield break;
                }
            }

            elapsed += Time.deltaTime;
            yield return null;
        }

        Debug.LogWarning("Movement timeout.");
        StopMovement();
    }

    public void StopMovement()
    {
        if (agent.enabled && agent.isOnNavMesh)
        {
            agent.isStopped = true;
            agent.ResetPath();
        }
    }
}
