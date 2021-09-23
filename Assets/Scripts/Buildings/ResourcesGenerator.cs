using Mirror;
using UnityEngine;

public class ResourcesGenerator : NetworkBehaviour
{
    [SerializeField] private Health health = null;
    [SerializeField] private int resourcesPerInverval = 10;
    [SerializeField] private float interval = 2f;

    private float timer;

    private RTSPlayer player;

    public override void OnStartServer()
    {
        base.OnStartServer();
        timer = interval;
        player = connectionToClient.identity.GetComponent<RTSPlayer>();
        health.ServerOnDie += ServerHandleDie;
        GameOverHandler.ServerOnGameOver += ServerGameOver;
    }

    public override void OnStopServer()
    {
        base.OnStopServer();
        health.ServerOnDie -= ServerHandleDie;
        GameOverHandler.ServerOnGameOver -= ServerGameOver;
    }

    [ServerCallback]
    private void Update()
    {
        timer -= Time.deltaTime;
        if (timer < 0f)
        {
            timer += interval;
            player.SetResources(player.GetResources() + resourcesPerInverval);
        }
    }


    private void ServerGameOver()
    {
        enabled = false;
    }

    private void ServerHandleDie()
    {
        NetworkServer.Destroy(gameObject);
    }
}
