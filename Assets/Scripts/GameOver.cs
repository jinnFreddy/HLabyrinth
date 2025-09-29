using UnityEngine;

public class GameOver : MonoBehaviour, IInteractable
{
    [SerializeField] private Loadout _loadout;
    [SerializeField] private GameObject _animatorGO;
    [SerializeField] private Animator animator;

    private void Awake()
    {
        _animatorGO.SetActive(false);
    }

    public void Interact(Interactable interactor)
    {
        if (_loadout.rewardItems == 3)
        {
            _animatorGO.SetActive(true);
            animator.Play("FadeOut");
        }
    }
}
