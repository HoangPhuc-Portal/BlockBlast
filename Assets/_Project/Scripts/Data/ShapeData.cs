//using UnityEngine;

//namespace BlockBlast.Data
//{
//    /// <summary>
//    /// ScriptableObject định nghĩa hình dạng và màu sắc của khối gạch
//    /// </summary>
//    [CreateAssetMenu(fileName = "NewShapeData", menuName = "Block Blast/Shape Data")]
//    public class ShapeData : ScriptableObject
//    {
//        [Header("Dimensions")]
//        public int rows = 3;
//        public int columns = 3;

//        [Header("Matrix Data (Flattened 1D Array)")]
//        [SerializeField] private bool[] matrixData;

//        [Header("Visuals")]
//        public Color blockColor = Color.cyan;

//        /// <summary>
//        /// Chuyển mảng 1D lưu trong Inspector thành mảng 2D cho Game Logic
//        /// </summary>
//        public bool[,] GetMatrix()
//        {
//            bool[,] matrix = new bool[rows, columns];
//            for (int r = 0; r < rows; r++)
//            {
//                for (int c = 0; c < columns; c++)
//                {
//                    int index = r * columns + c;
//                    matrix[r, c] = (index < matrixData.Length) && matrixData[index];
//                }
//            }
//            return matrix;
//        }

//        private void OnValidate()
//        {
//            if (matrixData == null || matrixData.Length != rows * columns)
//            {
//                matrixData = new bool[rows * columns];
//            }
//        }
//    }
//}

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

        // Cache lại để không cấp phát mảng mới mỗi lần gọi GetMatrix() (hàm này được gọi
        // liên tục mỗi frame khi đang kéo khối, gây rác bộ nhớ/GC không cần thiết).
        private bool[,] cachedMatrix;

        /// <summary>
        /// Chuyển mảng 1D lưu trong Inspector thành mảng 2D cho Game Logic
        /// </summary>
        public bool[,] GetMatrix()
        {
            if (cachedMatrix == null)
            {
                cachedMatrix = new bool[rows, columns];
                for (int r = 0; r < rows; r++)
                {
                    for (int c = 0; c < columns; c++)
                    {
                        int index = r * columns + c;
                        cachedMatrix[r, c] = (index < matrixData.Length) && matrixData[index];
                    }
                }
            }
            return cachedMatrix;
        }

        private void OnValidate()
        {
            if (matrixData == null || matrixData.Length != rows * columns)
            {
                matrixData = new bool[rows * columns];
            }
            // Dữ liệu có thể đã đổi trong Inspector -> buộc tính lại ở lần gọi GetMatrix() tiếp theo
            cachedMatrix = null;
        }

        private void OnEnable()
        {
            cachedMatrix = null;
        }
    }
}