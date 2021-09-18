using System.Collections.Generic;
using Mirror;
using System;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private List<Unit> myUnits = null;

    public List<Unit> GetMyUnits() => myUnits;
    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();
        Unit.ServerOnUnitSpawned += ServerHandlerUnitSpawn;
        Unit.ServerOnUnitDespawned += ServerHandlerUnitDespawn;
    }
    public override void OnStopServer()
    {
        base.OnStopServer();
        Unit.ServerOnUnitSpawned -= ServerHandlerUnitSpawn;
        Unit.ServerOnUnitDespawned -= ServerHandlerUnitDespawn;
    }

    private void ServerHandlerUnitSpawn(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
        myUnits.Add(unit);
    }

    private void ServerHandlerUnitDespawn(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
        myUnits.Remove(unit);
    }
    #endregion
    #region Client

    public override void OnStartClient()
    {
        base.OnStartClient();
        if (!isClientOnly) return;
        Unit.AuthorityOnUnitSpawned += AuthorityHandlerUnitSpawn;
        Unit.AuthorityOnUnitDespawned += AuthorityHandlerUnitDespawn;

    }
    public override void OnStopClient()
    {
        base.OnStopClient();
        if (!isClientOnly) return;
        Unit.AuthorityOnUnitSpawned -= AuthorityHandlerUnitSpawn;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandlerUnitDespawn;

    }

    private void AuthorityHandlerUnitSpawn(Unit unit)
    {
        if (!hasAuthority) return;
        myUnits.Add(unit);
    }

    private void AuthorityHandlerUnitDespawn(Unit unit)
    {
        if (!hasAuthority) return;
        myUnits.Remove(unit);
    }



    #endregion
}
