using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindButton : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private InputActionReference actionReference;

    [Tooltip("The binding index belonging to this button.")]
    [SerializeField] private int bindingIndex;

    [Header("UI")]
    [SerializeField] private TMP_Text bindingText;
    [SerializeField] private TMP_Text waitingText;
    [SerializeField] private Button rebindButton;

    private InputActionRebindingExtensions.RebindingOperation
        rebindingOperation;

    private void Start()
    {
        UpdateBindingDisplay();

        if (waitingText != null)
            waitingText.gameObject.SetActive(false);
    }

    public void StartRebinding()
    {
        if (actionReference == null)
        {
            Debug.LogWarning("RebindButton: No Input Action assigned.");
            return;
        }

        InputAction action = actionReference.action;

        if (bindingIndex < 0 ||
            bindingIndex >= action.bindings.Count)
        {
            Debug.LogWarning(
                $"RebindButton: Invalid binding index for {action.name}."
            );

            return;
        }

        action.Disable();

        if (bindingText != null)
            bindingText.gameObject.SetActive(false);

        if (waitingText != null)
            waitingText.gameObject.SetActive(true);

        if (rebindButton != null)
            rebindButton.interactable = false;

        rebindingOperation = action
            .PerformInteractiveRebinding(bindingIndex)
            .WithCancelingThrough("<Keyboard>/escape")
            .OnCancel(operation =>
            {
                FinishRebinding();
            })
            .OnComplete(operation =>
            {
                FinishRebinding();
                InputRebindSaveManager.SaveBindings();
            });

        rebindingOperation.Start();
    }

    private void FinishRebinding()
    {
        rebindingOperation?.Dispose();
        rebindingOperation = null;

        actionReference.action.Enable();

        if (bindingText != null)
            bindingText.gameObject.SetActive(true);

        if (waitingText != null)
            waitingText.gameObject.SetActive(false);

        if (rebindButton != null)
            rebindButton.interactable = true;

        UpdateBindingDisplay();
    }

    public void UpdateBindingDisplay()
    {
        if (actionReference == null ||
            bindingText == null)
        {
            return;
        }

        InputAction action = actionReference.action;

        if (bindingIndex < 0 ||
            bindingIndex >= action.bindings.Count)
        {
            bindingText.text = "Not Assigned";
            return;
        }

        bindingText.text =
            action.GetBindingDisplayString(bindingIndex);
    }

    private void OnDisable()
    {
        rebindingOperation?.Cancel();
        rebindingOperation?.Dispose();
        rebindingOperation = null;
    }
}