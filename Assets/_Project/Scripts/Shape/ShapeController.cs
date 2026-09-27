using UnityEngine;
using UnityEngine.EventSystems;

public class ShapeController : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private Vector3 originalPosition;
    private Transform boardTransform;
    private Camera mainCamera;

    private void Start()
    {
        mainCamera = Camera.main;
        originalPosition = transform.position;

        GameObject boardObj = GameObject.Find("GridBoard");
        if (boardObj != null) boardTransform = boardObj.transform;
    }

    public void OnBeginDrag(PointerEventData eventData) { }

    // 1. Dùng eventData.position thay vì Input.mousePosition
    public void OnDrag(PointerEventData eventData)
    {
        // Tạo Ray từ tọa độ vị trí chuột/tay bấm truyền vào từ eventData
        Ray ray = mainCamera.ScreenPointToRay(eventData.position);

        float boardY = boardTransform != null ? boardTransform.position.y : 0f;
        Plane boardPlane = new Plane(Vector3.up, new Vector3(0, boardY, 0));

        if (boardPlane.Raycast(ray, out float distance))
        {
            Vector3 worldPos = ray.GetPoint(distance);
            worldPos.y = boardY + 0.5f;
            transform.position = worldPos;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        // Truyền eventData sang hàm kiểm tra thả gạch
        bool placedSuccessfully = TryPlaceOnBoard(eventData);

        if (!placedSuccessfully)
        {
            transform.position = originalPosition;
        }
    }

    // 2. Truyền PointerEventData vào đây
    private bool TryPlaceOnBoard(PointerEventData eventData)
    {
        Ray ray = mainCamera.ScreenPointToRay(eventData.position);
        if (Physics.Raycast(ray, out RaycastHit hit))
        {
            if (hit.collider.CompareTag("GridCell")  )
            {
                return true;
            }
        }
        return false;
    }
}