using UnityEngine;

public static class BoundsExtensions
{
    public static void GetCorners(this Bounds bounds, Vector3[] corners)
    {
        Vector3 center = bounds.center;
        Vector3 extents = bounds.extents;

        corners[0] = center + new Vector3(extents.x, extents.y, extents.z);
        corners[1] = center + new Vector3(extents.x, extents.y, -extents.z);
        corners[2] = center + new Vector3(extents.x, -extents.y, extents.z);
        corners[3] = center + new Vector3(extents.x, -extents.y, -extents.z);
        corners[4] = center + new Vector3(-extents.x, extents.y, extents.z);
        corners[5] = center + new Vector3(-extents.x, extents.y, -extents.z);
        corners[6] = center + new Vector3(-extents.x, -extents.y, extents.z);
        corners[7] = center + new Vector3(-extents.x, -extents.y, -extents.z);
    }
}

