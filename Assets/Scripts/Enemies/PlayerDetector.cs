using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PlayerDetector : MonoBehaviour
{
    [SerializeField] private LayerMask playerLayer;
    [SerializeField] private BoxCollider boxCollider;

    public GameObject GetPlayerWithinRadius(int radius)
    {
        GameObject player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            float distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= radius * transform.localScale.x)
            {
                return player;
            }
        }
        return null;
    }

    public bool IsAnyoneLookingAtMe()
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

    public bool IsGameObjectInView(Camera cam)
    {
        Plane[] planes = GeometryUtility.CalculateFrustumPlanes(cam);
        Bounds bounds = boxCollider.bounds;
        if (!GeometryUtility.TestPlanesAABB(planes, bounds))
        {
            return false;
        }

        Vector3[] corners = new Vector3[8];
        bounds.GetCorners(corners);

        foreach (Vector3 corner in corners)
        {
            Vector3 direction = corner - cam.transform.position;
            if (Physics.Raycast(cam.transform.position, direction, out RaycastHit hit))
            {
                if (hit.collider.gameObject == gameObject)
                {
                    return true;
                }
            }
        }

        return false;
    }
}