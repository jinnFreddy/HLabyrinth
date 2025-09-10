using UnityEngine;
using UnityEngine.UI;

public class CrosshairManager : MonoBehaviour
{
    public static CrosshairManager Instance { get; private set; }

    [Header("Crosshair Settings")]
    [SerializeField] private Image normalCrosshair;
    [SerializeField] private Image interactingCrosshair;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        SetNormalCrosshair();
    }

    public void SetInteractingCrosshair()
    {
        UpdateCrosshairState(true);
    }

    public void SetNormalCrosshair()
    {
        UpdateCrosshairState(false);
    }

    private void UpdateCrosshairState(bool isInteracting)
    {
        if (normalCrosshair != null)
        {
            normalCrosshair.enabled = !isInteracting;
        }

        if (interactingCrosshair != null)
        {
            interactingCrosshair.enabled = isInteracting;
        }
    }
}
