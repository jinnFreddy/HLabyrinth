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
    private bool _wasAboveLastFrame = true;

    // Update is called once per frame
    void Update()
    {
        bool isAbove = player.transform.position.y >= fogPlane.transform.position.y;
        if (!_isRespawning && _wasAboveLastFrame && !isAbove)
        {
            _isRespawning = true;
            _wasAboveLastFrame = false;
            StartCoroutine(DelayedRespawn());
        }
        else if (isAbove)
        {
            _wasAboveLastFrame = true;
        }
        else
        {
            _wasAboveLastFrame = false;
        }
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
