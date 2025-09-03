using System;
using System.Collections.Generic;
using UnityEngine;

public class WinConditionGlow : MonoBehaviour
{
    [Header("Color States")]
    [SerializeField] private Color _1beam;
    [SerializeField] private Color _2beam;
    [SerializeField] private Color _3beam;

    [Header("Timing")]
    [SerializeField] private float pulseSpeed; 
    [SerializeField] private float transitionSpeed;

    private Renderer _renderer;
    private Material _material;

    private HashSet<LaserScript> activeLasers = new HashSet<LaserScript>();
    private Color _baseColor = Color.black;
    private Color _targetColor;

    private void Awake()
    {
        _renderer = GetComponent<Renderer>();
        _material = _renderer.material;
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        _material.SetColor("TopColor", _baseColor);
    }

    // Update is called once per frame
    void Update()
    {
        UpdateColorLerp();
    }

    private void UpdateColorLerp()
    {
        Color currentColor = _material.GetColor("_TopColor");
        Color lerpedColor = Color.Lerp(currentColor, _targetColor, transitionSpeed * Time.deltaTime);
        _material.SetColor("_TopColor", lerpedColor);
    }

    private void ForceUpdateVisualState()
    {
        int currentBeamCount = activeLasers.Count;
        _targetColor = GetTargetColorForState(currentBeamCount);
    }

    private Color GetTargetColorForState(int beamCount)
    {
        switch (beamCount)
        {
            case 0:
                return _baseColor;

            case 1:
                float pulse1 = Mathf.PingPong(Time.time * pulseSpeed, 1f);
                return Color.Lerp(_baseColor, _1beam, pulse1);

            case 2:
                float pulse2 = Mathf.PingPong(Time.time * pulseSpeed, 1f);
                return Color.Lerp(_1beam, _2beam, pulse2);

            case 3:
                return _3beam;

            default:
                return _baseColor;
        }
    }

    public void OnLaserEnter(LaserScript laser)
    {
        if (laser == null) return;

        if (activeLasers.Add(laser)) 
        {
            Debug.Log($"[Crystal] Laser ADDED: {laser.name}. Total: {activeLasers.Count}");
            ForceUpdateVisualState();
        }
    }

    public void OnLaserExit(LaserScript laser)
    {
        if (laser == null) return;

        if (activeLasers.Remove(laser))
        {
            Debug.Log($"[Crystal] Laser REMOVED: {laser.name}. Total: {activeLasers.Count}");
            ForceUpdateVisualState();
        }
    }
}
