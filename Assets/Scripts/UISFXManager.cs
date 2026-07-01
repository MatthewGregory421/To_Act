using UnityEngine;
using FMODUnity;

[RequireComponent(typeof(StudioEventEmitter))]
public class UISFXManager : MonoBehaviour
{
    public static UISFXManager Instance;

    [Header("FMOD")]
    [SerializeField] private StudioEventEmitter UISFX;

    [SerializeField]
    [ParamRef]
    private string UISelector = null;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        UISFX = GetComponent<StudioEventEmitter>();
    }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    public void PlayUIBack()
    {
        PlayUISound(0);
    }

    public void PlayUICloseMenu()
    {
        PlayUISound(1);
    }

    public void PlayUIConfirm()
    {
        PlayUISound(2);
    }

    public void PlayUIForward()
    {
        PlayUISound(3);
    }

    public void PlayUIOpenMenu()
    {
        PlayUISound(4);
    }

    private void PlayUISound(float selectorValue)
    {
        if (UISFX == null)
        {
            UISFX = GetComponent<StudioEventEmitter>();
        }

        if (UISFX == null)
        {
            Debug.LogWarning("UISFXManager: UISFX emitter is missing.");
            return;
        }

        if (string.IsNullOrEmpty(UISelector))
        {
            Debug.LogWarning("UISFXManager: UISelector parameter is missing.");
            return;
        }

        RuntimeManager.StudioSystem.setParameterByName(UISelector, selectorValue);
        UISFX.Play();
    }
}