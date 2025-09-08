using UnityEngine;

public class CollectItem : MonoBehaviour, IInteractable
{
    [SerializeField] private Loadout _loadout;
    [SerializeField] private Animator _doorAnimator;

    public void Interact(Interactable interactor)
    {
        _loadout.rewardItems++;
        this.gameObject.SetActive(false);
        _doorAnimator.SetTrigger("Open");
    }
}
