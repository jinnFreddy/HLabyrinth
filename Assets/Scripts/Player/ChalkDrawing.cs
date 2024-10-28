using System.Collections.Generic;
using UnityEngine;

public class ChalkDrawing : MonoBehaviour
{
    //public float chalkSize = 0.1f;  // Tamanho do círculo de giz

    //void Update()
    //{
    //    if (Input.GetMouseButton(0))
    //    {
    //        RaycastHit hit;
    //        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

    //        if (Physics.Raycast(ray, out hit))
    //        {
    //            Debug.DrawLine(transform.position, hit.point, Color.green);
    //            Renderer renderer = hit.collider.GetComponent<Renderer>();
    //            if (renderer != null)
    //            {
    //                renderer.material.SetVector("_ChalkPosition", hit.point);
    //                renderer.material.SetFloat("_ChalkSize", chalkSize);
    //            }
    //        }
    //    }
    //}

    public GameObject chalkDecalPrefab;  // Prefab do Decal de giz
    public float minDistanceBetweenDecals;  // Distância mínima entre decals

    private Vector3 lastPosition;  // Última posição desenhada
    private bool hasLastPosition = false;  // Se já existe uma última posição registrada

    void Update()
    {
        // Reset last position when the mouse button is released
        if (Input.GetMouseButtonUp(0))
        {
            hasLastPosition = false;
        }

        if (Input.GetMouseButton(0))  // Se o botão do mouse estiver pressionado
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                // Só cria um novo decal se a distância da última posição for maior que a mínima
                if (!hasLastPosition || Vector3.Distance(lastPosition, hit.point) > minDistanceBetweenDecals)
                {
                    // Interpolate points between lastPosition and the current hit point for smoother trail
                    List<Vector3> points = hasLastPosition ? GetInterpolatedPoints(lastPosition, hit.point, minDistanceBetweenDecals) : new List<Vector3> { hit.point };

                    foreach (Vector3 point in points)
                    {
                        // Instantiate a decal at each interpolated position
                        GameObject decal = Instantiate(chalkDecalPrefab, point, Quaternion.identity);
                        decal.transform.forward = hit.normal;  // Align the decal with the surface
                    }

                    // Salva a última posição desenhada
                    lastPosition = hit.point;
                    hasLastPosition = true;
                }
            }
        }
        else
        {
            hasLastPosition = false;  // Reseta quando o botão do mouse é solto
        }
    }

    List<Vector3> GetInterpolatedPoints(Vector3 start, Vector3 end, float stepSize)
    {
        List<Vector3> points = new List<Vector3>();
        float distance = Vector3.Distance(start, end);

        int steps = Mathf.FloorToInt(distance / stepSize);
        for (int i = 1; i <= steps; i++)
        {
            Vector3 point = Vector3.Lerp(start, end, i / (float)steps);
            points.Add(point);
        }

        return points;
    }
}