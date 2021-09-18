using Mirror;
using UnityEngine;
using UnityEngine.AI;

public class UnitMovement : NetworkBehaviour
{
    [SerializeField] NavMeshAgent agent;

    #region Server

    [ServerCallback]
    public void Update()
    {
        if (!agent.hasPath) return;
        if (agent.remainingDistance > agent.stoppingDistance) return;
        agent.ResetPath();
    }

    [Command]
    public void CmdMove(Vector3 targetPosition)
    {
        if (!NavMesh.SamplePosition(targetPosition, out NavMeshHit hit, 1f, NavMesh.AllAreas)) return;
        agent.SetDestination(hit.position);
    }
    #endregion



}
