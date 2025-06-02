using System;
using UnityEngine;

[RequireComponent (typeof(LineRenderer))]
public class LaserScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private float _defaultLength;
    [SerializeField] private int _numOfReflections;
    [SerializeField] private GameObject _light;

    [Header("Visuals")]
    [SerializeField] private float baseWidth;
    [SerializeField] private float maxLineWidth;
    [SerializeField] private float pulseSpeed;

    private float _time = 0f;
    private LineRenderer _lineRenderer;
    private RaycastHit _hit;
    private Ray _ray;

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
        _lineRenderer.useWorldSpace = true;
    }

    void Update()
    {
        ReflectLaser();
    }

    private void ReflectLaser()
    {
        _lineRenderer.enabled = false;

        _ray = new Ray(transform.position, transform.forward);
        float remainingLength = _defaultLength;

        Vector3[] positions = new Vector3[_numOfReflections + 1];
        int validPoints = 0;
        positions[validPoints++] = _ray.origin;

        bool hitAnyReflectiveSurface = false;

        if (_light.activeSelf)
        {
            for (int i = 0; i < _numOfReflections; i++)
            {
                if (Physics.Raycast(_ray.origin, _ray.direction, out _hit, remainingLength, _layerMask))
                {
                    positions[validPoints++] = _hit.point;
                    hitAnyReflectiveSurface = true;

                    remainingLength -= Vector3.Distance(_ray.origin, _hit.point);
                    _ray = new Ray(_hit.point, Vector3.Reflect(_ray.direction, _hit.normal));
                }
                else
                {
                    positions[validPoints++] = _ray.origin + _ray.direction * remainingLength;
                    break;
                }
            }

            if (hitAnyReflectiveSurface && validPoints > 1)
            {

                _lineRenderer.positionCount = validPoints;

                for (int i = 0; i < validPoints; i++)
                {
                    _lineRenderer.SetPosition(i, positions[i]);
                }

                _lineRenderer.enabled = true;
                AnimateBeamWidth(validPoints - 1);
            }
            else
            {
                _lineRenderer.enabled = false;
            }
        }
    }

    private void AnimateBeamWidth(float numReflections)
    {
        float widthGrowth = Mathf.Lerp(baseWidth, maxLineWidth, numReflections / (float)_numOfReflections);
        float effectivePulseSpeed = pulseSpeed * (1 + numReflections * 0.5f);
        _time += Time.deltaTime * effectivePulseSpeed;
        float pulse = Mathf.Abs(Mathf.Sin(_time * Mathf.PI));
        float animatedWidth = baseWidth + (widthGrowth - baseWidth) * pulse;

        AnimationCurve smoothCurve = new AnimationCurve(
        new Keyframe(0, animatedWidth),
        new Keyframe(1, animatedWidth)
    );

        _lineRenderer.widthCurve = smoothCurve;
        _lineRenderer.widthMultiplier = 1f;
    }
}
