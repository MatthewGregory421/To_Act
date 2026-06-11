using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEngine;
using UnityEngine.UI;

public class UIbridge : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    [SerializeField] Slider shieldCooldown;
    [SerializeField] Slider slamCooldown;

    [SerializeField] GameObject shieldicon;
    [SerializeField] GameObject slamicon;

    public float shieldCooldownValue = 1;
    public float slamCooldownValue = 1;
    [SerializeField] bool shieldenabled = true;
    [SerializeField] bool slamenabled = true;
    
    void Start()
    {
        slamicon.SetActive(slamenabled);
        shieldicon.SetActive(shieldenabled);
    }

    // Update is called once per frame
    void Update()
    {
       shieldCooldown.value = 1 - shieldCooldownValue;
       slamCooldown.value = 1 - slamCooldownValue;
    }

    
}
