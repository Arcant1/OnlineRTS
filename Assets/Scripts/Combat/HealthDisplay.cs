using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private RectTransform healthBar = null;
    [SerializeField] private Health health;
    private Camera mainCamera;
    private float maxWidth;
    private void Start()
    {
        mainCamera = Camera.main;
        maxWidth = healthBar.sizeDelta.x;
    }

    private void Update()
    {
        transform.up = mainCamera.transform.up;
        healthBar.sizeDelta = new Vector2(
            health.GetHealthPercentage() * maxWidth,
            healthBar.sizeDelta.y
            );
    }


}
