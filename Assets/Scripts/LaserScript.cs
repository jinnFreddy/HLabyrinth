using System;
using UnityEngine;

[RequireComponent (typeof(LineRenderer))]
public class LaserScript : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask _reflectiveSurfaces;
    [SerializeField] private LayerMask _lightSource;
    [SerializeField] private LayerMask _crystalCluster;
    [SerializeField] private float _defaultLength;
    [SerializeField] private int _numOfReflections;
    [SerializeField] private WinConditionGlow _crystalVisuals;

    private bool _isConnectedToCrystal = false;
    private float _rayOffset = 0.01f; 
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

        int combinedMask = _reflectiveSurfaces.value | _crystalCluster.value;
        bool hitCrystal = false;

        for (int i = 0; i < _numOfReflections; i++)
        {
            if (Physics.Raycast(_ray.origin, _ray.direction, out _hit, remainingLength, combinedMask, QueryTriggerInteraction.Collide))
            {
                positions[validPoints++] = _hit.point;

                bool isLayerMatch = (_crystalCluster.value & (1 << _hit.collider.gameObject.layer)) != 0;
                bool isTagMatch = _hit.collider.CompareTag("CrystalCluster");
                int crystalLayerIndex = LayerMask.NameToLayer("CrystalCluster");
                bool isDirectLayerMatch = _hit.collider.gameObject.layer == crystalLayerIndex;
                bool isCrystalCluster = isLayerMatch || isTagMatch || isDirectLayerMatch;

                if (isCrystalCluster)
                {
                    if (_crystalVisuals == null)
                    {
                        _crystalVisuals = _hit.collider.GetComponent<WinConditionGlow>();
                    }

                    if (!_isConnectedToCrystal && _crystalVisuals != null)
                    {
                        _crystalVisuals.OnLaserEnter(this);
                        _isConnectedToCrystal = true;
                    }
                    hitCrystal = true;
                    remainingLength = 0;
                    break;
                }

                Vector3 reflectDir = Vector3.Reflect(_ray.direction, _hit.normal);
                Vector3 newOrigin = _hit.point + reflectDir.normalized * _rayOffset;

                remainingLength -= Vector3.Distance(_ray.origin, _hit.point);

                _ray = new Ray(newOrigin, reflectDir);
            }
            else
            {
                positions[validPoints++] = _ray.origin + _ray.direction * remainingLength;
                break;
            }
        }

        if (_isConnectedToCrystal && !hitCrystal && _crystalVisuals != null)
        {
            _crystalVisuals.OnLaserExit(this);
            _isConnectedToCrystal = false;
            _crystalVisuals = null;
        }

        if (validPoints > 1)
        {

            _lineRenderer.positionCount = validPoints;
            _lineRenderer.SetPositions(positions);
            _lineRenderer.enabled = true;
        }
    }
}
