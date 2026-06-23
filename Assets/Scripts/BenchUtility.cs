using UnityEngine;

public static class BenchUtility
{
    public static Bench FindBench(string benchID)
    {
        Bench[] benches = Object.FindObjectsByType<Bench>(
            FindObjectsSortMode.None
        );

        foreach (var bench in benches)
        {
            if (bench.benchID == benchID)
                return bench;
        }

        Debug.LogWarning("Bench not found: " + benchID);
        return null;
    }
}