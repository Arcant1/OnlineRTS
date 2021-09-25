using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;

public class RTSPlayer : NetworkBehaviour
{
    [SerializeField] private Building[] availableBuildings = new Building[0];
    [SerializeField] private LayerMask buildingBlockLayer = new LayerMask();
    [SerializeField] private float buildingRangeLimit = 5f;
    [SyncVar(hook = nameof(ClientHandleResourcesUpdated))]
    private int resources = 500;
    private Color teamColor = new Color();

    public Color GetTeamColor() => teamColor;

    public event Action<int> ClientOnResourcesChanged;
    private List<Unit> myUnits = new List<Unit>();
    private List<Building> myBuildings = new List<Building>();

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
    public override void OnStopServer()
    {
        base.OnStopServer();
        Unit.ServerOnUnitSpawned -= ServerHandlerUnitSpawn;
        Unit.ServerOnUnitDespawned -= ServerHandlerUnitDespawn;
        Building.ServerOnBuildingSpawned -= ServerHandlerBuildingSpawn;
        Building.ServerOnBuildingDespawned -= ServerHandlerBuildingDespawn;
    }
    [Server]
    public void SetResources(int newResources)
    {
        resources = newResources;
    }
    [Server]
    public void SetTeamColor(Color newTeamColor)
    {
        teamColor = newTeamColor;
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
        // Check if exists
        foreach (Building building in availableBuildings)
        {
            if (building.GetId() == buildingId)
            {
                buildingToPlace = building;
                break;
            }
        }
        if (!buildingToPlace) return;
        // Check if have enough resources
        if (resources < buildingToPlace.GetPrice()) return;
        // Check if not collides with another thing defined in the layermask
        BoxCollider buildingCollider = buildingToPlace.GetComponent<BoxCollider>();
        if (!CanPlaceBuilding(buildingCollider, position)) return;
        // Instatiate the actual building
        GameObject buildingInstance = Instantiate(buildingToPlace.gameObject, position, buildingToPlace.transform.rotation);
        NetworkServer.Spawn(buildingInstance, connectionToClient);
        // Pay
        SetResources(GetResources() - buildingToPlace.GetPrice());
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
    private void ClientHandleResourcesUpdated(int oldValue, int newValue)
    {
        ClientOnResourcesChanged?.Invoke(newValue);
    }
    #endregion
    public bool CanPlaceBuilding(BoxCollider buildingCollider, Vector3 position)
    {
        // Check if not collides with another thing defined in the layermask
        if (Physics.CheckBox(
            position + buildingCollider.center,
            buildingCollider.size / 2,
            Quaternion.identity,
            buildingBlockLayer))
            return false;

        // Check if there is no other building inside its range
        foreach (Building building in myBuildings)
        {
            if ((position - building.transform.position).sqrMagnitude <= buildingRangeLimit * buildingRangeLimit)
            {
                return true;
            }
        }
        return false;
    }
}
