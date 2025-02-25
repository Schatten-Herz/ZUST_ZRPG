using System;
using System.Collections.Generic;
using System.Net.NetworkInformation;
using UnityEngine;

public class SpinAction : BaseAction
{
    private float _totalSpinAmount;
    
    [SerializeField] private Sprite spinSprite;
    
    public void Update()
    {
        if (!IsActive)
        {
            return;
        }
        
        float spinAddAmount = 360f * Time.deltaTime;
        transform.eulerAngles += new Vector3(0 ,spinAddAmount, 0);

        _totalSpinAmount += spinAddAmount;
        if (_totalSpinAmount >= 360f)
        {
            ActionComplete();
        }
    }

    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        ActionStart(onActionComplete);
        _totalSpinAmount = 0f;
    }

    // public override string GetActionName()
    // {
    //     return "Spin";
    // }
    
    public override Sprite GetActionSprite()
    {
        return spinSprite;
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = Unit.GetGridPosition();

        return new List<GridPosition>
        {
            unitGridPosition
        };
    }
    
    public override int GetActionPointsCost()
    {
        return 1;
    }

    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        return new EnemyAIAction
        {
            GridPosition = gridPosition,
            ActionValue = 0,
        };
    }
    
    public override string GetActionDescription()
    {
        return "旋转";
    }
}
