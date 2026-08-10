using UnityEngine;

public class tellanimatortoplayfootstep : MonoBehaviour
{
    [SerializeField] PlayerAnimations p;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void PlayFootstep()
    {
        p.PlayFootstep();
    }
}
