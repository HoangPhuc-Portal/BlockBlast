using UnityEngine;

namespace BlockBlast.Data
{
    /// <summary>
    /// ScriptableObject định nghĩa hình dạng và màu sắc của khối gạch
    /// </summary>
    [CreateAssetMenu(fileName = "NewShapeData", menuName = "Block Blast/Shape Data")]
    public class ShapeData : ScriptableObject
    {
        [Header("Dimensions")]
        public int rows = 3;
        public int columns = 3;

        [Header("Matrix Data (Flattened 1D Array)")]
        [SerializeField] private bool[] matrixData;

        [Header("Visuals")]
        public Color blockColor = Color.cyan;

        /// <summary>
        /// Chuyển mảng 1D lưu trong Inspector thành mảng 2D cho Game Logic
        /// </summary>
        public bool[,] GetMatrix()
        {
            bool[,] matrix = new bool[rows, columns];
            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < columns; c++)
                {
                    int index = r * columns + c;
                    matrix[r, c] = (index < matrixData.Length) && matrixData[index];
                }
            }
            return matrix;
        }

        private void OnValidate()
        {
            if (matrixData == null || matrixData.Length != rows * columns)
            {
                matrixData = new bool[rows * columns];
            }
        }
    }
}