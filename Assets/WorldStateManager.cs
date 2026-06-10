using System.Collections.Generic;
using UnityEngine;

public class WorldStateManager : MonoBehaviour
{
    public static WorldStateManager Instance;

    private HashSet<string> deadEnemies = new HashSet<string>();

    private string currentBenchID;
    private Vector3 currentBenchPosition;

    private string currentSceneName;

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

    // -------------------------
    // ENEMY PERSISTENCE
    // -------------------------

    public void KillEnemy(string enemyID)
    {
        deadEnemies.Add(enemyID);
    }

    public bool IsEnemyDead(string enemyID)
    {
        return deadEnemies.Contains(enemyID);
    }

    public void SetCurrentBench(string benchID, Vector3 position)
    {
        currentBenchID = benchID;
        currentBenchPosition = position;
    }

    public string GetCurrentBench()
    {
        if (string.IsNullOrEmpty(currentBenchID))
        {
            Debug.LogWarning("No bench set yet!");
        }

        return currentBenchID;
    }

    public Vector3 GetBenchPosition()
    {
        return currentBenchPosition;
    }

    public void RestAtBench()
    {
        deadEnemies.Clear();
    }

    public void SetCurrentScene(string sceneName)
    {
        currentSceneName = sceneName;
    }

    public string GetCurrentScene()
    {
        return currentSceneName;
    }
}