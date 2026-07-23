using UnityEngine;
using FMODUnity;

public class DestructibleFloor : MonoBehaviour
{
    [Header("Health")]
    public int health = 1;

    [Header("Audio")]
    [SerializeField]
    private EventReference groundBreakSound;

    private bool hasBroken;

    public void TakeSlamHit(int damage)
    {
        if (hasBroken)
            return;

        health -= damage;

        if (health <= 0)
        {
            Break();
        }
    }

    private void Break()
    {
        if (hasBroken)
            return;

        hasBroken = true;

        PlayGroundBreakSound();

        Destroy(gameObject);
    }

    private void PlayGroundBreakSound()
    {
        if (groundBreakSound.IsNull)
            return;

        RuntimeManager.PlayOneShot(
            groundBreakSound,
            transform.position
        );
    }
}