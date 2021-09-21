using UnityEngine;
using UnityEngine.InputSystem;

public class UnitCommandGiver : MonoBehaviour
{
    [SerializeField] private UnitSelectionHandler unitSelectionHandler = null;
    private Camera mainCamera = null;
    [SerializeField] private LayerMask layerMask;
    private void Awake()
    {
        mainCamera = Camera.main;
    }
    private void Update()
    {
        if (Mouse.current.rightButton.isPressed)
        {
            Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
            if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, layerMask)) return;
            if (hit.collider.TryGetComponent<Targetable>(out Targetable target))
            {
                if (target.hasAuthority)
                {
                    TryMove(hit.point); return;
                }
                TryTarget(target);
                return;
            }
            TryMove(hit.point);
        }
    }

    private void OnEnable()
    {
        GameOverHandler.ClientOnGameOver += HandleClientGameOver;
    }

    private void OnDisable()
    {
        GameOverHandler.ClientOnGameOver -= HandleClientGameOver;
    }

    private void HandleClientGameOver(string winnerName)
    {
        enabled = false;
    }

    private void TryTarget(Targetable target)
    {
        foreach (Unit unit in unitSelectionHandler.SelectedUnits)
        {
            unit.GetTargeter().CmdSetTarget(target.gameObject);
        }
    }


    private void TryMove(Vector3 point)
    {
        foreach (Unit unit in unitSelectionHandler.SelectedUnits)
        {
            unit.GetUnitMovement().CmdMove(point);
        }
    }

}
