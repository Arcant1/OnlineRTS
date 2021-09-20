using System.Collections.Generic;
using Mirror;
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

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        if (NetworkServer.active) return;
        Unit.AuthorityOnUnitSpawned += AuthorityHandlerUnitSpawn;
        Unit.AuthorityOnUnitDespawned += AuthorityHandlerUnitDespawn;

    }
    public override void OnStopClient()
    {
        base.OnStopClient();
        if (!isClientOnly || !hasAuthority) return;
        Unit.AuthorityOnUnitSpawned -= AuthorityHandlerUnitSpawn;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandlerUnitDespawn;

    }

    private void AuthorityHandlerUnitSpawn(Unit unit)
    {
        myUnits.Add(unit);
    }

    private void AuthorityHandlerUnitDespawn(Unit unit)
    {
        myUnits.Remove(unit);
    }
    #endregion
}
