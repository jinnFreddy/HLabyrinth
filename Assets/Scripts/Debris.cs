using System.Collections;
using UnityEngine;

public class Debris : MonoBehaviour
{
    private Rigidbody rb;
    private bool hasFallen = false;

    [SerializeField] private float initialForceMagnitude = 5f;
    [SerializeField] private float initialTorqueMagnitude = 2f;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    public void ActivatePiece(float delay)
    {
        if (rb == null || hasFallen) return;

        Vector3 randomForce = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(0f, .2f),
            Random.Range(-1f, 1f)
        );

        StartCoroutine(ActivateWithDelay(delay));
    }

    private IEnumerator ActivateWithDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        rb.isKinematic = false;
        rb.useGravity = true;

        // Apply initial push + spin
        Vector3 randomDirection = Random.insideUnitSphere.normalized;
        rb.AddForce(randomDirection * initialForceMagnitude, ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * initialTorqueMagnitude, ForceMode.Impulse);

        hasFallen = true;
    }
}
