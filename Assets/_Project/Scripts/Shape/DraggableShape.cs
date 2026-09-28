using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using BlockBlast.Data;
using BlockBlast.Grid;

namespace BlockBlast.Shape
{
    /// <summary>
    /// Xử lý sự kiện kéo thả khối gạch trên UI Canvas và tự động tạo mô hình con
    /// </summary>
    public class DraggableShape : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
    {
        [Header("Settings")]
        [SerializeField] private float dragOffsetYScreen = 100f;
        [SerializeField] private float scaleOnDrag = 1.2f;

        [Header("Prefab for UI Tiles")]
        [SerializeField] private GameObject blockTilePrefab;

        public ShapeData ShapeData { get; private set; }

        private Vector3 originalPosition;
        private Vector3 originalScale;
        private Canvas parentCanvas;

        public void Setup(ShapeData data)
        {
            ShapeData = data;
            originalPosition = transform.localPosition;
            originalScale = transform.localScale;
            parentCanvas = GetComponentInParent<Canvas>();

            BuildVisualShape();
        }

        private void BuildVisualShape()
        {
            // Xóa hết các tile cũ nếu có
            foreach (Transform child in transform)
            {
                Destroy(child.gameObject);
            }

            bool[,] matrix = ShapeData.GetMatrix();
            float tileSize = 30f; // Kích thước ô trong khay

            for (int r = 0; r < ShapeData.rows; r++)
            {
                for (int c = 0; c < ShapeData.columns; c++)
                {
                    if (matrix[r, c])
                    {
                        GameObject tile = Instantiate(blockTilePrefab, transform);
                        RectTransform rt = tile.GetComponent<RectTransform>();

                        // Tính toán vị trí tương đối
                        rt.anchoredPosition = new Vector2(c * tileSize, r * tileSize);

                        Image img = tile.GetComponent<Image>();
                        if (img != null)
                        {
                            img.color = ShapeData.blockColor;
                        }
                    }
                }
            }
        }

        // Chiếu điểm màn hình xuống đúng mặt phẳng của bàn cờ (thay vì ScreenToWorldPoint(z=0),
        // vốn chỉ đúng với camera Orthographic; camera trong scene này là Perspective nên
        // cách cũ luôn trả về sai tọa độ X/Y).
        private Vector3 ScreenPointToBoardWorldPosition(Vector2 screenPos)
        {
            Ray ray = Camera.main.ScreenPointToRay(screenPos);
            Plane boardPlane = new Plane(Vector3.forward, new Vector3(0, 0, GridBoard.Instance.BoardZ));

            if (boardPlane.Raycast(ray, out float distance))
            {
                return ray.GetPoint(distance);
            }

            // Trường hợp hy hữu tia không cắt mặt phẳng: trả về vị trí hiện tại của khối
            return transform.position;
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            transform.localScale = originalScale * scaleOnDrag;
        }

        public void OnDrag(PointerEventData eventData)
        {
            Vector2 mousePos = eventData.position + new Vector2(0, dragOffsetYScreen);
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                parentCanvas.transform as RectTransform,
                mousePos,
                parentCanvas.worldCamera,
                out Vector2 localPoint
            );

            transform.localPosition = localPoint;

            Vector3 worldPos = ScreenPointToBoardWorldPosition(mousePos);

            GridBoard.Instance.UpdateDragPreview(ShapeData, worldPos);
        }

        public void OnPointerUp(PointerEventData eventData)
        {
            transform.localScale = originalScale;

            Vector2 mousePos = eventData.position + new Vector2(0, dragOffsetYScreen);
            Vector3 worldPos = ScreenPointToBoardWorldPosition(mousePos);

            bool success = GridBoard.Instance.TryPlaceShape(ShapeData, worldPos);

            if (success)
            {
                TrayManager.Instance.OnShapePlaced(this);
                Destroy(gameObject);
            }
            else
            {
                transform.localPosition = originalPosition;
                GridBoard.Instance.ClearPreviewAndHighlights();
            }
        }
    }
}