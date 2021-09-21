using System.Collections;
using Mirror;
using System.Collections.Generic;
using UnityEngine;
using System;

public class GameOverHandler : NetworkBehaviour
{
    public static event Action<string> ClientOnGameOver;
    public static event Action ServerOnGameOver;
    private List<UnitBase> bases = new List<UnitBase>();
    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();
        UnitBase.ServerOnBaseSpawn += ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawn += ServerHandleBaseDespawned;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();

        UnitBase.ServerOnBaseSpawn -= ServerHandleBaseSpawned;
        UnitBase.ServerOnBaseDespawn -= ServerHandleBaseDespawned;
    }
    [Server]
    private void ServerHandleBaseSpawned(UnitBase unitBase)
    {
        bases.Add(unitBase);
    }

    [Server]
    private void ServerHandleBaseDespawned(UnitBase unitBase)
    {
        bases.Remove(unitBase);
        if (bases.Count != 1) return; // there are more than 1 player
        var winner = bases[0].connectionToClient.connectionId.ToString();
        RpcGameOver($"Player { winner } won ");
        ServerOnGameOver?.Invoke();
    }
    #endregion


    #region Client

    [ClientRpc]
    private void RpcGameOver(string winnerName)
    {
        ClientOnGameOver?.Invoke(winnerName);
    }

    #endregion
}
