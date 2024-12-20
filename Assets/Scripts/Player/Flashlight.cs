using UnityEngine;

public class Flashlight : MonoBehaviour
{
    [SerializeField] GameObject _flashlight;
    private bool _isFlashlightActive = false;

    void Start()
    {
        _flashlight.SetActive(false);
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.F))
        {
            if (_isFlashlightActive == false)
            {
                _flashlight.SetActive(true);
                _isFlashlightActive = true;
            }
            else
            {
                _flashlight.SetActive(false);
                _isFlashlightActive = false;
            }
        }
    }
}
