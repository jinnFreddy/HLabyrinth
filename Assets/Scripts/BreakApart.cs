using NUnit.Framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BreakApart : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject intactPillar;
    [SerializeField] private GameObject brokenPillarPrefab;
    [SerializeField] private Transform spawnPoint;
    [SerializeField] private float collapseDelay;
    [SerializeField] private float minPieceDelay;
    [SerializeField] private float maxPieceDelay;

    private bool hasBroken = false;

    private void Awake()
    {
        intactPillar.SetActive(true);
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!hasBroken && other.gameObject.CompareTag("Player"))
        {
            Break();
        }
    }

    private void Break()
    {
        hasBroken = true;
        intactPillar.SetActive(false);

        GameObject brokenGO = Instantiate(brokenPillarPrefab, spawnPoint.position, spawnPoint.rotation);
        brokenGO.SetActive(true);

        Debris[] debrisPieces = brokenGO.GetComponentsInChildren<Debris>();

        foreach (var piece in debrisPieces)
        {
            float randomDelay = UnityEngine.Random.Range(minPieceDelay, maxPieceDelay);
            piece.ActivatePiece(collapseDelay + randomDelay);
        }
    }
}
