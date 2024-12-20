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

    private LineRenderer _lineRenderer;
    private RaycastHit _hit;
    private Ray _ray;
    private Vector3 _direction;

    void Start()
    {
        _lineRenderer = GetComponent<LineRenderer>();
    }

    void Update()
    {
        ReflectLaser();
    }

    private void ReflectLaser()
    {
        _ray = new Ray(transform.position, transform.forward);

        _lineRenderer.positionCount = 1;
        _lineRenderer.SetPosition(0, transform.position);

        float remainingLength = _defaultLength;

        if (Physics.Raycast(_ray.origin, _ray.direction, out _hit, remainingLength, _layerMask) && _light.activeSelf)
        {
            _lineRenderer.enabled = true;

            for (int i = 0; i < _numOfReflections; i++)
            {
                if (Physics.Raycast(_ray.origin, _ray.direction, out _hit, remainingLength, _layerMask))
                {
                    _lineRenderer.positionCount += 1;
                    _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, _hit.point);

                    remainingLength -= Vector3.Distance(_ray.origin, _hit.point);

                    _ray = new Ray(_hit.point, Vector3.Reflect(_ray.direction, _hit.normal));
                }
                else
                {
                    _lineRenderer.positionCount += 1;
                    _lineRenderer.SetPosition(_lineRenderer.positionCount - 1, _ray.origin + (_ray.direction * remainingLength));
                    break;
                }
            }
        }
        else
        {
            _lineRenderer.enabled = false;
        }
    }
}
