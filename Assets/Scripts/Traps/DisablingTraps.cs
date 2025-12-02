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
    [SerializeField] private GameObject aviso;
    [SerializeField] private Text aviso_text;

    private WireTrap _currentTrap = null;
    private float _progress = 0f;
    private bool hasStartedDisabling = false;

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

        Debug.DrawRay(ray.origin, ray.direction * _maxDistance, Color.green);

        if (Physics.Raycast(ray, out hit, _maxDistance, _trapMask))
        {
            CrosshairManager.Instance?.SetInteractingCrosshair();
            WireTrap trap = hit.collider.GetComponentInChildren<WireTrap>();
            aviso.SetActive(true);

            if (trap != null && !trap._isDisabled)
            {
                aviso_text.text = trap.name;
                _currentTrap = trap;
                return;
            }
        }

        aviso.SetActive(false);
        _currentTrap = null;
        _progress = 0f;
        if (_progressBar) _progressBar.fillAmount = 0f;
    }

    private void HandleDisabling()
    {
        if (_currentTrap == null || !HasWirecutterEquipped() || !Input.GetKey(KeyCode.E))
        {
            ResetDisabling();
            return;
        }

        if (IsPlayerMoving())
        {
            ResetDisabling();
            return;
        }

        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        if (!Physics.Raycast(ray, out hit, _maxDistance, _trapMask) || hit.collider.GetComponentInChildren<WireTrap>() != _currentTrap)
        {
            ResetDisabling();
            return;
        }

        if (!hasStartedDisabling)
        {
            SoundManager.PlayDisableStart();
            hasStartedDisabling = true;
        }

        aviso_text.text = "Disabling";
        _progress += Time.deltaTime / _disableTime;
        if (_progress >= 1f)
        {
            _currentTrap.Disable();
            CrosshairManager.Instance?.SetNormalCrosshair();
            ResetDisabling();
        }
        else
        {
            if (_progressBar) _progressBar.fillAmount = _progress;
        }
    }

    private void ResetDisabling()
    {
        SoundManager.StopDisableSound();
        _progress = 0f;
        if (_progressBar) _progressBar.fillAmount = 0f;
        hasStartedDisabling = false;
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
