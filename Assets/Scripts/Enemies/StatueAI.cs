using NUnit.Framework;
using Pathfinding;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;

#nullable enable
public class StatueAI : MonoBehaviour
{
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float speed;
    [SerializeField] private Animator animator;

    //private AIPath aiPath;
    //private AIDestinationSetter aiDestinationSetter;
    [SerializeField] private NavMeshAgent agent;
    private GameObject? currentTarget;

    private void Awake()
    {
        //aiPath = FindFirstObjectByType<AIPath>();
        //aiDestinationSetter = FindFirstObjectByType<AIDestinationSetter>();
        agent.speed = speed;
        agent.isStopped = true;
    }

    private void Update()
    {
        GameObject target = GetPlayerWithinRadius();

        if(!IsAnyoneLookingAtMe() && currentTarget == null && target != null)
        {
            SetCurrentTarget(target);
            animator.SetTrigger("Observing");
            animator.speed = 1f;
            agent.speed = speed;
        }
        else if (IsAnyoneLookingAtMe())
        {
            SetCurrentTarget(null);
            animator.speed = 0f;
            agent.speed = 0f;
        }
    }
    private GameObject GetPlayerWithinRadius()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= 20 * transform.localScale.x)
            {
                return player;
            }
        }
        return null;
    }

    private void SetCurrentTarget(GameObject? target)
    {
        currentTarget = target;

        if (target != null)
        {
            //aiDestinationSetter.target = target.transform;
            //aiPath.canMove = true;
            agent.isStopped = false;
            agent.SetDestination(target.transform.position);
        } else
        {
            //aiDestinationSetter.target = null;
            //aiPath.canMove = false;
            agent.isStopped = true;
            agent.ResetPath();
        }
    }
    private bool IsAnyoneLookingAtMe()
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Camera playerCamera = Camera.main;
            if (playerCamera != null && IsGameObjectInView(playerCamera))
            {
                return true;
            }
        }
        return false;
    }

    private bool IsGameObjectInView(Camera cam)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        Bounds bounds = boxCollider.bounds;
        if(!GeometryUtility.TestPlanesAABB(planes, bounds))
        {
            return false;
        }

        Vector3[] corners = new Vector3[8];
        bounds.GetCorners(corners);

        int visibleCorners = 0;
        int totalCorners = 0;

        foreach (Vector3 corner in corners)
        {
            Vector3 viewportPoint = cam.WorldToViewportPoint(corner);
            totalCorners++;
            if (IsInScreenBounds(viewportPoint))
            {
                Vector3 direction = corner - cam.transform.position;
                if (Physics.Raycast(cam.transform.position, direction, out RaycastHit hit))
                {
                    if (hit.collider.gameObject == gameObject)
                    {
                        visibleCorners++;
                    }
                }
            }
        }
        return visibleCorners >= 2;
    }

    private bool IsInScreenBounds(Vector3 viewportPoint)
    {
        return viewportPoint.x >= 0 && viewportPoint.x <= 1 &&
               viewportPoint.y >= 0 && viewportPoint.y <= 1 &&
               viewportPoint.z > 0;
    }
}
