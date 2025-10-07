using System.Collections;
using UnityEngine;

public class FogPlaneConstraint : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject fogPlane;
    [SerializeField] private Transform respawnPoint;
    [SerializeField] private BreakApart[] structures;
    [SerializeField] private float respawnDelay;

    private bool _isRespawning = false;

    private void OnTriggerEnter(Collider other)
    {
        if (_isRespawning || !other.CompareTag("Player")) return;

        StartCoroutine(DelayedRespawn());
    }

    private IEnumerator DelayedRespawn()
    {
        GameManager.Instance.DeathScreenPlatforming();

        yield return new WaitForSeconds(respawnDelay);
        TriggerRespawn();
    }

    private void TriggerRespawn()
    {
        Rigidbody rb = player.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.position = respawnPoint.position;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        GameManager.Instance.isDead = false;
        SoundManager.Unmute();
        _isRespawning = false;
        foreach (var s in structures)
        {
            if (s.hasBroken)
            {
                s.ResetStructure();
            }
        }
    }
}
