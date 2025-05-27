using UnityEngine;

public class UVFlashlight : MonoBehaviour
{
    [Tooltip("List of materials that react to UV Flashlight")]
    [SerializeField] private Material[] revealedMaterials;
    [SerializeField] private Light _UVlight;

    void Update()
    {
        if (_UVlight == null) return;

        bool isActive = _UVlight.gameObject.activeSelf;

        foreach (var mat in revealedMaterials)
        {
            if (mat == null) continue;

            if (isActive)
            {
                Vector3 forward = transform.forward.normalized;
                mat.SetVector("_LightPosition", transform.position);
                mat.SetVector("_LightDirection", new Vector4(forward.x, forward.y, forward.z, 0));
                mat.SetFloat("_LightAngle", _UVlight.spotAngle);
                mat.SetFloat("_IsUVActive", 1.0f); 
            }
            else
            {
                mat.SetFloat("_IsUVActive", 0.0f);
            }
        }
    }
}
