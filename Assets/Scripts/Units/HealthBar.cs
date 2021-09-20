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
        foregroundImage.transform.localScale = new Vector3(amount, 1f, 1f);
    }
    private void OnMouseEnter()
    {
        print("enter bar");
        canvas.enabled = true;
    }
    private void OnMouseExit()
    {
        print("exit bar");
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
