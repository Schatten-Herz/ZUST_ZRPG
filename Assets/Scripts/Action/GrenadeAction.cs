using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

public class GrenadeAction : BaseAction
{
    [SerializeField] private Sprite grenadeSprite;
    [SerializeField] private Transform grenadeProjectilePrefab;
    
    private int _maxThrowDistance = 7;
    
    private void Update()
    {
        if(!IsActive)
        {
            return;
        }
        
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        Transform grenadeProjectileTransform = Instantiate(grenadeProjectilePrefab, Unit.GetWorldPosition(), Quaternion.identity);
        GrenadeProjectile grenadeProjectile = grenadeProjectileTransform.GetComponent<GrenadeProjectile>();
        grenadeProjectile.Setup(gridPosition,ActionComplete);
        
        ActionStart(onActionComplete);
    }

    private void OnGrenadeBehaviourComplete()
    {
        ActionComplete();
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        GridPosition unitGridPosition = Unit.GetGridPosition();
        for (int x = -_maxThrowDistance; x <= _maxThrowDistance; x++)
        {
            for (int z = -_maxThrowDistance; z <= _maxThrowDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidateGridPosition(testGridPosition))
                {
                    continue;
                }
                
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if(testDistance > _maxThrowDistance)
                {
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }
        
        return validGridPositionList;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction()
        {
            GridPosition = gridPosition,
            ActionValue = 0,
        };
    }
    
    public override Sprite GetActionSprite()
    {
        return grenadeSprite;
    }
    
    public override string GetActionDescription()
    {
        return "手榴弹"; 
    }
    
    
}
