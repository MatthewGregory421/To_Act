using System.Collections.Generic;
using UnityEngine;

public class WorldStateManager : MonoBehaviour
{
    public static WorldStateManager Instance;

    private HashSet<string> deadEnemies = new HashSet<string>();
    private HashSet<string> collectedPickups = new HashSet<string>();
    private HashSet<string> playedNarrations = new HashSet<string>();

    private string currentBenchID;
    private string currentSceneName;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
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

    public void PlayNarrationTrigger(string narrationID)
    {
        playedNarrations.Add(narrationID);
    }

    public bool HasPlayedNarrationTrigger(string narrationID)
    {
        return playedNarrations.Contains(narrationID);
    }

    public void RestAtBench()
    {
        RespawnEnemiesFromBench();

        // Do NOT clear collectedPickups here.
        // Ability pickups should stay collected permanently.
    }

    // -------------------------
    // PICKUP PERSISTENCE
    // -------------------------

    public void CollectPickup(string pickupID)
    {
        collectedPickups.Add(pickupID);
    }

    public bool IsPickupCollected(string pickupID)
    {
        return collectedPickups.Contains(pickupID);
    }

    public List<string> GetCollectedPickups()
    {
        return new List<string>(collectedPickups);
    }

    public void SetCollectedPickups(List<string> pickups)
    {
        if (pickups == null)
        {
            collectedPickups = new HashSet<string>();
            return;
        }

        collectedPickups = new HashSet<string>(pickups);
    }

    // -------------------------
    // BENCH / SCENE
    // -------------------------

    public void SetCurrentBench(string benchID)
    {
        currentBenchID = benchID;
    }

    public string GetCurrentBench()
    {
        if (string.IsNullOrEmpty(currentBenchID))
        {
            Debug.LogWarning("No bench set yet!");
        }

        return currentBenchID;
    }

    public void RespawnEnemiesFromBench()
    {
        deadEnemies.Clear();

        EnemyBase[] enemies = FindObjectsByType<EnemyBase>(
            FindObjectsInactive.Include,
            FindObjectsSortMode.None
        );

        foreach (EnemyBase enemy in enemies)
        {
            enemy.RespawnEnemy();
        }
    }

    public void SetCurrentScene(string sceneName)
    {
        currentSceneName = sceneName;
    }

    public string GetCurrentScene()
    {
        return currentSceneName;
    }

    public void ApplySave(SaveData data)
    {
        currentSceneName = data.sceneName;
        currentBenchID = data.benchID;

        SetCollectedPickups(data.collectedPickups);

        ApplyCollectedAbilitiesToPlayer();
    }

    public void ApplyCollectedAbilitiesToPlayer()
    {
        PlayerMovementInputSystem player =
            FindFirstObjectByType<PlayerMovementInputSystem>();

        if (player == null)
        {
            Debug.LogWarning("No player found to apply collected abilities to.");
            return;
        }

        List<string> pickups = GetCollectedPickups();

        player.hasShield = pickups.Contains("ShieldPickup");
        player.hasGroundSlam = pickups.Contains("GroundSlamPickup");

        player.UpdateAbilityUI();
    }

    public void ResetWorldStateForNewGame()
    {
        deadEnemies.Clear();
        collectedPickups.Clear();
        playedNarrations.Clear();

        currentBenchID = "";
        currentSceneName = "";

        PlayerMovementInputSystem player =
            FindFirstObjectByType<PlayerMovementInputSystem>();

        if (player != null)
        {
            player.hasShield = false;
            player.hasGroundSlam = false;
            player.UpdateAbilityUI();
        }

        Debug.Log("World state fully reset for new game.");
    }
}