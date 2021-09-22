using Mirror;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text resourcesText = null;
    private RTSPlayer player;



    private void Update()
    {
        if (player == null)
        {
            player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();

            if (player != null)
            {
                ClientHandleResourcesUpdated(player.GetResources());
                player.ClientOnResourcesChanged += ClientHandleResourcesUpdated;
            }
        }
    }

    private void OnDisable()
    {
        player.ClientOnResourcesChanged -= ClientHandleResourcesUpdated;
    }
    private void ClientHandleResourcesUpdated(int resources)
    {
        resourcesText.SetText($"Resources: {resources}");
    }
}
