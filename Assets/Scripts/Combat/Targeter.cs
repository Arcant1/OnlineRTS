using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Targeter : NetworkBehaviour
{
    [SerializeField] private Targetable target;
    #region Server
    [Command]
    void CmdSetTarget(GameObject targetGameObject)
    {
        if (targetGameObject.TryGetComponent<Targetable>(out Targetable netTarget)) return;
        this.target = netTarget;
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
