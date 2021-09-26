using Mirror;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class RTSNetworkManager : NetworkManager
{
    [SerializeField] private GameObject unitBasePrefab = null;
    [SerializeField] private GameOverHandler gameOverHandlerPrefab = null;

    private bool isGameInProgress = false;
    public List<RTSPlayer> Players { get; } = new List<RTSPlayer>();

    public static event Action ClientOnConnected;
    public static event Action ClientOnDisconnected;

    #region Server
    public override void OnServerAddPlayer(NetworkConnection conn)
    {
        base.OnServerAddPlayer(conn);

        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();

        Players.Add(player);

        player.SetDisplayName($"Player: {Players.Count}");

        player.SetTeamColor(new Color(
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f),
            UnityEngine.Random.Range(0f, 1f)
            ));

        player.SetPartyOwner(Players.Count == 1);
    }
    public override void OnServerConnect(NetworkConnection conn)
    {
        base.OnServerConnect(conn);
        if (!isGameInProgress) return;
        conn.Disconnect();
    }

    public override void OnServerDisconnect(NetworkConnection conn)
    {
        RTSPlayer player = conn.identity.GetComponent<RTSPlayer>();
        Players.Remove(player);
        base.OnServerDisconnect(conn);
    }

    public void StartGame()
    {
        if (Players.Count < 2) return;
        isGameInProgress = true;
        ServerChangeScene("Scene_Map_1");
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        Players.Clear();
        isGameInProgress = false;
    }
    public override void OnServerSceneChanged(string sceneName)
    {
        if (SceneManager.GetActiveScene().name.StartsWith("Scene_Map"))
        {
            GameOverHandler gameOverHandler =
                Instantiate<GameOverHandler>(gameOverHandlerPrefab);
            NetworkServer.Spawn(gameOverHandler.gameObject);
            foreach (RTSPlayer player in Players)
            {
                var baseInstance = Instantiate(
                    unitBasePrefab,
                    GetStartPosition().position,
                    Quaternion.identity
                    );
                NetworkServer.Spawn(baseInstance, player.connectionToClient);

            }
        }
    }
    #endregion

    #region Client
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        ClientOnConnected?.Invoke();
    }

    public override void OnClientDisconnect(NetworkConnection conn)
    {
        base.OnClientDisconnect(conn);
        ClientOnDisconnected?.Invoke();
    }

    public override void OnStopClient()
    {
        base.OnStopClient();
        Players.Clear();
    }

    #endregion


}
