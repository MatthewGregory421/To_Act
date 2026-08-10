using UnityEngine;

public class SpinObject2D : MonoBehaviour
{
    [SerializeField] private float spinSpeed = 90f;

    private void Update()
    {
        transform.Rotate(0f, 0f, spinSpeed * Time.deltaTime);
    }
}