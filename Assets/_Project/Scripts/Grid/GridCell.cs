using UnityEngine;

namespace BlockBlast.Grid
{
    /// <summary>
    /// Quản lý dữ liệu và hiệu ứng hiển thị của một ô cờ trên bàn 8x8
    /// </summary>
    public class GridCell : MonoBehaviour
    {
        [Header("Renderers")]
        [SerializeField] private Renderer cellBackgroundRenderer;
        [SerializeField] private GameObject fillVisual;
        [SerializeField] private Renderer fillRenderer;
        [SerializeField] private GameObject highlightGlowVisual;

        public int Row { get; private set; }
        public int Col { get; private set; }
        public bool IsOccupied { get; private set; }
        public Color CurrentColor { get; private set; }

        private readonly Color defaultEmptyColor = new Color(0.92f, 0.85f, 0.76f); // Màu kem/be chuẩn

        public void Init(int row, int col)
        {
            Row = row;
            Col = col;
            IsOccupied = false;
            SetStateNormal();
        }

        public void SetOccupied(bool occupied, Color color = default)
        {
            IsOccupied = occupied;
            CurrentColor = color;

            if (IsOccupied)
            {
                fillVisual.SetActive(true);
                fillRenderer.material.color = color;
                if (highlightGlowVisual != null) highlightGlowVisual.SetActive(false);
            }
            else
            {
                SetStateNormal();
            }
        }

        public void SetStateNormal()
        {
            if (IsOccupied) return;

            fillVisual.SetActive(false);
            if (highlightGlowVisual != null) highlightGlowVisual.SetActive(false);
            cellBackgroundRenderer.material.color = defaultEmptyColor;
        }

        // Hiển thị bóng xem trước vị trí đặt khối
        public void SetStatePreview(Color color)
        {
            if (IsOccupied) return;

            fillVisual.SetActive(true);
            Color previewColor = color;
            previewColor.a = 0.5f; // Làm mờ 50%
            fillRenderer.material.color = previewColor;
        }

        // Bật ánh hào quang phát sáng theo thời gian thực (Real-time Line Highlight)
        public void SetStateHighlightLine()
        {
            if (highlightGlowVisual != null)
            {
                highlightGlowVisual.SetActive(true);
            }
        }
    }
}