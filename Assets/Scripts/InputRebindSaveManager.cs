using UnityEngine;
using UnityEngine.InputSystem;

public class InputRebindSaveManager : MonoBehaviour
{
    public static InputRebindSaveManager Instance;

    [SerializeField] private InputActionAsset inputActions;

    private const string RebindSaveKey = "InputBindingOverrides";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        LoadBindings();
    }

    public static void SaveBindings()
    {
        if (Instance == null ||
            Instance.inputActions == null)
        {
            return;
        }

        string rebindData =
            Instance.inputActions.SaveBindingOverridesAsJson();

        PlayerPrefs.SetString(RebindSaveKey, rebindData);
        PlayerPrefs.Save();
    }

    public void LoadBindings()
    {
        if (inputActions == null)
            return;

        if (!PlayerPrefs.HasKey(RebindSaveKey))
            return;

        string rebindData =
            PlayerPrefs.GetString(RebindSaveKey);

        inputActions.LoadBindingOverridesFromJson(rebindData);
    }

    public void ResetAllBindings()
    {
        if (inputActions == null)
            return;

        inputActions.RemoveAllBindingOverrides();

        PlayerPrefs.DeleteKey(RebindSaveKey);
        PlayerPrefs.Save();

        RefreshRebindButtons();
    }

    private void RefreshRebindButtons()
    {
        RebindButton[] buttons =
            FindObjectsByType<RebindButton>(
                FindObjectsSortMode.None
            );

        foreach (RebindButton button in buttons)
        {
            button.UpdateBindingDisplay();
        }
    }
}