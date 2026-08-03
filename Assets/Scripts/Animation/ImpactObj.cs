using Unity.VisualScripting;
using UnityEngine;

public class ImpactObj : MonoBehaviour
{
    [SerializeField] Animator anim;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (!anim.GetCurrentAnimatorStateInfo(0).IsName("Impact"))
        {
            Destroy(gameObject);
        }
    }
}
