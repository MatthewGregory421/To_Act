using UnityEngine;
using UnityEngine.Tilemaps;

public class FirstBenchTutorial : MonoBehaviour
{
    [Header("Bench To Watch")]
    public string requiredBenchID;

    [Header("Objects To Remove")]
    public GameObject tutorialCanvas;
    public GameObject blockingWall;

    [Header("Tilemap Destruction")]
    [SerializeField] private Tilemap wallTilemap;

    private bool unlocked;

    private void Start()
    {
        CheckBench();
    }

    private void Update()
    {
        if (unlocked)
            return;

        CheckBench();
    }

    private void CheckBench()
    {
        if (WorldStateManager.Instance == null)
            return;

        string currentBench =
            WorldStateManager.Instance.GetCurrentBench();

        if (currentBench != requiredBenchID)
            return;

        UnlockTutorial();
    }

    private void UnlockTutorial()
    {
        unlocked = true;

        Debug.Log(
            "First bench reached. Removing tutorial blockers."
        );

        if (tutorialCanvas != null)
        {
            Destroy(tutorialCanvas);
        }

        if (blockingWall != null)
        {
            Destroy(blockingWall);
        }

        if (wallTilemap != null)
        {
            Destroy(wallTilemap.gameObject);
        }
    }
}