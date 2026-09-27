using System.Collections.Generic;
using UnityEngine;
using BlockBlast.Core;
using BlockBlast.Data;
using BlockBlast.Grid;

namespace BlockBlast.Shape
{
    /// <summary>
    /// Quản lý khay chứa 3 khối gạch bên dưới màn hình và kiểm tra điều kiện thua game
    /// </summary>
    public class TrayManager : MonoBehaviour
    {
        public static TrayManager Instance { get; private set; }

        [Header("References")]
        [SerializeField] private Transform[] slots = new Transform[3];
        [SerializeField] private DraggableShape shapePrefab;
        [SerializeField] private List<ShapeData> availableShapes;

        private DraggableShape[] currentShapes = new DraggableShape[3];

        private void Awake()
        {
            if (Instance == null) Instance = this;
            else Destroy(gameObject);
        }

        private void Start()
        {
            SpawnNewHand();
        }

        public void SpawnNewHand()
        {
            for (int i = 0; i < 3; i++)
            {
                if (slots[i].childCount > 0) continue;

                ShapeData randomData = availableShapes[Random.Range(0, availableShapes.Count)];
                DraggableShape newShape = Instantiate(shapePrefab, slots[i]);
                newShape.Setup(randomData);
                currentShapes[i] = newShape;
            }

            CheckGameOverCondition();
        }

        public void OnShapePlaced(DraggableShape shape)
        {
            for (int i = 0; i < 3; i++)
            {
                if (currentShapes[i] == shape)
                {
                    currentShapes[i] = null;
                    break;
                }
            }

            bool isEmpty = true;
            foreach (var s in currentShapes)
            {
                if (s != null) { isEmpty = false; break; }
            }

            if (isEmpty)
            {
                SpawnNewHand();
            }
            else
            {
                CheckGameOverCondition();
            }
        }

        private void CheckGameOverCondition()
        {
            bool canPlaceAny = false;

            for (int i = 0; i < 3; i++)
            {
                if (currentShapes[i] != null)
                {
                    if (GridBoard.Instance.CanPlaceShapeAnywhere(currentShapes[i].ShapeData))
                    {
                        canPlaceAny = true;
                        break;
                    }
                }
            }

            if (!canPlaceAny)
            {
                GameEvents.TriggerGameOver();
            }
        }
    }
}