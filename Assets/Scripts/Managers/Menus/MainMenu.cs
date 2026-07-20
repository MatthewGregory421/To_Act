using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [Header("Panels")]
    public GameObject mainMenuPanel;
    public GameObject slotSelectPanel;
    public GameObject optionsPanel;

    [Header("Delete Confirm Panel")]
    public GameObject deleteConfirmPanel;

    [Header("Slot UI Text")]
    public TextMeshProUGUI[] slotTexts;

    [Header("Slot Ability Icons")]
    [Tooltip("One shield icon for each save slot, in slot order.")]
    public GameObject[] shieldIcons;

    [Tooltip("One ground-slam icon for each save slot, in slot order.")]
    public GameObject[] groundSlamIcons;

    private int slotPendingDelete;

    private void Start()
    {
        ShowMainMenu();
    }

    private void PlayConfirmSFX()
    {
        if (UISFXManager.Instance != null)
            UISFXManager.Instance.PlayUIConfirm();
    }

    private void PlayOpenSFX()
    {
        if (UISFXManager.Instance != null)
            UISFXManager.Instance.PlayUIOpenMenu();
    }

    private void PlayUIBackSFX()
    {
        if (UISFXManager.Instance != null)
            UISFXManager.Instance.PlayUIBack();
    }

    public void PlayGame()
    {
        PlayConfirmSFX();

        RefreshSlotUI();

        mainMenuPanel.SetActive(false);
        slotSelectPanel.SetActive(true);
    }

    public void OpenOptions()
    {
        PlayOpenSFX();

        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        PlayConfirmSFX();
        Application.Quit();
    }

    public void SelectSlot(int slot)
    {
        PlayOpenSFX();

        if (SaveManager.Instance.HasSave(slot))
            LoadGame(slot);
        else
            NewGame(slot);
    }

    public void OpenDeleteConfirm(int slot)
    {
        PlayOpenSFX();

        slotPendingDelete = slot;
        deleteConfirmPanel.SetActive(true);
    }

    public void CancelDelete()
    {
        PlayOpenSFX();
        deleteConfirmPanel.SetActive(false);
    }

    public void ConfirmDelete()
    {
        PlayConfirmSFX();

        SaveManager.Instance.DeleteSave(slotPendingDelete);
        deleteConfirmPanel.SetActive(false);

        RefreshSlotUI();
    }

    public void NewGame(int slot)
    {
        PlayConfirmSFX();

        SaveManager.Instance.DeleteSave(slot);
        SaveManager.Instance.RequestNewGame(slot);

        StartGameThroughBootstrap();
    }

    public void LoadGame(int slot)
    {
        PlayConfirmSFX();

        if (!SaveManager.Instance.HasSave(slot))
            return;

        SaveManager.Instance.RequestLoadGame(slot);

        StartGameThroughBootstrap();
    }

    private void StartGameThroughBootstrap()
    {
        SceneManager.LoadScene("Bootstrap");
    }

    private void ShowMainMenu()
    {
        mainMenuPanel.SetActive(true);
        slotSelectPanel.SetActive(false);
        optionsPanel.SetActive(false);
        deleteConfirmPanel.SetActive(false);

        RefreshSlotUI();
    }

    public void BackToMainMenu()
    {
        PlayUIBackSFX();
        ShowMainMenu();
    }

    private void RefreshSlotUI()
    {
        for (int i = 0; i < slotTexts.Length; i++)
        {
            SaveData data = SaveManager.Instance.LoadGame(i);

            if (data != null)
            {
                slotTexts[i].text = "Last Checkpoint: " + data.benchID;

                SetShieldIcon(i, data.hasShield);
                SetGroundSlamIcon(i, data.hasGroundSlam);
            }
            else
            {
                slotTexts[i].text = "Empty Slot";

                SetShieldIcon(i, false);
                SetGroundSlamIcon(i, false);
            }
        }
    }

    private void SetShieldIcon(int slot, bool shouldShow)
    {
        if (slot < 0 || slot >= shieldIcons.Length)
            return;

        if (shieldIcons[slot] != null)
            shieldIcons[slot].SetActive(shouldShow);
    }

    private void SetGroundSlamIcon(int slot, bool shouldShow)
    {
        if (slot < 0 || slot >= groundSlamIcons.Length)
            return;

        if (groundSlamIcons[slot] != null)
            groundSlamIcons[slot].SetActive(shouldShow);
    }
}