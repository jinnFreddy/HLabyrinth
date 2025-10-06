using System.Collections.Generic;
using System.Drawing;
using UnityEngine;

public class ChalkDrawing : MonoBehaviour
{
    public GameObject chalkDecalPrefab; 
    void Update()
    {
        if (Input.GetMouseButton(0))  
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

            if (Physics.Raycast(ray, out hit))
            {
                GameObject decal = Instantiate(chalkDecalPrefab, hit.point, Quaternion.identity);
                decal.transform.forward = -hit.normal;
            }
        }
    }
}