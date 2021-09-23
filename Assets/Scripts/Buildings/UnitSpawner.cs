using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class UnitSpawner : NetworkBehaviour, IPointerClickHandler
{
    [SerializeField] private Health health = null;
    [SerializeField] private Unit unitPrefab = null;
    [SerializeField] private Transform unitSpawnPoint = null;
    [SerializeField] private TMP_Text remainingUnitsText = null;
    [SerializeField] private Image unitProgressImage = null;
    [SerializeField] private int maxUnitQueue = 5;
    [SerializeField] private float spawneMoveRange = 10f;
    [SerializeField] private float unitSpawnDuration = 3f;

    [SyncVar(hook = nameof(ClientHandleQueuedUnitsUpdated))]
    private int queuedUnits;
    [SyncVar]
    private float unitTimer;
    private RTSPlayer player;
    private float progressImageVelocity;

    private void Update()
    {
        if (isClient)
        {
            UpdateTimerDisplay();
        }

        if (isServer)
        {
            ProduceUnits();
        }
    }


    #region Server
    public override void OnStartServer()
    {
        base.OnStartServer();
        health.ServerOnDie += HandleServerDie;
    }
    public override void OnStopServer()
    {
        base.OnStopServer();
        health.ServerOnDie -= HandleServerDie;
    }
    [Server]
    private void ProduceUnits()
    {
        if (queuedUnits == 0) return;
        unitTimer += Time.deltaTime;
        if (unitTimer < unitSpawnDuration) return;

        GameObject unitInstance = Instantiate(
            unitPrefab.gameObject,
            unitSpawnPoint.position,
            unitSpawnPoint.rotation);

        NetworkServer.Spawn(unitInstance, connectionToClient);
        Vector2 circularOffset = UnityEngine.Random.insideUnitCircle;
        Vector3 spawnOffset = new Vector3 (circularOffset.x, unitSpawnPoint.position.y, circularOffset.y)* spawneMoveRange;
        UnitMovement unitMovement = unitInstance.GetComponent<UnitMovement>();
        unitMovement.ServerMove(unitSpawnPoint.position + spawnOffset);
        queuedUnits--;
        unitTimer = 0;
    }
    [Command]
    private void CmdSpawnUnit()
    {
        if (queuedUnits == maxUnitQueue) return;
        RTSPlayer player = connectionToClient.identity.GetComponent<RTSPlayer>();
        if (player.GetResources() < unitPrefab.GetResourceCost()) return;
        queuedUnits++;
        player.SetResources(player.GetResources() - unitPrefab.GetResourceCost());
    }


    [Server]
    private void HandleServerDie()
    {
        NetworkServer.Destroy(gameObject);
    }



    #endregion

    #region Client

    private void UpdateTimerDisplay()
    {
        float progress = unitTimer / unitSpawnDuration;
        if (progress < unitProgressImage.fillAmount)
        {
            unitProgressImage.fillAmount = progress;
        }
        else
        {
            unitProgressImage.fillAmount = Mathf.SmoothDamp(
                unitProgressImage.fillAmount,
                progress,
                ref progressImageVelocity,
                0.1f);
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (!hasAuthority) return;
        CmdSpawnUnit();
    }
    public void ClientHandleQueuedUnitsUpdated(int oldValue, int newValue)
    {
        remainingUnitsText.SetText(newValue.ToString());
    }
    #endregion
}
