using System.Collections;
using UnityEngine;

public class FogPlaneConstraint : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject fogPlane;
    [SerializeField] private float margin;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private BreakApart[] structures;
    [SerializeField] private float respawnDelay;

    private bool _isRespawning = false;

    // Update is called once per frame
    void Update()
    {
        if (!_isRespawning && player.transform.position.y <= fogPlane.transform.position.y - margin)
        {
            StartCoroutine(DelayedRespawn());
        }
    }

    private IEnumerator DelayedRespawn()
    {
        _isRespawning = true;

        //if (deathEffect != null)
        //{
        //    Instantiate(deathEffect, player.position, Quaternion.identity);
        //}

        //if (deathSound != null)
        //{
        //    AudioSource.PlayClipAtPoint(deathSound, player.position);
        //}

        yield return new WaitForSeconds(respawnDelay);
        TriggerRespawn();
    }

    private void TriggerRespawn()
    {
        foreach (var s in structures)
        {
            if (s.hasBroken)
            {
                s.ResetStructure();
            }
        }

        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.position = respawnPoint.position;
        }

        _isRespawning = false;
    }
}
