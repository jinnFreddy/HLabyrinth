using UnityEngine;

[ExecuteAlways]
public class UVReveal : MonoBehaviour
{
    [SerializeField] Material _mat;
    [SerializeField] Light _light;
    [SerializeField] GameObject _FlaslightUV;

    private void OnDrawGizmos()
    {
        if (_light != null && _light.type == LightType.Spot)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawRay(_light.transform.position, _light.transform.forward * _light.range);

            float rad = _light.range * Mathf.Tan(Mathf.Deg2Rad * _light.spotAngle * 0.5f);
            Gizmos.DrawWireSphere(_light.transform.position + _light.transform.forward * _light.range, rad);
        }
    }

    private void Update()
    {
        if (_FlaslightUV.gameObject.activeSelf)
        {
            if (_mat && _light)
            {
                Vector3 lightForward = _light.transform.forward.normalized;

                _mat.SetVector("_LightPosition", _light.transform.position);
                _mat.SetVector("_LightDirection", new Vector4(lightForward.x, lightForward.y, lightForward.z, 0));
                _mat.SetFloat("_LightAngle", _light.spotAngle);
            }
        }
    }
}
