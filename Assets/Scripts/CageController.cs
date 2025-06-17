using System.Collections;
using UnityEngine;

public class CageController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float checkInterval = 0.1f;
    [SerializeField] private float closeDelay = 1f;
    [SerializeField] private Transform wallTransform; // Optional: wall to animate closed
    [SerializeField] private float wallCloseDuration = 2f;
    [SerializeField] private Transform centerPoint;
    [SerializeField] private float detectionRadius = 2f;

    [Header("Runtime References")]
    [SerializeField] private LayerMask shadowLayerMask;
    [SerializeField] private LayerMask playerLayerMask;

    private bool hasShadowInside = false;
    private bool playerIsInside = false;
    private bool cageClosed = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(CheckCageStatus), 0f, checkInterval);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other))
        {
            Debug.Log("Player entered cage");
            playerIsInside = true;
        }

        if (IsShadow(other))
        {
            Debug.Log("Shadow entered cage");
            hasShadowInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other))
        {
            Debug.Log("Player left cage");
            playerIsInside = false;
        }

        if (IsShadow(other))
        {
            Debug.LogWarning("Shadow left cage – not trapping");
            hasShadowInside = false;
        }
    }

    private bool IsPlayer(Collider other)
    {
        return (playerLayerMask & (1 << other.gameObject.layer)) != 0 ||
               other.CompareTag("Player");
    }

    private bool IsShadow(Collider other)
    {
        return (shadowLayerMask & (1 << other.gameObject.layer)) != 0 ||
               other.CompareTag("Shadow");
    }

    private void CheckCageStatus()
    {
        if (cageClosed) return;

        // Only trigger closing logic if:
        // - Player left the cage
        // - Shadow is still inside
        CheckForShadows();

        if (!playerIsInside && hasShadowInside)
        {
            Debug.Log("Closing cage – player escaped, shadow trapped!");
            StartCoroutine(CloseCage());
        }
    }

    private void CheckForShadows()
    {
        Collider[] nearby = Physics.OverlapSphere(centerPoint.position, detectionRadius, shadowLayerMask);

        if (nearby.Length > 0)
        {
            hasShadowInside = true;
            foreach (var col in nearby)
            {
                Debug.Log($"Found shadow in sphere: {col.name}");
            }
        }
        else
        {
            hasShadowInside = false;
        }
    }

    private IEnumerator CloseCage()
    {
        cageClosed = true;

        // Optional: animate walls closing over time
        if (wallTransform != null)
        {
            Quaternion startRotation = wallTransform.rotation;
            Quaternion targetRotation = wallTransform.rotation * Quaternion.Euler(90, 0, 0); // Example rotation

            float elapsed = 0f;
            while (elapsed < wallCloseDuration)
            {
                wallTransform.localRotation = Quaternion.Slerp(startRotation, targetRotation, elapsed / wallCloseDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }

            wallTransform.localRotation = targetRotation;
        }

        // Final cage locked state
        Debug.Log("Shadow Trapped!");

        // Optional: play sound, lock door, show UI feedback
    }
}
