using UnityEngine;

public class FirstBenchTutorial : MonoBehaviour
{
    [Header("Bench To Watch")]
    public string requiredBenchID;

    [Header("Objects To Remove")]
    public GameObject tutorialCanvas;
    public GameObject blockingWall;


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
        string currentBench = WorldStateManager.Instance.GetCurrentBench();

        if (currentBench == requiredBenchID)
        {
            unlocked = true;

            if (tutorialCanvas != null)
                Destroy(tutorialCanvas);

            if (blockingWall != null)
                Destroy(blockingWall);

            Debug.Log("First bench used. Tutorial gate removed.");
        }
    }
}
