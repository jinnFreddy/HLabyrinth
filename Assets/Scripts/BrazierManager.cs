using System.Threading;
using UnityEngine;

public class BrazierManager : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Brazier[] braziers;
    // [SerializeField] private AudioClip brazierLitSound;
    [SerializeField] private float finalWinDelay;

    [Header("Runtime")]
    private int trappedShadows = 0;
    private bool allBraziersLit = false;

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
        if (allBraziersLit || trappedShadows >= braziers.Length)
        {
            return;
        }

        Debug.Log($"[BrazierManager] Shadow trapped – lighting brazier #{trappedShadows + 1}");

        braziers[trappedShadows].LightUp();
        trappedShadows++;

        //if (brazierLitSound != null)
        //{
        //    AudioSource.PlayClipAtPoint(brazierLitSound, transform.position);
        //}

        if (trappedShadows >= braziers.Length)
        {
            allBraziersLit = true;
            Debug.Log("[BrazierManager] All braziers lit!");
            Invoke(nameof(OnAllBraziersLit), finalWinDelay);
        }
    }

    private void OnAllBraziersLit()
    {
        // Trigger cutscene, open door, activate final clue, etc.
        Debug.Log("[BrazierManager] Puzzle complete!");

        // Optional: Play final sound or VFX
    }
}
