using UnityEngine;

public class WireTrap : MonoBehaviour
{

    [SerializeField] private Transform _TrapMechanism;
    [SerializeField] public bool _isDisabled = false;
    [SerializeField] private GameObject _tp;

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
            PlayerMovement player = other.GetComponent<PlayerMovement>();
            GameManager.Instance.teleporter = _tp;
            player.HurtPlayer();
            this.gameObject.SetActive(false);
            Debug.Log("Wire trap triggered");
            
        }
    }
}
