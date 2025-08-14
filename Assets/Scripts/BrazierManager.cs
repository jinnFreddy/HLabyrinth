using System.Collections;
using System.Threading;
using UnityEngine;

public class BrazierManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float _NumberOfBraziers;
    // [SerializeField] private AudioClip brazierLitSound;
    [SerializeField] private float finalWinDelay;
    [SerializeField] private Transform wallTransform;
    [SerializeField] private float _risingUnits;
    [SerializeField] private float wallCloseDuration;

    [Header("Runtime")]
    private int trappedShadows = 0;
    private bool allBraziersLit = false;
    private bool isCageOpening = false;

    public static BrazierManager Instance { get; private set; }

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void OnShadowTrapped()
    {
        if (allBraziersLit || trappedShadows >= _NumberOfBraziers)
        {
            return;
        }

        Debug.Log($"[BrazierManager] Shadow trapped – {trappedShadows + 1}");

        //braziers[trappedShadows].LightUp();
        trappedShadows++;

        //if (brazierLitSound != null)
        //{
        //    AudioSource.PlayClipAtPoint(brazierLitSound, transform.position);
        //}

        if (trappedShadows >= _NumberOfBraziers)
        {
            allBraziersLit = true;
            Debug.Log("[BrazierManager] All braziers lit!");
            Invoke(nameof(OnAllBraziersLit), finalWinDelay);
        }
    }

    private void OnAllBraziersLit()
    {
        // Trigger cutscene, open door, activate final clue, etc.
        if (!isCageOpening && wallTransform != null)
        {
            isCageOpening = true;
            StartCoroutine(OpenCage());
            Debug.Log("[BrazierManager] Puzzle complete!");
        }
        // Optional: Play final sound or VFX
    }

    private IEnumerator OpenCage()
    {
        if (wallTransform != null)
        {
            Vector3 startPos = wallTransform.position;
            Vector3 targetPos = new Vector3(startPos.x, startPos.y - _risingUnits, startPos.z);

            float elapsed = 0f;
            while (elapsed < wallCloseDuration)
            {
                wallTransform.position = Vector3.Lerp(startPos, targetPos, elapsed / wallCloseDuration);
                elapsed += Time.deltaTime;
                yield return null;
            }
            wallTransform.position = targetPos;
        }

        // Final cage locked state
        Debug.Log("Final Cage Opened");

        // Optional: play sound, lock door, show UI feedback
    }
}
