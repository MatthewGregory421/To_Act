using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class SpinWhileFalling : MonoBehaviour
{
    [Header("Spin Settings")]
    [SerializeField] private float spinSpeed = 360f;
    [SerializeField] private float fallingThreshold = -0.05f;

    private Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void FixedUpdate()
    {
        bool isFalling = rb.linearVelocity.y < fallingThreshold;

        if (isFalling)
        {
            rb.angularVelocity = spinSpeed;
        }
        else
        {
            rb.angularVelocity = 0f;
        }
    }
}