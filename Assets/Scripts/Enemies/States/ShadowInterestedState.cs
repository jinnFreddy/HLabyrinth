using UnityEngine;

public class ShadowInterestedState: ShadowFSMState
{
    private readonly int patrolRadius = 2;
    private readonly int playerDetectionRadius = 30;
    private readonly int moveSpeed = 2;
    private float lookTime = 0f; // Timer to track how long players are looking
    private bool isBeingLookedAt = false; // Flag to determine if any player is looking
    private const float timeThreshold = 2f; // Threshold to determine the next Bracken state
    private Transform currentTarget;

    public ShadowInterestedState(Shadow shadow): base(shadow)
    {
        _id = ShadowFSMStateType.INTERESTED;
    }

    public override void Enter()
    {
        base.Enter();
        _shadow.pathController.EnableRotation(false);
        _shadow.pathController.SetMoveSpeed(moveSpeed);
        _shadow.pathController.SetRandomDestination(patrolRadius);
        _shadow.pathController.OnTargetReachedEvent += OnTargetReached;

        GameObject target = _shadow.playerDetector.GetPlayerWithinRadius(playerDetectionRadius);
        if (target != null)
        {
            currentTarget = target.transform;
        }
    }

    public override void Update()
    {
        base.Update();
        if(currentTarget != null)
        {
            _shadow.transform.LookAt(currentTarget);
        }

        GameObject target = _shadow.playerDetector.GetPlayerWithinRadius(playerDetectionRadius);
        if (_shadow.playerDetector.IsAnyoneLookingAtMe())
        {
            if (!isBeingLookedAt)
            {
                isBeingLookedAt=true;
                lookTime = 0f;
            }
            lookTime += Time.deltaTime;

            if (IsStaringTooLong(lookTime))
            {
                _shadow.shadowFSM.SetCurrentState(ShadowFSMStateType.AGGRESSIVE);
            }
        } else if (isBeingLookedAt)
        {
            isBeingLookedAt = false;

            if(IsStaringTooLong(lookTime) )
            {
                _shadow.shadowFSM.SetCurrentState(ShadowFSMStateType.AGGRESSIVE);
            }
            else {
                _shadow.shadowFSM.SetCurrentState(ShadowFSMStateType.SHY);
            }
        }

        if (target == null)
        {
            _shadow.shadowFSM.SetCurrentState(ShadowFSMStateType.PATROLLING);
        }
    }

    public override void Exit()
    {
        base.Exit();
        _shadow.pathController.OnTargetReachedEvent -= OnTargetReached;
        currentTarget = null;
        lookTime = 0f;
        isBeingLookedAt = false;
    }

    private bool IsStaringTooLong(float time)
    {
        if (time > 0f && time <= timeThreshold)
        {
            return false;
        } 
        else if (time > timeThreshold)
        {
            return true;
        }
        return false;
    }

    private void OnTargetReached()
    {
        _shadow.pathController.SetRandomDestination(patrolRadius);
    }
}