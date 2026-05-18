using UnityEngine;
using TMPro;

public class FloatingText : MonoBehaviour
{
    public float floatSpeed = 1.5f;
    public float lifetime = 1.5f;

    private TMP_Text text;
    private Color startColor;

    void Start()
    {
        text = GetComponent<TMP_Text>();
        startColor = text.color;
    }

    void Update()
    {
        // Move upward
        transform.position += Vector3.up * floatSpeed * Time.deltaTime;

        // Fade out
        lifetime -= Time.deltaTime;
        float alpha = lifetime / 1.5f;

        text.color = new Color(startColor.r, startColor.g, startColor.b, alpha);

        if (lifetime <= 0)
        {
            Destroy(gameObject);
        }
    }
}