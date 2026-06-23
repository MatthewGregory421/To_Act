using System.IO;
using UnityEngine;

public class SaveManager : MonoBehaviour
{
    public static SaveManager Instance;

    public enum SaveMode
    {
        None,
        NewGame,
        LoadGame
    }

    public SaveMode pendingMode = SaveMode.None;
    public int pendingSlot = -1;
    public int currentSlot = -1;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void RequestNewGame(int slot)
    {
        pendingMode = SaveMode.NewGame;
        pendingSlot = slot;
        currentSlot = slot;
    }

    public void RequestLoadGame(int slot)
    {
        pendingMode = SaveMode.LoadGame;
        pendingSlot = slot;
        currentSlot = slot;
    }

    public bool HasRequest()
    {
        return pendingMode != SaveMode.None;
    }

    public void Clear()
    {
        pendingMode = SaveMode.None;
        pendingSlot = -1;
    }

    // SAVE FILES
    private string GetPath(int slot)
    {
        return Application.persistentDataPath + $"/save_slot_{slot}.json";
    }

    public void SaveGame(PlayerMovementInputSystem player, string sceneName, int slot)
    {
        SaveData data = new SaveData();

        data.sceneName = sceneName;
        data.benchID = WorldStateManager.Instance.GetCurrentBench();
        data.hasShield = player.hasShield;
        data.hasGroundSlam = player.hasGroundSlam;
        data.collectedPickups = WorldStateManager.Instance.GetCollectedPickups();

        File.WriteAllText(GetPath(slot), JsonUtility.ToJson(data, true));
    }

    public SaveData LoadGame(int slot)
    {
        string path = GetPath(slot);

        if (!File.Exists(path))
            return null;

        return JsonUtility.FromJson<SaveData>(File.ReadAllText(path));
    }

    public bool HasSave(int slot)
    {
        return File.Exists(GetPath(slot));
    }

    public void DeleteSave(int slot)
    {
        string path = GetPath(slot);

        Debug.Log($"Deleting save: {path}");

        if (File.Exists(path))
            File.Delete(path);
    }
}