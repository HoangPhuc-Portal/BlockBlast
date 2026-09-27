using UnityEngine;

namespace BlockBlast.Visuals
{
    /// <summary>
    /// Điều khiển khối 3D phía trên đĩa tròn xoay 360 độ liên tục
    /// </summary>
    public class RotatingPlate : MonoBehaviour
    {
        [Header("360 Rotation Settings")]
        [SerializeField] private Vector3 rotationAxis = Vector3.up;
        [SerializeField] private float rotationSpeed = 36f; // Tốc độ quay 36 độ/giây (hoàn thành 360 độ trong 10 giây)

        private void Update()
        {
            transform.Rotate(rotationAxis, rotationSpeed * Time.deltaTime, Space.Self);
        }
    }
}