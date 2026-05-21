using UnityEngine;

public class DynamicBackgrounds : MonoBehaviour
{
    [SerializeField]  SpriteRenderer BG1;
    [SerializeField] SpriteRenderer BG2;

    [SerializeField] int index = 1;
    [SerializeField] float speed = 10f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(index == 1)
        {
            BG1.color = Color.Lerp(new Color(BG1.color.r, BG1.color.g, BG1.color.b, BG1.color.a), new Color(BG1.color.r, BG1.color.g, BG1.color.b, 1), Time.deltaTime * speed);
            BG2.color = Color.Lerp(new Color(BG2.color.r, BG2.color.g, BG2.color.b, BG2.color.a), new Color(BG2.color.r, BG2.color.g, BG2.color.b, 0), Time.deltaTime * speed);
            
        }
        if (index == 2)
        {
            BG2.color = Color.Lerp(new Color(BG2.color.r, BG2.color.g, BG2.color.b, BG2.color.a), new Color(BG2.color.r, BG2.color.g, BG2.color.b, 1), Time.deltaTime * speed);
            BG1.color = Color.Lerp(new Color(BG1.color.r, BG1.color.g, BG1.color.b, BG1.color.a), new Color(BG1.color.r, BG1.color.g, BG1.color.b, 0), Time.deltaTime * speed);
        }
    }
}
