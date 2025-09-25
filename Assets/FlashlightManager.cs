using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashlightManager : MonoBehaviour
{
    [SerializeField] private GameObject uvFlashlight;

    [SerializeField] private LayerMask uvLayer;
    [SerializeField] private Light uvLight;
    [SerializeField] private DecalProjector[] decalProjectors;
    [SerializeField] private float maxActiveDistance = 50f;

    public static FlashlightManager Instance;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    private void Start()
    {
        foreach (var decal in decalProjectors)
        {
            if (decal != null)
                decal.enabled = false;
        }
    }

    private void Update()
    {
        if (uvFlashlight.activeInHierarchy &&
            uvLight != null &&
            uvLight.isActiveAndEnabled)
        {
            UpdateDecals();
        }
        else
        {
            DisableAllDecals();
        }
    }

    public void UpdateDecals()
    {
        Vector3 lightPos = uvLight.transform.position;
        Vector3 lightDir = uvLight.transform.forward;
        float range = uvLight.range;
        bool hitDetected = false;
        Vector3 hitPoint = Vector3.zero;

        for (int i = 0; i < 5; i++)
        {
            Vector3 dir = RandomCone.ConeDirection(lightDir, range, uvLight.spotAngle * 0.5f);
            if (Physics.Raycast(lightPos, dir, out RaycastHit hit, range, uvLayer))
            {
                hitPoint = hit.point;
                hitDetected = true;
                break;
            }
        }

        foreach (var decal in decalProjectors)
        {
            if (decal == null) continue;

            float distance = Vector3.Distance(decal.transform.position, hitPoint);
            decal.enabled = hitDetected && (distance <= maxActiveDistance);
        }
    }

    private void DisableAllDecals()
    {
        foreach (var decal in decalProjectors)
        {
            if (decal != null)
                decal.enabled = false;
        }
    }
}
public static class RandomCone
{
    public static Vector3 ConeDirection(Vector3 forward, float range, float maxAngleDegrees)
    {
        float angle = UnityEngine.Random.Range(0f, maxAngleDegrees);
        float rad = Mathf.Deg2Rad * angle;
        float z = Mathf.Cos(rad);
        float r = Mathf.Sin(rad);
        float phi = UnityEngine.Random.Range(0f, Mathf.PI * 2f);

        Vector3 localDir = new Vector3(r * Mathf.Cos(phi), r * Mathf.Sin(phi), z);
        return Quaternion.LookRotation(forward) * localDir;
    }
}
