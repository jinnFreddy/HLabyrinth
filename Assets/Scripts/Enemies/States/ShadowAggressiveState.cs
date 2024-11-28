using UnityEngine;

public class ShadowAggressiveState: ShadowFSMState
{
    private readonly float moveSpeed = 6f;
    private readonly int radius = 20;
    private Transform currentTarget;

    public ShadowAggressiveState(Shadow shadow) : base(shadow)
    {
        _id = ShadowFSMStateType.AGGRESSIVE;
    }

    public override void Enter()
    {
        base.Enter();
        _shadow.pathController.EnableRotation(true);
        _shadow.pathController.SetMoveSpeed(moveSpeed);
        _shadow.pathController.OnTargetReachedEvent += OnTargetReached;

        GameObject player = _shadow.playerDetector.GetPlayerWithinRadius(radius);
        if (player != null)
        {
            currentTarget = player.transform;
        } else {
            _shadow.shadowFSM.SetCurrentState(ShadowFSMStateType.SHY);
        }

    }

    public override void Update()
    {
        base.Update();

        if (_shadow.playerDetector.GetPlayerWithinRadius(radius) == null)
        {
            _shadow.shadowFSM.SetCurrentState(ShadowFSMStateType.SHY);
        }
    }

    public override void Exit()
    {
        base.Exit();
        _shadow.pathController.OnTargetReachedEvent -= OnTargetReached;
        currentTarget = null;
    }

    private void OnTargetReached()
    {
        if (currentTarget != null)
        {
            _shadow.pathController.SetDestination(currentTarget.transform.position);
        }
    }
    
}