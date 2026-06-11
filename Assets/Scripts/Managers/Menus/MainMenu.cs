using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    public UISFXManager uiSFXManager;

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

    // =========================
    // MAIN MENU
    // =========================
    public void PlayGame()
    {
        uiSFXManager?.PlayUIConfirm();

        RefreshSlotUI();

        mainMenuPanel.SetActive(false);
        slotSelectPanel.SetActive(true);
    }

    public void OpenOptions()
    {
        uiSFXManager?.PlayUIOpenMenu();

        mainMenuPanel.SetActive(false);
        optionsPanel.SetActive(true);
    }

    public void QuitGame()
    {
        uiSFXManager?.PlayUIConfirm();
        Application.Quit();
    }

    // =========================
    // SLOT SELECTION
    // =========================
    public void SelectSlot(int slot)
    {
        selectedSlot = slot;

        uiSFXManager?.PlayUIOpenMenu();

        if (SaveManager.Instance.HasSave(slot))
            LoadGame(slot);
        else
            NewGame(slot);
    }

    public void OpenDeleteConfirm(int slot)
    {
        uiSFXManager?.PlayUIOpenMenu();

        slotPendingDelete = slot;

        deleteConfirmPanel.SetActive(true);
    }

    public void CancelDelete()
    {
        uiSFXManager?.PlayUIOpenMenu();

        deleteConfirmPanel.SetActive(false);
    }

    public void ConfirmDelete()
    {
        uiSFXManager?.PlayUIConfirm();

        SaveManager.Instance.DeleteSave(slotPendingDelete);

        deleteConfirmPanel.SetActive(false);

        RefreshSlotUI();
    }

    public void NewGame(int slot)
    {
        uiSFXManager?.PlayUIConfirm();

        SaveManager.Instance.DeleteSave(slot);
        SaveManager.Instance.RequestNewGame(slot);

        SceneManager.LoadScene("Bootstrap");
    }

    public void LoadGame(int slot)
    {
        uiSFXManager?.PlayUIConfirm();

        if (!SaveManager.Instance.HasSave(slot))
            return;

        SaveManager.Instance.RequestLoadGame(slot);

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
        uiSFXManager?.PlayUIOpenMenu();
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
                {
                    slotTexts[i].text = "Last Checkpoint: " + data.benchID;
                }
                else
                {
                    slotTexts[i].text = "Empty Slot";
                }
            }
            else
            {
                slotTexts[i].text = "Empty Slot";
            }
        }
    }
}