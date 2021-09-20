using System;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    [SerializeField] private Health health;
    [SerializeField] private GameObject healthBarParent = null;
    [SerializeField] private Image healthBarImage = null;

    private Camera mainCamera;
    private void OnEnable()
    {
        health.ClientOnHealthChanged += HandleHealthUpdated;
    }
    private void OnDisable()
    {
        health.ClientOnHealthChanged -= HandleHealthUpdated;
    }
    private void HandleHealthUpdated(int currentHealth, int maxHealth)
    {
        healthBarImage.fillAmount = (float)currentHealth / maxHealth;
    }

    private void Start()
    {
        mainCamera = Camera.main;
    }

    private void OnMouseEnter()
    {
        healthBarParent.SetActive(true);
    }
    private void OnMouseExit()
    {
        healthBarParent.SetActive(false);
    }
    private void Update()
    {

    }


}
