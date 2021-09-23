using System;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    private List<Unit> myUnits = new List<Unit>();
    private List<Building> myBuildings = new List<Building>();
    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    private int resources = 500;
    public event Action<int> ClientOnResourcesChanged;
    private void ClientHandleResourcesUpdated(int oldValue, int newValue)
    {
        ClientOnResourcesChanged?.Invoke(newValue);
    }

    [SerializeField] private Building[] buildings = new Building[0];
    public int GetResources() => resources;
    public List<Unit> GetMyUnits() => myUnits;
    public List<Building> GetMyBuildings() => myBuildings;

    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();
        Unit.ServerOnUnitSpawned += ServerHandlerUnitSpawn;
        Unit.ServerOnUnitDespawned += ServerHandlerUnitDespawn;
        Building.ServerOnBuildingSpawned += ServerHandlerBuildingSpawn;
        Building.ServerOnBuildingDespawned += ServerHandlerBuildingDespawn;
    }
    [Server]
    public void AddResource(int amount)
    {
        if (amount <= 0) return;
        resources = Mathf.Max(resources, resources + amount);
    }
    [Server]
    public void SetResources(int newResources)
    {
        resources = newResources;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        Unit.ServerOnUnitSpawned -= ServerHandlerUnitSpawn;
        Unit.ServerOnUnitDespawned -= ServerHandlerUnitDespawn;
        Building.ServerOnBuildingSpawned -= ServerHandlerBuildingSpawn;
        Building.ServerOnBuildingDespawned -= ServerHandlerBuildingDespawn;
    }

    private void ServerHandlerBuildingDespawn(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;
        myBuildings.Remove(building);
    }

    private void ServerHandlerBuildingSpawn(Building building)
    {
        if (building.connectionToClient.connectionId != connectionToClient.connectionId) return;
        myBuildings.Add(building);
    }

    private void ServerHandlerUnitSpawn(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId)
        {
            return;
        }
        myUnits.Add(unit);
    }

    private void ServerHandlerUnitDespawn(Unit unit)
    {
        if (unit.connectionToClient.connectionId != connectionToClient.connectionId) return;
        myUnits.Remove(unit);
    }

    [Command]
    public void CmdTryPlaceBuilding(int buildingId, Vector3 position)
    {
        Building buildingToPlace = null;
        foreach (Building building in buildings)
        {
            if (building.GetId() == buildingId)
            {
                buildingToPlace = building;
                break;
            }
        }
        if (!buildingToPlace) return;
        GameObject buildingInstance = Instantiate(buildingToPlace.gameObject, position, buildingToPlace.transform.rotation);
        print(buildingInstance == null);
        NetworkServer.Spawn(buildingInstance, connectionToClient);
    }

    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        if (NetworkServer.active) return;
        Unit.AuthorityOnUnitSpawned += AuthorityHandlerUnitSpawn;
        Unit.AuthorityOnUnitDespawned += AuthorityHandlerUnitDespawn;
        Building.AuthorithyOnBuildingSpawned += AuthorityHandlerBuildingSpawn;
        Building.AuthorithyOnBuildingDespawned += AutorityHandlerBuildingDespawn;

    }
    public override void OnStopClient()
    {
        base.OnStopClient();
        if (!isClientOnly || !hasAuthority) return;
        Unit.AuthorityOnUnitSpawned -= AuthorityHandlerUnitSpawn;
        Unit.AuthorityOnUnitDespawned -= AuthorityHandlerUnitDespawn;
        Building.AuthorithyOnBuildingSpawned -= AuthorityHandlerBuildingSpawn;
        Building.AuthorithyOnBuildingDespawned -= AutorityHandlerBuildingDespawn;
    }

    private void AutorityHandlerBuildingDespawn(Building building)
    {
        myBuildings.Remove(building);
    }

    private void AuthorityHandlerBuildingSpawn(Building building)
    {
        myBuildings.Add(building);
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
