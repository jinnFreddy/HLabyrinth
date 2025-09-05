using System;
using System.Collections.Generic;
using UnityEngine;

public class WinConditionGlow : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private Color _1beam;
    [SerializeField] private Color _2beam;
    [SerializeField] private Color _3beam;
    [SerializeField] private float _pulseSpeed; 
    [SerializeField] private float _transitionSpeed;
    [SerializeField] private Animator _doorAnimator;

    private Renderer _renderer;
    private Material _material;

    private HashSet<LaserScript> _activeLasers = new HashSet<LaserScript>();
    private Color _baseColor = Color.black;
    private Color _targetColor = Color.black;
    private int _currentBeamCount = 0;
    private bool _winCondition = false;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _material = _renderer.material;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _targetColor = _baseColor;
        _material.SetColor("TopColor", _baseColor);
    }

    // Update is called once per frame
    void Update()
    {
        ColorState();
        TransitionColor();
    }

    private void ColorState()
    {
        int beamCount = _activeLasers.Count;
        if (beamCount != _currentBeamCount)
        {
            if (beamCount >= 3 && !_winCondition)
            {
                TriggerWinCondition();
            }
            _currentBeamCount = beamCount;
        }
        _baseColor = GetTargetColorForState(beamCount);
    }

    private Color GetTargetColorForState(int beamCount)
    {
        float pulse = Mathf.PingPong(Time.time * _pulseSpeed, 1f);
        switch (beamCount)
        {
            case 0:
                return Color.black;

            case 1:
                return Color.Lerp(Color.black, _1beam, pulse);

            case 2:
                return Color.Lerp(_1beam, _2beam, pulse);

            case 3:
                return Color.Lerp(_2beam, _3beam, pulse);

            default:
                return _baseColor;
        }
    }

    private void TransitionColor()
    {
        _targetColor = Color.Lerp(_targetColor, _baseColor, _transitionSpeed * Time.deltaTime);
        _material.SetColor("_TopColor", _targetColor);
    }

    public void OnLaserEnter(LaserScript laser)
    {
        if (laser == null) return;

        if (_activeLasers.Add(laser)) 
        {
            Debug.Log($"[Crystal] Laser ADDED: {laser.name}. Total: {_activeLasers.Count}");
        }
    }

    public void OnLaserExit(LaserScript laser)
    {
        if (laser == null) return;

        if (_activeLasers.Remove(laser))
        {
            Debug.Log($"[Crystal] Laser REMOVED: {laser.name}. Total: {_activeLasers.Count}");
        }
    }

    private void TriggerWinCondition()
    {
        _winCondition = true;
        if (_doorAnimator != null)
        {
            _doorAnimator.SetTrigger("Open");
            Debug.Log("[Win] Trigger 'Open' SET");
            return;
        }
    }
}
