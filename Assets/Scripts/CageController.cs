using System.Collections;
using UnityEngine;

public class CageController : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float checkInterval = 0.1f;
    [SerializeField] private float closeDelay = 1f;
    [SerializeField] private Transform[] wallTransform;
    [SerializeField] private float wallCloseDuration = 2f;
    [SerializeField] private Transform centerPoint;
    [SerializeField] private Vector3 detectionBox;
    [SerializeField] private float _risingUnits;
    [SerializeField] private Brazier brazier;

    [Header("Runtime References")]
    [SerializeField] private LayerMask shadowLayerMask;
    [SerializeField] private LayerMask playerLayerMask;

    private Vector3[] initialWallPositions;
    private bool hasShadowInside = false;
    private bool playerIsInside = false;
    private bool cageClosed = false;
    private bool isProcessingTrap = false;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        InvokeRepeating(nameof(CheckCageStatus), 0f, checkInterval);
    }

    private void Update()
    {
        CheckCageStatus();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (IsPlayer(other))
        {
            Debug.Log("Player entered cage");
            playerIsInside = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (IsPlayer(other))
        {
            Debug.Log("Player left cage");
            playerIsInside = false;

            CheckForShadows();

            if (hasShadowInside && !isProcessingTrap)
            {
                Debug.Log("[Cage] Closing – shadow trapped");
                StartCoroutine(TriggerTrapSequence());
                // BrazierManager.Instance.OnShadowTrapped();
                // this.enabled = false; 
            }
        }
    }

    private bool IsPlayer(Collider other)
    {
        return (playerLayerMask & (1 << other.gameObject.layer)) != 0 ||
               other.CompareTag("Player");
    }

    //private bool IsShadow(Collider other)
    //{
    //    return (shadowLayerMask & (1 << other.gameObject.layer)) != 0 ||
    //           other.CompareTag("Shadow");
    //}

    private void CheckCageStatus()
    {
        if (isProcessingTrap) return;

        CheckForShadows();

        if (!cageClosed)
        {
            if (!playerIsInside && hasShadowInside)
            {
                Debug.Log("Closing cage – player escaped, shadow trapped!");
                StartCoroutine(TriggerTrapSequence());
            }
        }
        else
        {
            if (!hasShadowInside)
            {
                brazier.TurnOff();
                StartCoroutine(OpenCage());
            }
        }

    }

    private void CheckForShadows()
    {
        Collider[] nearby = Physics.OverlapBox(centerPoint.position, detectionBox * 0.5f, Quaternion.identity, shadowLayerMask);

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

    private IEnumerator TriggerTrapSequence()
    {
        isProcessingTrap = true;
        yield return null;

        CheckForShadows();
        Collider[] nearby = Physics.OverlapBox(centerPoint.position, detectionBox * 0.5f, Quaternion.identity, shadowLayerMask);
        if (!hasShadowInside || playerIsInside)
        {
            Debug.LogWarning("[Cage] Trap canceled - shadow no longer inside or player re-entered");
            isProcessingTrap = false;
            yield break;
        }

        BrazierManager.Instance.OnShadowTrapped();
        yield return StartCoroutine(CloseCage());
        nearby[0].GetComponent<ShadowPathControllerNM>().caged = true;
        cageClosed = true;
        isProcessingTrap = false;
        Debug.Log("Shadow Trapped!");
    }

    private IEnumerator OpenCage()
    {
        isProcessingTrap = true;

        foreach (Transform wall in wallTransform)
        {
            if (wall != null)
            {
                Vector3 openPosition = GetInitialWallPosition(wall);
                Vector3 startPosition = wall.position;

                float elapsedTime = 0f;
                while (elapsedTime < wallCloseDuration)
                {
                    float progress = elapsedTime / wallCloseDuration;
                    wall.position = Vector3.Lerp(startPosition, openPosition, progress);

                    elapsedTime += Time.deltaTime;
                    yield return null; 
                }

                wall.position = openPosition;
            }
        }

        if (brazier != null)
        {
            brazier.TurnOff(); 
        }

        cageClosed = false;
        isProcessingTrap = false;
    }

    private Vector3 GetInitialWallPosition(Transform wallTransform)
    {
        for (int i = 0; i < this.wallTransform.Length; i++)
        {
            if (this.wallTransform[i] == wallTransform)
            {
                return initialWallPositions[i];
            }
        }
        return wallTransform.position;
    }

    private IEnumerator CloseCage()
    {
        //cageClosed = true;
        //brazierMix.SetActive(true);

        // animate walls closing over time
        foreach (Transform walls in wallTransform)
        {
            if (walls != null)
            {
                Vector3 startPos = walls.position;
                Vector3 targetPos = new Vector3(startPos.x, startPos.y + _risingUnits, startPos.z);

                float elapsed = 0f;
                while (elapsed < wallCloseDuration)
                {
                    SoundManager.PlaySound(SoundType.METALDOOR);
                    walls.position = Vector3.Lerp(startPos, targetPos, elapsed / wallCloseDuration);
                    elapsed += Time.deltaTime;
                    yield return null;
                }

                walls.position = targetPos;

            }
        }

        brazier.LightUp();
        // Final cage locked state
        Debug.Log("Shadow Trapped!");

        // Optional: play sound, lock door, show UI feedback
    }

    private void OnDrawGizmos()
    {
        Color color = hasShadowInside ? Color.red : Color.yellow;

        Gizmos.matrix = Matrix4x4.TRS(centerPoint.position, transform.rotation, Vector3.one);
        Gizmos.color = color;
        Gizmos.DrawWireCube(Vector3.zero, detectionBox);
    }
}
