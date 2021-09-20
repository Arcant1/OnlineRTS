using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    [SerializeField] Health health = null;
    [SerializeField] Image foregroundImage = null;
    [SerializeField] Canvas canvas = null;

    float amount = 0;
    void UpdateBar(int currentHealth, int maxHealth)
    {
        amount = currentHealth / (float)maxHealth;
        if (Mathf.Approximately(amount, 0) || Mathf.Approximately(amount, 1))
        {
            canvas.enabled = false;
            return;
        }
        canvas.enabled = true;
        foregroundImage.fillAmount = amount;
    }
    private void OnMouseEnter()
    {
        canvas.enabled = true;
    }
    private void OnMouseExit()
    {
        canvas.enabled = false;
    }
    private void OnEnable()
    {
        health.ClientOnHealthChanged += UpdateBar;
    }
    private void OnDisable()
    {
        health.ClientOnHealthChanged -= UpdateBar;
    }
}
