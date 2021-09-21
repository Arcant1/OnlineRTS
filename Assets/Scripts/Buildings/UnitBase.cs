using Mirror;
using UnityEngine;
using System;

public class UnitBase : NetworkBehaviour
{
    [SerializeField] private Health health = null;
    public static event Action<int> ServerOnPlayerDie;
    public static event Action<UnitBase> ServerOnBaseSpawn;
    public static event Action<UnitBase> ServerOnBaseDespawn;
    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();
        health.ServerOnDie += ServerHandleDie;
        ServerOnBaseSpawn?.Invoke(this);
    }

    [Server]
    private void ServerHandleDie()
    {
        ServerOnBaseDespawn?.Invoke(this);
        ServerOnPlayerDie?.Invoke(connectionToClient.connectionId);
        NetworkServer.Destroy(gameObject);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        health.ServerOnDie -= ServerHandleDie;

    }
    #endregion

    #region Client

    #endregion
}
