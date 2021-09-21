using System;
using System.Collections;
using System.Collections.Generic;
using Mirror;
using UnityEngine;

public class Building : NetworkBehaviour
{

    [SerializeField] private Sprite icon = null;
    [SerializeField] private int price = 100;
    [SerializeField] private int id = -1;

    public static event Action<Building> ServerOnBuildingSpawned;
    public static event Action<Building> ServerOnBuildingDespawned;

    public static event Action<Building> AuthorithyOnBuildingSpawned;
    public static event Action<Building> AuthorithyOnBuildingDespawned;

    public Sprite GetIcon() => icon;
    public int GetPrice() => price;
    public int GetId() => id;

    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();
        ServerOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        ServerOnBuildingDespawned?.Invoke(this);
    }
    #endregion

    #region Client
    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        AuthorithyOnBuildingSpawned?.Invoke(this);
    }

    public override void OnStopAuthority()
    {
        base.OnStopAuthority();
        if (!hasAuthority) return;
        AuthorithyOnBuildingDespawned?.Invoke(this);
    }
    #endregion
}
