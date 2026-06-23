using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;


public class PlayerInputRecovery : MonoBehaviour
{
    private PlayerMovementInputSystem movement;
    private PlayerInput playerInput;

    private void Awake()
    {
        movement = GetComponent<PlayerMovementInputSystem>();
        playerInput = GetComponent<PlayerInput>();
    }

    private void OnEnable()
    {
        StartCoroutine(RecoverInputAfterLoad());
    }

    private IEnumerator RecoverInputAfterLoad()
    {
        yield return null;
        yield return null;

        if (movement != null)
            movement.enabled = true;

        if (playerInput != null)
            playerInput.enabled = true;

        Debug.Log("Player input recovered after load.");
    }
}
