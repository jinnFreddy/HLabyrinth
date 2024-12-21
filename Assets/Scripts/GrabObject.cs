using UnityEngine;

public class GrabObject : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _mirrorHandle;
    [SerializeField] private Transform _playerOrientation;
    [SerializeField] private Transform _initialPosition;
    [SerializeField] private PlayerCam _playerCam;

    public void Interact(Interactable interactor)
    {
        if (!_playerCam.isMovingObjects)
        {
            _playerCam.isMovingObjects = true;
            _player.GetComponent<PlayerMovement>().isSlowed = true;
            _mirrorHandle.gameObject.transform.SetParent(_playerOrientation);
        }
        else
        {
            _playerCam.isMovingObjects = false;
            _player.GetComponent<PlayerMovement>().isSlowed = false;
            _mirrorHandle.gameObject.transform.SetParent(_initialPosition);
        }
    }
}
