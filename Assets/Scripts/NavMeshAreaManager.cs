using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAreaManager : MonoBehaviour
{
    public static NavMeshAreaManager Instance { get; private set; }

    private int forbiddenAreaIndex = -1;
    private int allAreasMask;
    private int chaseMask; 
    private int avoidanceMask;
    private int cagedMask;
    private int aggressiveStateCount = 0;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            forbiddenAreaIndex = NavMesh.GetAreaFromName("Forbidden");
            if (forbiddenAreaIndex != -1)
            {
                allAreasMask = -1;
                chaseMask = allAreasMask;
                avoidanceMask = allAreasMask & ~(1 << forbiddenAreaIndex);
                cagedMask = 0;
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void EnterAggressiveState(NavMeshAgent agent, Vector3 destination)
    {
        if (forbiddenAreaIndex == -1 || agent == null) return;

        aggressiveStateCount++;

        agent.areaMask = chaseMask;
        ForcePathRecalculation(agent, destination);
    }

    public void ExitAggressiveState(NavMeshAgent agent, Vector3 destination)
    {
        if (forbiddenAreaIndex == -1 || agent == null) return;

        aggressiveStateCount = Mathf.Max(0, aggressiveStateCount - 1);
        ShadowPathControllerNM shadowController = agent.GetComponent<ShadowPathControllerNM>();

        if (aggressiveStateCount == 0)
        {
            if (shadowController != null && shadowController.caged)
            {
                agent.areaMask = cagedMask;
            }
            else
            {
                agent.areaMask = avoidanceMask;
            }
            ForcePathRecalculation(agent, destination);
        }
        else
        {
            agent.areaMask = chaseMask;
            ForcePathRecalculation(agent, destination);
        }
    }

    public void SetAvoidanceMask(NavMeshAgent agent)
    {
        agent.areaMask = avoidanceMask;
    }

    public void ForcePathRecalculation(NavMeshAgent agent, Vector3 destination)
    {
        if (agent == null || destination == Vector3.zero) return;

        agent.isStopped = true;
        agent.ResetPath();
        agent.SetDestination(destination);
        agent.isStopped = false;
    }
}
