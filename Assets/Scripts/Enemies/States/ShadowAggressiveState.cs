using UnityEngine;
using UnityEngine.AI;
using static UnityEditor.Experimental.GraphView.GraphView;

public class ShadowAggressiveState : ShadowFSMState
{
    private readonly float moveSpeed = 6f;
    private readonly int radius = 20;
    private Transform currentTarget;
    private NavMeshAgent agent;
    private bool isHandlingEvent = false;

    public ShadowAggressiveState(Shadow shadow) : base(shadow)
    {
        _id = ShadowFSMStateType.AGGRESSIVE;
    }

    public override void Enter()
    {
        base.Enter();
        agent = _shadow.GetComponent<NavMeshAgent>();

        agent.isStopped = false;
        agent.speed = moveSpeed;
        agent.updateRotation = true;
        agent.stoppingDistance = 0.2f; 

        GameObject player = _shadow.playerDetector.GetPlayerWithinRadius(radius);
        if (player != null)
        {
            currentTarget = player.transform;
        }
        else
        {
            _shadow.shadowFSM.SetCurrentState(ShadowFSMStateType.SHY);
            return;
        }

        NavMeshAreaManager.Instance.EnterAggressiveState(agent, currentTarget.position);
        _shadow.pathController.OnTargetReachedEvent += SafeOnTargetReached;
    }

    public override void Update()
    {
        base.Update();
        GameObject player = _shadow.playerDetector.GetPlayerWithinRadius(radius);
        if (player == null)
        {
            _shadow.shadowFSM.SetCurrentState(ShadowFSMStateType.SHY);
            return;
        }

        currentTarget = player.transform;
        if (!isHandlingEvent && !agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + 1f)
        {
            NavMeshAreaManager.Instance.ForcePathRecalculation(agent, currentTarget.position);
        }
    }

    public override void Exit()
    {
        base.Exit();

        agent = _shadow.GetComponent<NavMeshAgent>();
        Vector3 currentDestination = agent.hasPath ? agent.destination : Vector3.zero;

        NavMeshAreaManager.Instance.ExitAggressiveState(agent, currentDestination);
        _shadow.pathController.OnTargetReachedEvent -= SafeOnTargetReached;
        currentTarget = null;
    }

    private void SafeOnTargetReached()
    {
        isHandlingEvent = true;

        try
        {
            Debug.Log("Aggressive state: Target reached (non-pathing)");

            // Play sound
            // AudioSource.PlayClipAtPoint(attackSound, transform.position);
        }
        finally
        {
            isHandlingEvent = false;
        }

    }
}