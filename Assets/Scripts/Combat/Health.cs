using Mirror;
using System;
using UnityEngine;

public class Health : NetworkBehaviour
{
    [SerializeField] private int maxHealth = 30;
    [SyncVar(hook = nameof(HandleHealthUpdated))]
    private int currentHealth;
    public event Action ServerOnDie;
    public event Action<int, int> ClientOnHealthChanged;
    public int GetCurrentHealth() => currentHealth;

    public float GetHealthPercentage() => (float)currentHealth / (float)maxHealth;
    public int GetMaxHealth() => maxHealth;

    public override void OnStartServer()
    {
        base.OnStartServer();
        currentHealth = maxHealth;
    }

    #region Server
    [Server]
    public void DealDamage(int damage)
    {
        if (damage <= 0) return;
        if (currentHealth == 0) return;
        currentHealth = Mathf.Max(0, currentHealth - damage);
        if (currentHealth > 0) return;
        ServerOnDie?.Invoke();
        Die();
    }

    public void Die()
    {
        print($"{gameObject.name} has died");
        NetworkServer.Destroy(gameObject);
    }
    #endregion


    #region Client
    private void HandleHealthUpdated(int oldHealth, int newHealth)
    {
        ClientOnHealthChanged?.Invoke(newHealth, maxHealth);
    }

    #endregion
}
