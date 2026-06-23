using UnityEngine;

public class SFXBankLoaderManager : MonoBehaviour
{
    public static SFXBankLoaderManager Instance;

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
}