using UnityEngine;
using System.Collections;

public class CameraLag : MonoBehaviour
{
    [Header("Target")]
    public Transform target;

    [Header("Camera Position")]
    public Vector3 offset = new Vector3(0f, 1.5f, 0f);

    [Header("Camera Size")]
    public float orthographicSize = 7f;

    [Header("Lag Settings")]
    public float smoothTime = 0.08f;
    public float maxDistanceFromPlayer = 0.75f;
    public float snapThreshold = 0.01f;

    [Header("Axis Control")]
    public bool followX = true;
    public bool followY = true;

    [Header("Camera Shake")]
    public float shakeDuration = 0.15f;
    public float shakeStrength = 0.12f;

    private Vector3 velocity;
    private Camera cam;

    private Vector3 currentShakeOffset;
    private Coroutine shakeCoroutine;

    public static CameraLag Instance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;

        cam = GetComponent<Camera>();

        if (cam != null)
        {
            cam.orthographic = true;
            cam.orthographicSize = orthographicSize;
        }
    }

    private void LateUpdate()
    {
        if (target == null) return;

        if (cam != null)
            cam.orthographicSize = orthographicSize;

        Vector3 desiredPosition = transform.position;

        if (followX)
            desiredPosition.x = target.position.x + offset.x;

        if (followY)
            desiredPosition.y = target.position.y + offset.y;

        desiredPosition.z = transform.position.z;

        float distance = Vector2.Distance(transform.position, desiredPosition);

        if (distance < snapThreshold)
        {
            transform.position = desiredPosition + currentShakeOffset;
            velocity = Vector3.zero;
            return;
        }

        Vector3 smoothedPosition = Vector3.SmoothDamp(
            transform.position,
            desiredPosition,
            ref velocity,
            smoothTime,
            Mathf.Infinity,
            Time.unscaledDeltaTime
        );

        Vector3 offsetFromTarget = smoothedPosition - desiredPosition;

        if (offsetFromTarget.magnitude > maxDistanceFromPlayer)
        {
            smoothedPosition = desiredPosition + offsetFromTarget.normalized * maxDistanceFromPlayer;
        }

        smoothedPosition.z = transform.position.z;

        transform.position = smoothedPosition + currentShakeOffset;
    }

    public void ShakeCamera()
    {
        if (shakeCoroutine != null)
            StopCoroutine(shakeCoroutine);

        shakeCoroutine = StartCoroutine(ShakeRoutine());
    }

    private IEnumerator ShakeRoutine()
    {
        float timer = 0f;

        while (timer < shakeDuration)
        {
            timer += Time.unscaledDeltaTime;

            currentShakeOffset = new Vector3(
                Random.Range(-shakeStrength, shakeStrength),
                Random.Range(-shakeStrength, shakeStrength),
                0f
            );

            yield return null;
        }

        currentShakeOffset = Vector3.zero;
        shakeCoroutine = null;
    }
}