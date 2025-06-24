using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BreakApart : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject intactPillar;
    [SerializeField] private GameObject brokenPillarPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float minPieceDelay;
    [SerializeField] private float maxPieceDelay;
    [SerializeField] private float warningTime;
    [SerializeField] private float trembleIntensity;
    [SerializeField] private float trembleSpeed;
    [SerializeField] private BoxCollider _boxCollider; 

    private bool hasBroken = false;
    private bool isWarning = false;
    private bool isImpactProcessed = false;

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
        //if (warningSound != null)
        //{
        //    AudioSource.PlayClipAtPoint(warningSound, transform.position);
        //}

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

            intactPillar.transform.position = originalPosition + tremble;
            elapsed += Time.deltaTime;
            yield return null;
        }

        Break();
    }

    private void Break()
    {
        intactPillar.SetActive(false);
        _boxCollider.isTrigger = true;

        if (brokenPillarPrefab != null && spawnPoint != null)
        {
            GameObject brokenGO = Instantiate(brokenPillarPrefab, spawnPoint.position, spawnPoint.rotation);
            brokenGO.SetActive(true);
            Debris[] debrisPieces = brokenGO.GetComponentsInChildren<Debris>();

            foreach (var piece in debrisPieces)
            {
                float randomDelay = UnityEngine.Random.Range(minPieceDelay, maxPieceDelay);
                piece.ActivatePiece(randomDelay);
            }
        }

        isWarning = false;
        isImpactProcessed = false;
    }
}
