using UnityEngine;

public class GrabObject : MonoBehaviour, IInteractable
{
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _mirrorHandle; //MIRROR GO
    [SerializeField] private Transform _playerOrientation; //TO BE CHILD OF PLAYER'S ORIENTATION GO
    [SerializeField] private Transform _initialPosition; // TO BE CHILD OF INITIAL ENVIRONMENT HIERARCHY
    [SerializeField] private PlayerCam _playerCam; // FOR FIXING THE CAMERA, SO IT STOPS

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
