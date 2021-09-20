using Mirror;
using System;
using UnityEngine;
using UnityEngine.EventSystems;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private GameObject unitPrefab = null;
    [SerializeField] private Transform spawnPoint = null;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!hasAuthority) return;
        CmdSpawnUnit();
    }
    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();
        health.ServerOnDie += HandleServerDie;
    }
    public override void OnStopServer()
    {
        base.OnStopServer();
        health.ServerOnDie -= HandleServerDie;
    }
    [Server]
    private void HandleServerDie()
    {
        NetworkServer.Destroy(gameObject);
    }

    [Command]
    private void CmdSpawnUnit()
    {
        GameObject unitInstance = Instantiate(
            unitPrefab,
            spawnPoint.position,
            spawnPoint.rotation);

        NetworkServer.Spawn(unitInstance, connectionToClient);

    }

    #endregion

    #region Client

    //ss

    #endregion
}
