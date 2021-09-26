using Mirror;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class BuildingButton : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    [SerializeField] private Building building = null;
    [SerializeField] private Image iconImage = null;
    [SerializeField] private TMP_Text priceText = null;
    [SerializeField] private LayerMask floorMask = new LayerMask();

    private Camera mainCamera;
    private RTSPlayer player;
    private GameObject buildingPreviewInstance;
    private Renderer buildingRendererInstance;
    private BoxCollider buildingCollider;

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button != PointerEventData.InputButton.Left) return;
        if (player.GetResources() < building.GetPrice()) return;
        buildingPreviewInstance = Instantiate(building.GetBuildingPreview());
        buildingRendererInstance = buildingPreviewInstance.GetComponentInChildren<Renderer>();
        buildingPreviewInstance.SetActive(true);
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (buildingPreviewInstance == null) return;
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask))
        {
            // ask for placing building
            player.CmdTryPlaceBuilding(building.GetId(), hit.point);
        }
        Destroy(buildingPreviewInstance);
    }
    private void UpdateBuildingPreview()
    {
        //print(buildingPreviewInstance == null);
        Ray ray = mainCamera.ScreenPointToRay(Mouse.current.position.ReadValue());
        if (!Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, floorMask)) return;
        buildingPreviewInstance.transform.position = hit.point;
        if (!buildingPreviewInstance.activeSelf)
        {
            buildingPreviewInstance.SetActive(true);
        }
        Color color = player.CanPlaceBuilding(buildingCollider, hit.point) ? Color.green : Color.red;
        foreach (Material material in buildingRendererInstance.materials)
        {
            material.SetColor("_BaseColor", color);
        }
    }
    private void Awake()
    {
        mainCamera = Camera.main;
        player = NetworkClient.connection.identity.GetComponent<RTSPlayer>();
    }
    private void Start()
    {
        if (building == null) return;
        buildingCollider = building.GetComponent<BoxCollider>();
        priceText.text = building.GetPrice().ToString();
        iconImage.sprite = building.GetIcon();
    }
    private void Update()
    {
        if (buildingPreviewInstance == null) return;
        UpdateBuildingPreview();
    }
}
