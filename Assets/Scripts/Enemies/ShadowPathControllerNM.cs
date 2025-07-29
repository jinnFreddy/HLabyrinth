using System;
using UnityEngine;
using UnityEngine.AI;

public class ShadowPathControllerNM : MonoBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    public Action OnTargetReachedEvent;

    // Update is called once per frame
    void Update()
    {
        if (agent != null &&
            !agent.pathPending &&
            agent.remainingDistance <= agent.stoppingDistance &&
            agent.velocity.sqrMagnitude < 0.5f)
        {
            OnTargetReachedEvent?.Invoke();
        }
    }


    private Vector3 PickRandomPoint(float radius)
    {
        Vector3 randomPos = transform.position + UnityEngine.Random.insideUnitSphere * radius;
        randomPos.y = transform.position.y;
        return randomPos;
    }

    private Vector3 GetFurthestPointInOppositeDirection(Transform origin, float distance)
    {
        Vector3 direction = -origin.forward;
        Vector3 targetPos = origin.position + direction * distance;
        targetPos.y = transform.position.y;
        return targetPos;
    }

    public void SetDestination(Vector3 target)
    {
        if (agent != null && agent.enabled)
        {
            agent.SetDestination(target);
        }
    }

    public void SetRandomDestination(float radius)
    {
        Vector3 randomDestination = PickRandomPoint(radius);
        SetDestination(randomDestination);
    }

    public void SetFurthestDestination(float radius)
    {
        Vector3 furthestDestination = GetFurthestPointInOppositeDirection(transform, radius);
        SetDestination(furthestDestination);
    }

    public void SetMoveSpeed(float speed)
    {
        if (agent != null)
        {
            agent.speed = speed;
        }
    }

    public void EnableRotation(bool value)
    {
        if (agent != null)
        {
            agent.updateRotation = value;
        }
    }
}
