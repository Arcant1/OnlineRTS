using Mirror;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    private Targetable target;
    public Targetable GetTarget() => target;
    #region Server

    public override void OnStartServer()
    {
        base.OnStartServer();
        GameOverHandler.ServerOnGameOver += ServerHandleGameOver;
    }
    public override void OnStopServer()
    {
        base.OnStopServer();
        GameOverHandler.ServerOnGameOver -= ServerHandleGameOver;
    }

    private void ServerHandleGameOver()
    {
        ClearTarget();
    }

    [Command]
    public void CmdSetTarget(GameObject targetGameObject)
    {
        if (!targetGameObject.TryGetComponent<Targetable>(out Targetable newTarget)) return;
        target = newTarget;
    }
    [Server]
    public void ClearTarget()
    {
        target = null;
    }

    #endregion

    #region Client

    #endregion
}
