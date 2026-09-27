using System.Collections.Generic;
using UnityEngine;
using BlockBlast.Core;
using BlockBlast.Data;

namespace BlockBlast.Grid
{
    /// <summary>
    /// Quản lý logic bàn chơi 8x8, kiểm tra vị trí đặt khối và nhận diện xóa hàng/cột real-time
    /// </summary>
    public class GridBoard : MonoBehaviour
    {
        public static GridBoard Instance { get; private set; }

        public const int GRID_SIZE = 8;

        [Header("Board Settings")]
        [SerializeField] private GridCell cellPrefab;
        [SerializeField] private Transform boardParent;
        [SerializeField] private float cellSize = 1.1f;

        private GridCell[,] grid = new GridCell[GRID_SIZE, GRID_SIZE];
        private List<GridCell> currentPreviewCells = new List<GridCell>();
        private List<GridCell> currentHighlightedLineCells = new List<GridCell>();

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);

            GenerateBoard();
        }

        private void Start()
        {
            //GenerateBoard();
        }

        private void GenerateBoard()
        {
            Vector3 origin = transform.position - new Vector3((GRID_SIZE - 1) * cellSize / 2f, (GRID_SIZE - 1) * cellSize / 2f, 0);

            for (int r = 0; r < GRID_SIZE; r++)
            {
                for (int c = 0; c < GRID_SIZE; c++)
                {
                    Vector3 cellPos = origin + new Vector3(c * cellSize, r * cellSize, 0);
                    GridCell cell = Instantiate(cellPrefab, cellPos, Quaternion.identity, boardParent);
                    cell.Init(r, c);
                    grid[r, c] = cell;
                }
            }
        }

        // ----------------------------------------------------------------------
        // CƠ CHẾ XEM TRƯỚC VÀ PHÁT SÁNG HÀNG/CỘT THỜI GIAN THỰC (REAL-TIME PREVIEW)
        // ----------------------------------------------------------------------
        public void UpdateDragPreview(ShapeData shapeData, Vector3 worldPosition)
        {
            ClearPreviewAndHighlights();

            if (!GetGridCoordinatesFromWorld(worldPosition, out int startRow, out int startCol))
                return;

            bool[,] matrix = shapeData.GetMatrix();

            if (!CanPlaceShape(matrix, startRow, startCol))
                return;

            // 1. Hiển thị bóng xem trước vị trí đặt khối
            List<Vector2Int> shapeFootprint = new List<Vector2Int>();
            for (int r = 0; r < shapeData.rows; r++)
            {
                for (int c = 0; c < shapeData.columns; c++)
                {
                    if (matrix[r, c])
                    {
                        int targetR = startRow + r;
                        int targetC = startCol + c;
                        grid[targetR, targetC].SetStatePreview(shapeData.blockColor);
                        currentPreviewCells.Add(grid[targetR, targetC]);
                        shapeFootprint.Add(new Vector2Int(targetR, targetC));
                    }
                }
            }

            // 2. Giả lập và Nhận diện ngay lập tức các Hàng/Cột sẽ bị xóa
            SimulateAndHighlightLines(shapeFootprint);
        }

        private void SimulateAndHighlightLines(List<Vector2Int> footprint)
        {
            bool[,] virtualBoard = new bool[GRID_SIZE, GRID_SIZE];
            for (int r = 0; r < GRID_SIZE; r++)
            {
                for (int c = 0; c < GRID_SIZE; c++)
                {
                    virtualBoard[r, c] = grid[r, c].IsOccupied;
                }
            }

            foreach (var pos in footprint)
            {
                virtualBoard[pos.x, pos.y] = true;
            }

            List<int> fullRows = new List<int>();
            List<int> fullCols = new List<int>();

            for (int r = 0; r < GRID_SIZE; r++)
            {
                bool isFull = true;
                for (int c = 0; c < GRID_SIZE; c++)
                {
                    if (!virtualBoard[r, c]) { isFull = false; break; }
                }
                if (isFull) fullRows.Add(r);
            }

            for (int c = 0; c < GRID_SIZE; c++)
            {
                bool isFull = true;
                for (int r = 0; r < GRID_SIZE; r++)
                {
                    if (!virtualBoard[r, c]) { isFull = false; break; }
                }
                if (isFull) fullCols.Add(c);
            }

            HashSet<GridCell> cellsToHighlight = new HashSet<GridCell>();
            foreach (int r in fullRows)
            {
                for (int c = 0; c < GRID_SIZE; c++) cellsToHighlight.Add(grid[r, c]);
            }
            foreach (int c in fullCols)
            {
                for (int r = 0; r < GRID_SIZE; r++) cellsToHighlight.Add(grid[r, c]);
            }

            foreach (var cell in cellsToHighlight)
            {
                cell.SetStateHighlightLine();
                currentHighlightedLineCells.Add(cell);
            }
        }

        public void ClearPreviewAndHighlights()
        {
            foreach (var cell in currentPreviewCells) cell.SetStateNormal();
            foreach (var cell in currentHighlightedLineCells) cell.SetStateNormal();
            currentPreviewCells.Clear();
            currentHighlightedLineCells.Clear();
        }

        // ----------------------------------------------------------------------
        // ĐẶT KHỐI CHÍNH THỨC VÀ TÍNH ĐIỂM
        // ----------------------------------------------------------------------
        public bool TryPlaceShape(ShapeData shapeData, Vector3 worldPosition)
        {
            ClearPreviewAndHighlights();

            if (!GetGridCoordinatesFromWorld(worldPosition, out int startRow, out int startCol))
                return false;

            bool[,] matrix = shapeData.GetMatrix();
            if (!CanPlaceShape(matrix, startRow, startCol))
                return false;

            for (int r = 0; r < shapeData.rows; r++)
            {
                for (int c = 0; c < shapeData.columns; c++)
                {
                    if (matrix[r, c])
                    {
                        grid[startRow + r, startCol + c].SetOccupied(true, shapeData.blockColor);
                    }
                }
            }

            GameEvents.TriggerShapePlaced();
            ProcessCompletedLines();
            return true;
        }

        private void ProcessCompletedLines()
        {
            List<int> rowsToClear = new List<int>();
            List<int> colsToClear = new List<int>();

            for (int r = 0; r < GRID_SIZE; r++)
            {
                bool full = true;
                for (int c = 0; c < GRID_SIZE; c++) if (!grid[r, c].IsOccupied) full = false;
                if (full) rowsToClear.Add(r);
            }

            for (int c = 0; c < GRID_SIZE; c++)
            {
                bool full = true;
                for (int r = 0; r < GRID_SIZE; r++) if (!grid[r, c].IsOccupied) full = false;
                if (full) colsToClear.Add(c);
            }

            int totalCleared = rowsToClear.Count + colsToClear.Count;
            if (totalCleared > 0)
            {
                HashSet<GridCell> targetCells = new HashSet<GridCell>();
                foreach (int r in rowsToClear) for (int c = 0; c < GRID_SIZE; c++) targetCells.Add(grid[r, c]);
                foreach (int c in colsToClear) for (int r = 0; r < GRID_SIZE; r++) targetCells.Add(grid[r, c]);

                foreach (var cell in targetCells)
                {
                    cell.SetOccupied(false);
                }

                GameEvents.TriggerLinesCleared(totalCleared);
            }
        }

        public bool CanPlaceShapeAnywhere(ShapeData shapeData)
        {
            // Phòng ngừa trường hợp shapeData chưa được gán vào TrayManager
            if (shapeData == null)
            {
                Debug.Log("ShapeData is null. Cannot check placement.");
                return false;
            }
            bool[,] matrix = shapeData.GetMatrix();
            for (int r = 0; r < GRID_SIZE; r++)
            {
                for (int c = 0; c < GRID_SIZE; c++)
                {
                    if (CanPlaceShape(matrix, r, c)) return true;
                }
            }
            return false;
        }

        private bool CanPlaceShape(bool[,] matrix, int startRow, int startCol)
        {
            int rows = matrix.GetLength(0);
            int cols = matrix.GetLength(1);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    if (matrix[r, c])
                    {
                        int targetR = startRow + r;
                        int targetC = startCol + c;
                        if (targetR < 0 || targetR >= GRID_SIZE || targetC < 0 || targetC >= GRID_SIZE) return false;
                        if (grid[targetR, targetC] == null || grid[targetR, targetC].IsOccupied) return false;
                    }
                }
            }
            return true;
        }

        private bool GetGridCoordinatesFromWorld(Vector3 worldPos, out int row, out int col)
        {
            Vector3 origin = transform.position - new Vector3((GRID_SIZE - 1) * cellSize / 2f, (GRID_SIZE - 1) * cellSize / 2f, 0);
            Vector3 relPos = worldPos - origin;

            col = Mathf.RoundToInt(relPos.x / cellSize);
            row = Mathf.RoundToInt(relPos.y / cellSize);

            if (row >= 0 && row < GRID_SIZE && col >= 0 && col < GRID_SIZE) return true;
            return false;
        }
    }
}