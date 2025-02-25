using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAction : MonoBehaviour
{
    protected Unit Unit;
    protected bool IsActive;
    protected Action OnActionComplete;
    
    [SerializeField] private Sprite defaultSprite; // 作为默认图片（可选）
    

    public virtual Sprite GetActionSprite()
    {
        return defaultSprite;
    }

    protected virtual void Awake()
    {
        Unit = GetComponent<Unit>();
    }

    // public abstract string GetActionName();
    
    public abstract void TakeAction(GridPosition gridPosition,Action onActionComplete);

    public virtual bool IsValidMoveGridPosition(GridPosition gridPosition)
    {
        List<GridPosition> validGridPositionList = GetValidActionGridPositionList();
        return validGridPositionList.Contains(gridPosition);
    }

    public abstract List<GridPosition> GetValidActionGridPositionList();

    public virtual int GetActionPointsCost()
    {
        return 1;
    }
    
    protected void ActionStart(Action onActionComplete)
    {
        IsActive = true;
        this.OnActionComplete = onActionComplete;
    }
    
    protected void ActionComplete()
    {
        IsActive = false;
        OnActionComplete();
    }

    public EnemyAIAction GetBestEnemyAIAction()
    {
        List<EnemyAIAction> enemyAIActionList = new List<EnemyAIAction>();

        List<GridPosition> validActionGridPositionList = GetValidActionGridPositionList();

        foreach (GridPosition gridPosition in validActionGridPositionList)
        {
            EnemyAIAction enemyAIAction = GetEnemyAIAction(gridPosition);
            enemyAIActionList.Add(enemyAIAction);
        }

        if (enemyAIActionList.Count > 0)
        {
            enemyAIActionList.Sort((a, b) => b.ActionValue - a.ActionValue);
            return enemyAIActionList[0];
        }
        else
        {
            //没有合适的行动
            return null;
        }
    }

    public abstract EnemyAIAction GetEnemyAIAction(GridPosition gridPosition);
    
    public virtual string GetActionDescription()
    {
        return "Default action description"; // 在具体Action中重写这个方法
    }
}
