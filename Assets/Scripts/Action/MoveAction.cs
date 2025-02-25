using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class MoveAction : BaseAction
{
    private List<Vector3> _positionList;
    private int _currentPositionIndex;

    public event EventHandler OnStartMoving;
    public event EventHandler OnStopMoving;
    
    [SerializeField] private int maxMoveDistance = 6; //最大移动距离
    
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    
    [SerializeField] private Sprite moveSprite;

    private void Update()
    {
        if (!IsActive)
        {
            return;
        }
        
        Vector3 targetPosition = _positionList[_currentPositionIndex];
        float stoppingDistance = 0.1f;
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        
        
        float rotateSpeed = Time.deltaTime * 20;
        transform.forward = Vector3.Lerp(transform.forward,moveDirection,rotateSpeed); //移动时旋转朝向
        
        if (Vector3.Distance(transform.position, targetPosition) > stoppingDistance) //到达目标距离停止
        {
            float moveSpeed = 4f;
            transform.position += moveDirection * (Time.deltaTime * moveSpeed);
        }
        else
        { 
            _currentPositionIndex++;
            if (_currentPositionIndex >= _positionList.Count)
            {
                OnStopMoving?.Invoke(this,EventArgs.Empty);
                ActionComplete();
            }
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        List<GridPosition> pathGridPositionList = Pathfinding.Instance.FindPath(Unit.GetGridPosition(), gridPosition, out int pathLength);
        
        _currentPositionIndex = 0;
        _positionList = new List<Vector3>();
        
        foreach(GridPosition pathGridPosition in pathGridPositionList)
        {
            _positionList.Add(LevelGrid.Instance.GetWorldPosition(pathGridPosition));
        }
        
        OnStartMoving?.Invoke(this,EventArgs.Empty);
        ActionStart(onActionComplete);
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validMoveGridPositionList = new List<GridPosition>();
        
        GridPosition unitGridPosition = Unit.GetGridPosition();

        for (int x = -maxMoveDistance ; x <= maxMoveDistance; x++)
        {
            for (int z = -maxMoveDistance ; z <= maxMoveDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidateGridPosition(testGridPosition))
                {
                    continue;
                }

                if (unitGridPosition == testGridPosition)
                {
                    //跳过当前位置
                    continue;
                }

                if (LevelGrid.Instance.HasUnitAtGridPosition(testGridPosition))
                {
                    continue;
                }
                
                if(!Pathfinding.Instance.IsWalkableGridPosition(testGridPosition))
                {
                    continue;
                }
                
                if(!Pathfinding.Instance.HasPath(unitGridPosition,testGridPosition))
                {
                    continue;
                }

                int pathfindingDistanceMultiplier = 10;
                if(Pathfinding.Instance.GetPathLength(unitGridPosition,testGridPosition) > maxMoveDistance * pathfindingDistanceMultiplier)
                {
                    //超出最大移动距离
                    continue;
                }
                
                validMoveGridPositionList.Add(testGridPosition);
                //Debug.Log(testGridPosition);
            }
        }

        return validMoveGridPositionList;
    }

    // public override string GetActionName()
    // {
    //     return "Move";
    // }
    
    public override Sprite GetActionSprite()
    {
        return moveSprite;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        int targetCountAtGridPosition = Unit.GetShootAction().GetTargetCountAtPosition(gridPosition);
        return new EnemyAIAction
        {
            GridPosition = gridPosition,
            ActionValue = targetCountAtGridPosition * 10,
        };
    }
    
    public override string GetActionDescription()
    {
        return "移动";
    }
}
