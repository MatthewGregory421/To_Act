using UnityEngine;

public class BackToOptionsPanel : MonoBehaviour
{
    [Header("Panels")]
    public GameObject optionsPanel;
    public GameObject keyBindingsPanel;



    public void CloseKeyBindings()
    {
        if (UISFXManager.Instance != null)
        {
            UISFXManager.Instance.PlayUICloseMenu();
        }

        optionsPanel.SetActive(true);
        keyBindingsPanel.SetActive(false);
    }
}
