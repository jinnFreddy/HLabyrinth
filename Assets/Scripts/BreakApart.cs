using System.Collections;
using UnityEngine;

public class BreakApart : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] public GameObject intactPillar;
    [SerializeField] private Transform visualPillar;
    [SerializeField] public GameObject brokenPillarPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float minPieceDelay;
    [SerializeField] private float maxPieceDelay;
    [SerializeField] private float warningTime;
    [SerializeField] private float trembleIntensity;
    [SerializeField] private float trembleSpeed;
    [SerializeField] private Collider _collider; 

    public bool hasBroken = false;
    private bool isWarning = false;
    private bool isImpactProcessed = false;
    private GameObject spawnedBrokenGO;

    private void Awake()
    {
        intactPillar.SetActive(true);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (hasBroken || isWarning || isImpactProcessed) return;

        if (!hasBroken && other.gameObject.CompareTag("Player"))
        {
            isImpactProcessed = true;
            StartCoroutine(TriggerCollapseWithWarning());
        }
    }

    private IEnumerator TriggerCollapseWithWarning()
    {
        SoundManager.PlaySound(SoundType.PTREMBLE, 0.5f);

        isWarning = true;
        hasBroken = true;
        float elapsed = 0f;
        Vector3 originalPosition = intactPillar.transform.position;

        while (elapsed < warningTime)
        {
            Vector3 tremble = new Vector3(
                Mathf.PerlinNoise(Time.time * trembleSpeed, 0),
                Mathf.PerlinNoise(Time.time * trembleSpeed, 1),
                Mathf.PerlinNoise(Time.time * trembleSpeed, 2)
            ) * trembleIntensity;

            visualPillar.localPosition = tremble;
            elapsed += Time.deltaTime;
            yield return null;
        }

        Break();
    }

    private void Break()
    {
        intactPillar.SetActive(false);
        _collider.isTrigger = true;


        if (brokenPillarPrefab != null && spawnPoint != null)
        {
            spawnedBrokenGO = Instantiate(brokenPillarPrefab, spawnPoint.position, spawnPoint.rotation);
            Debris[] debrisPieces = spawnedBrokenGO.GetComponentsInChildren<Debris>();

            foreach (var piece in debrisPieces)
            {
                float randomDelay = Random.Range(minPieceDelay, maxPieceDelay);
                SoundManager.PlaySound(SoundType.PFALL, .5f);
                piece.ActivatePiece(randomDelay);
            }
        }

        isWarning = false;
        isImpactProcessed = false;
    }

    public void ResetStructure()
    {
        if (spawnedBrokenGO != null)
        {
            Destroy(spawnedBrokenGO);
            spawnedBrokenGO = null;
        }

        intactPillar.SetActive(true);
        hasBroken = false;
        isWarning = false;
        isImpactProcessed = false;
        _collider.isTrigger = false;
    }
}
