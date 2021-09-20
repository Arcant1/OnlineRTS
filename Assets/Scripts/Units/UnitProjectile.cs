using UnityEngine;
using Mirror;

public class UnitProjectile : NetworkBehaviour
{
    [SerializeField] private Rigidbody rb = null;
    [SerializeField] private float launchForce = 10f;
    [SerializeField] private float secondsToLive = 5f;
    [SerializeField] private int damage = 10;

    private void Start()
    {
        rb.velocity = transform.forward * launchForce;
    }
    public override void OnStartServer()
    {
        base.OnStartServer();
        Invoke(nameof(DestroySelf), secondsToLive);
    }
    [Server]
    private void DestroySelf()
    {
        NetworkServer.Destroy(gameObject);
    }

    [ServerCallback]
    void OnTriggerEnter(Collider other)
    {
        if(other.TryGetComponent<NetworkIdentity>(out NetworkIdentity networkIdentity))
        {
            if (networkIdentity.connectionToClient == connectionToClient) return;
        }
        if (other.TryGetComponent<Health>(out Health health))
        {
            health.DealDamage(damage);
        }
        if (other.GetComponent<UnitProjectile>() == null)
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}
