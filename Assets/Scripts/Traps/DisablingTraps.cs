using System;
using UnityEngine;
using UnityEngine.UI;

public class DisablingTraps : MonoBehaviour
{
    [SerializeField] private float _maxDistance = 5f;
    [SerializeField] private Image _progressBar;
    [SerializeField] private float _disableTime = 5f;
    [SerializeField] private LayerMask _trapMask;
    [SerializeField] private Loadout _loadout;

    private WireTrap _currentTrap = null;
    private float _progress = 0f;

    // Update is called once per frame
    void Update()
    {
        DetectWireTrap();
        HandleDisabling();
    }

    private void DetectWireTrap()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out hit, _maxDistance, _trapMask))
        {
            WireTrap trap = hit.collider.GetComponentInChildren<WireTrap>();
            if (trap != null && !trap._isDisabled)
            {
                _currentTrap = trap;
                return;
            }
        }

        _currentTrap = null;
        _progress = 0f;
        if (_progressBar) _progressBar.fillAmount = 0f;
    }

    private void HandleDisabling()
    {
        if (_currentTrap == null || !HasWirecutterEquipped() || !Input.GetKey(KeyCode.E))
        {
            _progress = 0f;
            if (_progressBar) _progressBar.fillAmount = 0f;
            return;
        }

        if (IsPlayerMoving())
        {
            _progress = 0f;
            return;
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, _maxDistance, _trapMask) || hit.collider.GetComponentInChildren<WireTrap>() != _currentTrap)
        {
            _progress = 0f;
            return;
        }

        _progress += Time.deltaTime / _disableTime;
        if (_progress >= 1f)
        {
            _currentTrap.Disable();
            _progress = 0f;
            if (_progressBar) _progressBar.fillAmount = 0f;
        }
        else
        {
            if (_progressBar) _progressBar.fillAmount = _progress;
        }
    }

    bool HasWirecutterEquipped()
    {
        if (_loadout == null) return false;

        if (_loadout.currentSlotIndex >= 0 && _loadout.currentSlotIndex < _loadout.loadoutSlots.Length)
        {
            return _loadout.loadoutSlots[_loadout.currentSlotIndex].itemName == "Wirecutter";
        }

        return false;
    }

    bool IsPlayerMoving()
    {
        // Replace with your actual movement input or velocity check
        return Input.GetAxis("Horizontal") != 0 || Input.GetAxis("Vertical") != 0;
    }
}
