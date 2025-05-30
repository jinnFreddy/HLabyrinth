using UnityEngine;

public class WireTrap : MonoBehaviour
{

    [SerializeField] private Transform _TrapMechanism;
    [SerializeField] public bool _isDisabled = false;

    public void Disable()
    {
        _isDisabled = true;
        this.gameObject.SetActive(false);
        Debug.Log("Wire trap disabled");
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log($"[TRIGGER] Entered by: {other.name}");
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            player.HurtPlayer();
            this.gameObject.SetActive(false);
            Debug.Log("Wire trap triggered");
            
        }
    }
}
