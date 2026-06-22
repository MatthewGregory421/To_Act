using UnityEngine;

public class CameraLag : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Lag Settings")]
    [Range(0.01f, 1f)]
    public float smoothTime = 0.15f;

    private Vector3 velocity = Vector3.zero;

    void LateUpdate()
    {
        if (target == null) return;

        Vector3 targetPosition = new Vector3(
            target.position.x,
            target.position.y,
            transform.position.z // keep camera's Z position
        );

        transform.position = Vector3.SmoothDamp(
            transform.position,
            targetPosition,
            ref velocity,
            smoothTime
        );
    }
}