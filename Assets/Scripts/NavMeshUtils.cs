using UnityEngine;
using UnityEngine.AI;

public static class NavMeshUtils
{
    public static float GetPathDistance(Vector3 from, Vector3 to)
    {
        if (!NavMesh.SamplePosition(from, out var fromHit, 5f, NavMesh.AllAreas))
            return Vector3.Distance(from, to);

        if (!NavMesh.SamplePosition(to, out var toHit, 5f, NavMesh.AllAreas))
            return Vector3.Distance(from, to);

        NavMeshPath path = new NavMeshPath();
        if (!NavMesh.CalculatePath(fromHit.position, toHit.position, NavMesh.AllAreas, path))
        {
            return Mathf.Infinity;
        }

        if (path.status == NavMeshPathStatus.PathInvalid)
        {
            return Mathf.Infinity;
        }

        Vector3[] corners = path.corners;
        if (corners.Length < 2)
            return Vector3.Distance(fromHit.position, toHit.position);

        float distance = 0f;
        for (int i = 0; i < corners.Length - 1; i++)
        {
            distance += Vector3.Distance(corners[i], corners[i + 1]);
        }

        return distance;
    }
}
