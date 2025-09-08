using UnityEngine;

public class GameOver : MonoBehaviour, IInteractable
{
    [SerializeField] private Loadout _loadout;

    public void Interact(Interactable interactor)
    {
        if (_loadout.rewardItems == 3)
        {
            // game over screen
        }
    }
}
