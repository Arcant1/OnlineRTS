using Mirror;
using TMPro;
using UnityEngine;

public class ResourcesDisplay : MonoBehaviour
{
    [SerializeField] private TMP_Text resourcesText = null;
    private RTSPlayer player;
    private void Awake()
    {
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
        ClientHandleResourcesUpdated(player.GetResources());
    }
    private void OnEnable()
    {
        player.ClientOnResourcesChanged += ClientHandleResourcesUpdated;
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
