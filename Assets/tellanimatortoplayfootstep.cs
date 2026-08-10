using UnityEngine;

public class tellanimatortoplayfootstep : MonoBehaviour
{
    [SerializeField] private PlayerAnimations p;

    public void PlayFootstep()
    {
        p.PlayFootstep();
    }

    public void FinishPickupAnimation()
    {
        p.FinishPickupAnimation();
    }
}