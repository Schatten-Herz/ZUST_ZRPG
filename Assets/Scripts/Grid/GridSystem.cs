using UnityEngine;
using System;

public class GridSystem<TGridObject> 
{
    private int _width;
    private int _height;
    private float _cellSize;
    private TGridObject[,] _gridObjectArray;
    
    public GridSystem( int width, int height , float cellSize, Func<GridPosition, GridSystem<TGridObject>, TGridObject> createGridObject)
    {
        this._height = height;
        this._width = width;
        this._cellSize = cellSize;

        _gridObjectArray = new TGridObject[width, height];
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                _gridObjectArray[x,z] = createGridObject(gridPosition, this);
            }
        }
    }

    public Vector3 GetWorldPosition(GridPosition gridPosition)
    {
        return new Vector3(gridPosition.x, 0, gridPosition.z) * _cellSize;
    }

    public GridPosition GetGridPosition(Vector3 worldPosition)
    {
        return new GridPosition(
            Mathf.RoundToInt(worldPosition.x / _cellSize),
            Mathf.RoundToInt(worldPosition.z / _cellSize)
            );
    }

    public void CreateDebugObject(Transform debugPrefab)
    {
        for (int x = 0; x < _width; x++)
        {
            for (int z = 0; z < _height; z++)
            {
                GridPosition gridPosition = new GridPosition(x, z);
                Transform debugTransform = GameObject.Instantiate(debugPrefab, GetWorldPosition(gridPosition), Quaternion.identity);
                GridDebugObject gridDebugObject = debugTransform.GetComponent<GridDebugObject>();
                gridDebugObject.SetGridObject(GetGridObject(gridPosition));
            }
        }
    }

    public TGridObject GetGridObject(GridPosition gridPosition)
    {
        return _gridObjectArray[gridPosition.x, gridPosition.z];
    }
    
    public bool IsValidateGridPosition(GridPosition gridPosition)
    {
        return gridPosition.x >= 0 && gridPosition.x < _width && gridPosition.z >= 0 && gridPosition.z < _height;
    }
    
    public int GetWidth()
    {
        return _width;
    }
    
    public int GetHeight()
    {
        return _height;
    }
    
}
