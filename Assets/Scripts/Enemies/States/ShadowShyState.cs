using UnityEngine;

public class ShadowShyState: ShadowFSMState
{
    private readonly int radius = 29;
    private readonly float moveSpeed = 7f;
    private Transform currentTarget;

    public ShadowShyState(Shadow shadow) : base(shadow)
    {
        _id = ShadowFSMStateType.SHY;
    }

    public override void Enter()
    {
        base.Enter();
        _shadow.pathController.EnableRotation(false);
        _shadow.pathController.SetMoveSpeed(moveSpeed);
        _shadow.pathController.SetFurthestDestination(radius);
        _shadow.pathController.OnTargetReachedEvent += OnTargetReached;
    }

    public override void Update()
    {
        base.Update();
        GameObject target = _shadow.playerDetector.GetPlayerWithinRadius(radius);
        if (target != null)
        {
            currentTarget = target.transform;
            _shadow.transform.LookAt(currentTarget);
        }

        if (_shadow.playerDetector.IsAnyoneLookingAtMe())
        {
            _shadow.shadowFSM.SetCurrentState(ShadowFSMStateType.AGGRESSIVE);
        }
    }

    public override void Exit()
    {
        base.Exit();
        _shadow.pathController.OnTargetReachedEvent -= OnTargetReached;
    }
    private void OnTargetReached()
    {
        _shadow.shadowFSM.SetCurrentState(ShadowFSMStateType.PATROLLING);
    }
}