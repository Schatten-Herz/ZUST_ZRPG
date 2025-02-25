using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GridSystemVisual : MonoBehaviour
{
    public static GridSystemVisual Instance { get; private set; }
    
    [Serializable]
    public struct GridVisualTypeMaterial
    {
        public GridVisualType gridVisualType;
        public Material material;
    }
    public enum GridVisualType
    {
        White,
        Red,
        RedSoft,
        Blue,
        Yellow,
        Green,
        GreenSoft,
        
    }
    [SerializeField] private List<GridVisualTypeMaterial> gridVisualTypeMaterialList;
    
    [SerializeField] private Transform gridSystemVisualSinglePrefab;
    
    private GridSystemVisualSingle[,] _gridSystemVisualSingleArray;
    
    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("有不止一个ActionSystem" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }
      
        Instance = this;
    }

    private void Start()
    {
        _gridSystemVisualSingleArray = new GridSystemVisualSingle[LevelGrid.Instance.GetWidth(), LevelGrid.Instance.GetHeight()];
        
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                Transform gridSystemVisualSingleTransform = Instantiate(gridSystemVisualSinglePrefab, LevelGrid.Instance.GetWorldPosition(new GridPosition(x, z)), Quaternion.identity);
                
                _gridSystemVisualSingleArray[x, z] = gridSystemVisualSingleTransform.GetComponent<GridSystemVisualSingle>();
            }
        }
        
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystemOnSelectedActionChanged;
        LevelGrid.Instance.OnAnyUnitMoveGridPosition += LevelGridOnAnyUnitMoveGridPosition;
        
        UpdateGridVisual();
    }

    public void HideAllGridPosition()
    {
        for (int x = 0; x < LevelGrid.Instance.GetWidth(); x++)
        {
            for (int z = 0; z < LevelGrid.Instance.GetHeight(); z++)
            {
                _gridSystemVisualSingleArray[x, z].Hide();
            }
        }
    }

    private void ShowGridPositionRange(GridPosition gridPosition, int range, GridVisualType gridVisualType)
    {
        List<GridPosition> gridPositionList = new List<GridPosition>();
        for (int x = -range; x <= range; x++)
        {
            for (int z = -range; z <= range; z++)
            {
                GridPosition testGridPosition = gridPosition + new GridPosition(x, z);
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if (testDistance > range)
                {
                    continue;
                }

                if (LevelGrid.Instance.IsValidateGridPosition(testGridPosition))
                {
                    gridPositionList.Add(testGridPosition);
                }
            }
        }
        
        ShowGridPositionList(gridPositionList, gridVisualType);
    }

    public void ShowGridPositionList(List<GridPosition> gridPositionList , GridVisualType gridVisualType)
    {
        foreach (GridPosition gridPosition in gridPositionList)
        {
            _gridSystemVisualSingleArray[gridPosition.x, gridPosition.z].Show(GetGridVisualTypeMaterial(gridVisualType));
        }
    }

    private void UpdateGridVisual()
    {
        HideAllGridPosition();
        
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        BaseAction selectedAction = UnitActionSystem.Instance.GetSelectedAction();

        GridVisualType gridVisualType;

        switch (selectedAction)
        {
            default:
            case MoveAction moveAction:
                gridVisualType = GridVisualType.White;
                break;
            case SpinAction spinAction:
                gridVisualType = GridVisualType.Blue;
                break;
            case ShootAction shootAction:
                Debug.Log("ShootAction branch triggered.");
                gridVisualType = GridVisualType.Red;
                
                ShowGridPositionRange(selectedUnit.GetGridPosition(),shootAction.GetMaxShootDistance(),GridVisualType.RedSoft);
                break;
            case HealthAction healthAction:
                Debug.Log("HealthAction branch triggered.");
                gridVisualType = GridVisualType.Green;
                
                ShowGridPositionRange(selectedUnit.GetGridPosition(),healthAction.GetMaxShootDistance(),GridVisualType.GreenSoft);
                break;
        }
        ShowGridPositionList(selectedAction.GetValidActionGridPositionList(),gridVisualType);
    }
    
    private void UnitActionSystemOnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }
    
    private void LevelGridOnAnyUnitMoveGridPosition(object sender, EventArgs e)
    {
        UpdateGridVisual();
    }

    private Material GetGridVisualTypeMaterial(GridVisualType gridVisualType)
    {
        foreach (GridVisualTypeMaterial gridVisualTypeMaterial in gridVisualTypeMaterialList)
        {
            if (gridVisualTypeMaterial.gridVisualType == gridVisualType)
            {
                return gridVisualTypeMaterial.material;
            }
        }

        Debug.LogError("找不到这个类型的材质" + gridVisualType);
        return null;
    }
}
