using Mirror;
using UnityEngine;

public class UnitFiring : NetworkBehaviour
{
    [SerializeField] Targeter targeter = null;
    [SerializeField] GameObject projectilePrefab = null;
    [SerializeField] Transform projectileSpawnPoint = null;
    [SerializeField] float fireRange = 11f;
    [SerializeField] float fireRate = 3f; //units by second
    [SerializeField] float rotationSpeed = 20f;

    private float lastFireTime;
    private Targetable target;

    [ServerCallback]
    void Update()
    {
        Targetable target = targeter.GetTarget();
        if (target == null) return;
        if (!CanFireAtTarget()) return;
        RotateAtTarget();
        Fire();
    }

    private void Fire()
    {
        if (Time.time > lastFireTime + (1 / fireRate))
        {
            lastFireTime = Time.time;
            Quaternion projectileRotation = Quaternion.LookRotation(
                targeter.GetTarget().GetAimAtPoint().position -
                projectileSpawnPoint.position
                );
            GameObject projectileInstance = Instantiate(
                projectilePrefab,
                projectileSpawnPoint.position,
                projectileRotation
                );

            NetworkServer.Spawn(
                projectileInstance,
                connectionToClient      // Owner of this Projectile
                );
        }
    }

    private void RotateAtTarget()
    {
        Quaternion targetRotation =
                    Quaternion.LookRotation(
                        targeter.GetTarget().transform.position - transform.position);
        transform.rotation =
            Quaternion.RotateTowards(
                transform.rotation, targetRotation,
                rotationSpeed * Time.deltaTime);
    }

    [Server]
    private bool CanFireAtTarget()
    {
        return (targeter.GetTarget().transform.position - transform.position).sqrMagnitude <=
            fireRange * fireRange;
    }
}
