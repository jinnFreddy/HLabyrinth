using UnityEngine;

public class ShadowPatrollingState : ShadowFSMState
{
    private readonly int patrolRadius = 20;
    private readonly float moveSpeed = 6f;
    private readonly int playerDetectionRadius = 10;

    public ShadowPatrollingState(Shadow shadow) : base(shadow)
    {
        _id = ShadowFSMStateType.PATROLLING;
    }

    public override void Enter()
    {
        base.Enter();
        _shadow.pathController.DisableAttackAnimation();
        _shadow.pathController.SetPatrolAnimation();
        _shadow.pathController.EnableRotation(true);
        _shadow.pathController.SetMoveSpeed(moveSpeed);
        _shadow.pathController.SetRandomDestination(patrolRadius);
        _shadow.pathController.OnTargetReachedEvent += OnTargetReached;
    }

    public override void Update()
    {
        base.Update();
        GameObject target = _shadow.playerDetector.GetPlayerWithinRadius(playerDetectionRadius);
        if (target != null)
        {
            _shadow.shadowFSM.SetCurrentState(ShadowFSMStateType.INTERESTED);
        }
    }

    public override void Exit() { 
        base.Exit();
        _shadow.pathController.DisablePatrolAnimation();
        _shadow.pathController.OnTargetReachedEvent -= OnTargetReached;

    }

    private void OnTargetReached()
    {
        _shadow.pathController.SetRandomDestination(patrolRadius);
    }
}