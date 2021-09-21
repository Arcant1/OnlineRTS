using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Targeter targeter = null;
    [SerializeField] private float chaseRange = 10f;
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
        agent.ResetPath();
    }

    [ServerCallback]
    public void Update()
    {
        Targetable currentTarget = targeter.GetTarget();
        if (currentTarget != null)
        {
            if (Vector3.SqrMagnitude(currentTarget.transform.position - transform.position) > chaseRange * chaseRange)
            {
                agent.SetDestination(currentTarget.transform.position);
            }
            else if (agent.hasPath)
            {
                agent.ResetPath();
            }
            return;
        }
        if (!agent.hasPath) return;
        if (agent.remainingDistance > agent.stoppingDistance) return;
        agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 targetPosition)
    {
        targeter.ClearTarget();
        if (!NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 1f, NavMesh.AllAreas)) return;
        agent.SetDestination(hit.position);
    }
    #endregion
}
