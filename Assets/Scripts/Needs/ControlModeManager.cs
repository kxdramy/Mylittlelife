
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.InputSystem;

[RequireComponent(typeof(UtilityAI))]
public class ControlModeManager : MonoBehaviour
{
    private PlayerController playerController;
    private CharacterController characterController;
    private NavMeshAgent agent;
    private UtilityAI utilityAI;

    private bool autoMode = false;
    public bool IsAutoMode => autoMode;
    private void Awake()
    {
        playerController = GetComponent<PlayerController>();
        characterController = GetComponent<CharacterController>();
        agent = GetComponent<NavMeshAgent>();
        utilityAI = GetComponent<UtilityAI>();

        agent.enabled = false;
        characterController.enabled = true;
        playerController.enabled = true;
    }

    private void Update()
    {
        if (Keyboard.current != null &&
            Keyboard.current.tabKey.wasPressedThisFrame)
        {
            SwitchMode();
        }
    }

    public void SwitchMode()
    {
        if (autoMode)
        {
            utilityAI.StopAutonomy();

            agent.enabled = false;

            characterController.enabled = true;
            playerController.enabled = true;

            autoMode = false;

            Debug.Log("MANUAL MODE");
        }
        else
        {
            if (!NavMesh.SamplePosition(
                transform.position,
                out NavMeshHit hit,
                1.5f,
                NavMesh.AllAreas))
            {
                Debug.LogWarning("Player is not on NavMesh!");
                return;
            }

            playerController.enabled = false;
            characterController.enabled = false;

            transform.position = hit.position;

            agent.enabled = true;

            if (!agent.isOnNavMesh)
            {
                agent.enabled = false;
                characterController.enabled = true;
                playerController.enabled = true;

                Debug.LogWarning("NavMeshAgent initialization failed!");
                return;
            }

            if (!utilityAI.StartAutonomy())
            {
                agent.enabled = false;
                characterController.enabled = true;
                playerController.enabled = true;
                return;
            }

            autoMode = true;

            Debug.Log("AUTO MODE: Utility AI enabled");
        }
    }
}
