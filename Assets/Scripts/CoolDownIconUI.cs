using UnityEngine;
using UnityEngine.UI;

public class CoolDownIconUI : MonoBehaviour
{
    [SerializeField] private Image fillImage;

    public void SetCooldownProgress(float current, float max)
    {
        fillImage.fillAmount = 1f - (current / max);
    }

    public void SetReady()
    {
        fillImage.fillAmount = 1f;
    }
}