using UnityEngine;

public class Followplayer : MonoBehaviour
{
    [SerializeField] Vector3 offset = Vector3.zero;
    [SerializeField] GameObject objtofollow;
    public float smoothtime = 0.3f;
    public bool delay = true;
    private Vector3 currentvel = Vector3.zero;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    private void Awake()
    {
        gameObject.transform.SetParent(null);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (delay) { 
        transform.position = Vector3.SmoothDamp(transform.position, objtofollow.transform.position + offset, ref currentvel, Time.deltaTime * smoothtime);
        }
       
    }

    private void Update()
    {
        if (!delay)
        {
            
                transform.position = objtofollow.transform.position + offset;
            
        }
    }
}
