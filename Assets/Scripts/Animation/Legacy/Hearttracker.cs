using NUnit.Framework;
using UnityEngine;

public class Hearttracker : MonoBehaviour
{
    PlayerHealth pHealth;
    [SerializeField] GameObject player;
    [SerializeField] GameObject heartIcon;
    private GameObject[] heartobjects;
    bool initialized = false;

    int currenthearts;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }
    private void Awake()
    {
        pHealth = player.GetComponent<PlayerHealth>();
        ReinitializeHealthHUD();
    }

    // Update is called once per frame
    void Update()
    {
        
        

        while (currenthearts != pHealth.currentHealth && pHealth.currentHealth >= 0 && initialized)
        {
            if (currenthearts > pHealth.currentHealth) 
            {
                removeHeart();
            
            }
            if(currenthearts < pHealth.currentHealth)
            {
                addHeart();
            }
        }
    }

    //If player max health changes
    public void ReinitializeHealthHUD()
    {
        initialized = false;
        heartobjects = new GameObject[pHealth.maxHealth];
        for (int i = 0; i < heartobjects.Length; i++)
        {
            heartobjects[i] = Instantiate(heartIcon, gameObject.transform.position + new Vector3(i * 0.5f, 0,0), gameObject.transform.rotation, gameObject.transform);
        }
        initialized = true;
        currenthearts = pHealth.maxHealth;
    }

    public void SyncHealth()
    {
        for (int i = 0; i < heartobjects.Length; i++)
        {
            heartobjects[i].SetActive(true);
        }
        for (currenthearts = pHealth.maxHealth ; currenthearts > pHealth.currentHealth; currenthearts--)
        {
            heartobjects[currenthearts-1].SetActive(false);
        }
    }

    public void removeHeart()
    {
        heartobjects[currenthearts - 1].SetActive(false);
        currenthearts--;
    }

    public void addHeart()
    {
        heartobjects[currenthearts - 1].SetActive(true);
        currenthearts++;
    }
}
