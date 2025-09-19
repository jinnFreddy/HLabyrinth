using System.Collections.Generic;
using System.Drawing;
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

    void Update()
    {
        if (Input.GetMouseButton(0))  // Se o botão do mouse estiver pressionado
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject decal = Instantiate(chalkDecalPrefab, hit.point, Quaternion.identity);
                decal.transform.forward = -hit.normal;  // Align the decal with the surface
            }
        }
    }
}