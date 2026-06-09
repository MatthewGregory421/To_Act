using UnityEngine;

public class Hover : MonoBehaviour
{
    [SerializeField] private float hoverHeight = 0.2f;
    [SerializeField] private float hoverSpeed = 2f;

    private Vector3 startPosition;

    private void Start()
    {
        startPosition = transform.position;
    }

    private void Update()
    {
        float yOffset = Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;
        transform.position = startPosition + Vector3.up * yOffset;
    }
}