using UnityEngine;

public class TPBack : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _player;
    [SerializeField] private Transform _spawnPoint;

    public void Interact(Interactable interactor)
    {
        _player.transform.position = _spawnPoint.position;
    }
}
