using UnityEngine;

public class Debris : MonoBehaviour
{
    private Rigidbody rb;
    private Vector3 originalPosition;
    private float delay;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        originalPosition = transform.localPosition;
        rb.isKinematic = true;
        rb.useGravity = false;
    }

    public void ActivatePiece(float delay)
    {
        if (rb == null) return;

        rb.isKinematic = false;
        rb.useGravity = true;

        Vector3 randomForce = new Vector3(
            Random.Range(-1f, 1f),
            Random.Range(0.5f, 1f),
            Random.Range(-1f, 1f)
        );

        Invoke(nameof(ApplyRandomForce), delay);
    }

    private void ApplyRandomForce()
    {
        rb.AddForce(rb.transform.up * Random.Range(2f, 5f), ForceMode.Impulse);
        rb.AddTorque(Random.insideUnitSphere * 5f, ForceMode.Impulse);
    }
}
