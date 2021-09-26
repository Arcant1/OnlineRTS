using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;
using System;
using UnityEngine.SceneManagement;

public class LobbyMenu : MonoBehaviour
{
    [SerializeField] private GameObject lobbyUI = null;
    private void OnEnable()
    {
        RTSNetworkManager.ClientOnConnected += HandleClientConnected;
        RTSNetworkManager.ClientOnDisconnected += HandleClientDisconnected;
    }
    private void OnDisable()
    {
        RTSNetworkManager.ClientOnConnected -= HandleClientConnected;
        RTSNetworkManager.ClientOnDisconnected -= HandleClientDisconnected;
    }

    private void HandleClientDisconnected()
    {
        throw new NotImplementedException();
    }

    private void HandleClientConnected()
    {
        lobbyUI.SetActive(true);
    }

    public void LeaveLobby()
    {
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            NetworkManager.singleton.StopHost();
        }
        else
        {
            NetworkManager.singleton.StopClient();
            SceneManager.LoadScene(0);
        }
    }

}
