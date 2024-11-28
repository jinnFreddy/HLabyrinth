using UnityEngine;
using Pathfinding;
using System;

public class ShadowAIPath : AIPath
{
    public Action OnTargetReachedEvent;

    public override void OnTargetReached()
    {
        base.OnTargetReached();
        OnTargetReachedEvent?.Invoke();
    }
}
