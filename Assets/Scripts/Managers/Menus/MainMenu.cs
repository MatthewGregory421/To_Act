using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
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

    private int slotPendingDelete;
    private int selectedSlot;

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
        selectedSlot = slot;

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
            if (SaveManager.Instance.HasSave(i))
            {
                SaveData data = SaveManager.Instance.LoadGame(i);

                if (data != null)
                    slotTexts[i].text = "Last Checkpoint: " + data.benchID;
                else
                    slotTexts[i].text = "Empty Slot";
            }
            else
            {
                slotTexts[i].text = "Empty Slot";
            }
        }
    }
}