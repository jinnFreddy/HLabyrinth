using NUnit.Framework;
using Pathfinding;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

#nullable enable
public class StatueAI : MonoBehaviour
{
    [SerializeField] private BoxCollider boxCollider;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float speed;
    private AIPath aiPath;
    private AIDestinationSetter aiDestinationSetter;
    private GameObject? currentTarget;

    private void Awake()
    {
        aiPath = FindFirstObjectByType<AIPath>();
        aiDestinationSetter = FindFirstObjectByType<AIDestinationSetter>();
    }

    private void Update()
    {
        GameObject target = GetPlayerWithinRadius();

        if(!IsAnyoneLookingAtMe() && currentTarget == null && target != null)
        {
            SetCurrentTarget(target);
        }
        else if (IsAnyoneLookingAtMe())
        {
            SetCurrentTarget(null);
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
            aiDestinationSetter.target = target.transform;
            aiPath.canMove = true;
        } else
        {
            aiDestinationSetter.target = null;
            aiPath.canMove = false;
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

        foreach(Vector3 corner in corners)
        {
            Vector3 direction = corner - cam.transform.position;
            if(Physics.Raycast(cam.transform.position, direction, out RaycastHit hit)) {
                if (hit.collider.gameObject == gameObject)
                {
                    return true;
                }    
            }
        }

        return false;
    }

    
}
