using System.Collections.Generic;
using UnityEngine;

public class GridObject
{

    private GridPosition _gridPosition;
    private GridSystem<GridObject> _gridSystem;
    private List<Unit> _unitList;

    public GridObject(GridPosition gridPosition, GridSystem<GridObject> gridSystem)
    {
        this._gridPosition = gridPosition;
        this._gridSystem = gridSystem;
        _unitList = new List<Unit>();
    }

    public override string ToString()
    {
        string unitString = "";
        foreach (Unit unit in _unitList)
        {
            unitString += unit + "\n";
        }
        return _gridPosition.ToString() + "\n" + unitString;
    }

    public void AddUnit(Unit unit)
    {
        _unitList.Add(unit);
    }

    public void RemoveUnit(Unit unit)
    {
        _unitList.Remove(unit);
    }

    public List<Unit> GetUnitList()
    {
        return _unitList;
    }
    
    public bool HasAnyUnit()
    {
        return _unitList.Count > 0;
    }

    public Unit GetUnit()
    {
        if (HasAnyUnit())
        {
            return _unitList[0];
        }
        else
        {
            return null;
        }
    }
    
}
