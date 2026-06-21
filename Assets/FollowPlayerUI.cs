using UnityEngine;

public class FollowPlayerUI : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform player;
    [SerializeField] private Vector3 worldOffset = new Vector3(0f, 7f, 0f);

    private RectTransform rectTransform;
    private Camera mainCamera;

    private void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        mainCamera = Camera.main;
    }

    private void Start()
    {
        FindPlayer();
    }

    private void LateUpdate()
    {
        if (player == null)
            FindPlayer();

        if (player == null || mainCamera == null)
            return;

        Vector3 screenPosition =
            mainCamera.WorldToScreenPoint(player.position + worldOffset);

        rectTransform.position = screenPosition;
    }

    private void FindPlayer()
    {
        GameObject found = GameObject.FindGameObjectWithTag("Player");

        if (found != null)
            player = found.transform;

        if (mainCamera == null)
            mainCamera = Camera.main;
    }
}