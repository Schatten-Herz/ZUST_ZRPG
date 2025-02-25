using System;
using System.Collections.Generic;
using UnityEngine;

public class ShootAction : BaseAction
{
    public static event EventHandler<OnShootEventArgs> OnAnyShoot;
    
    public event EventHandler<OnShootEventArgs> OnShoot;
    
    public class OnShootEventArgs : EventArgs
    {
        public Unit TargetUnit;
        public Unit ShootingUnit;
    }
    
    private enum State
    {
        Aiming,
        Shooting,
        Cooloff,
    }
    
    [SerializeField] private LayerMask obstaclesLayerMask;

    private State _state;
    private readonly int _maxShootDistance = 7;
    private float _stateTimer;
    private Unit _targetUnit;
    private bool _canShootBullet;
    
    [SerializeField] private Sprite shootSprite;
    
    public void Update()
    {
        if (!IsActive)
        {
            return;
        }

        _stateTimer -= Time.deltaTime;
        switch (_state)
        {
            case State.Aiming:
                Vector3 aimDirection = (_targetUnit.GetWorldPosition() - Unit.GetWorldPosition()).normalized;
                
                float rotateSpeed = 10f;
                transform.forward = Vector3.Lerp(transform.forward,aimDirection,Time.deltaTime * rotateSpeed);
                break;
            case State.Shooting:
                if (_canShootBullet)
                {
                    Shoot();
                    _canShootBullet = false;
                }
                break;
            case State.Cooloff:
               break;
        }
        
        if (_stateTimer <= 0)
        {
            NextState();
        }
    }

    private void NextState()
    {
        switch (_state)
        {
            case State.Aiming:
                _state = State.Shooting;
                float shootStateTime = 0.5f;
                _stateTimer = shootStateTime;
                break;
            case State.Shooting:
                _state = State.Cooloff;
                float coolOffStateTime = 0.5f;
                _stateTimer = coolOffStateTime;
                break;
            case State.Cooloff:
                ActionComplete();
                break;
        }
    }

    private void Shoot()
    {
        OnAnyShoot?.Invoke(this, new OnShootEventArgs()
        {
            TargetUnit = _targetUnit,
            ShootingUnit = Unit,
        });
        
        OnShoot?.Invoke(this, new OnShootEventArgs()
        {
            TargetUnit = _targetUnit,
            ShootingUnit = Unit,
        });
        _targetUnit.Damage(40);
    }
    
    public override void TakeAction(GridPosition gridPosition, Action onActionComplete)
    {
        ActionStart(onActionComplete);

        _targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        
        _state = State.Aiming;
        float aimingOffStateTime = 0.5f;
        _stateTimer = aimingOffStateTime;
        
        _canShootBullet = true;
    }

    public override List<GridPosition> GetValidActionGridPositionList()
    {
        GridPosition unitGridPosition = Unit.GetGridPosition();
        return GetValidActionGridPositionList(unitGridPosition);
    }

    public List<GridPosition> GetValidActionGridPositionList(GridPosition unitGridPosition)
    {
        List<GridPosition> validGridPositionList = new List<GridPosition>();

        for (int x = -_maxShootDistance; x <= _maxShootDistance; x++)
        {
            for (int z = -_maxShootDistance; z <= _maxShootDistance; z++)
            {
                GridPosition offsetGridPosition = new GridPosition(x, z);
                GridPosition testGridPosition = unitGridPosition + offsetGridPosition;

                if (!LevelGrid.Instance.IsValidateGridPosition(testGridPosition))
                {
                    continue;
                }
                
                int testDistance = Mathf.Abs(x) + Mathf.Abs(z);
                if(testDistance > _maxShootDistance)
                {
                    continue;
                }
                
                // validMoveGridPositionList.Add(testGridPosition);
                // continue;

                if (!LevelGrid.Instance.HasUnitAtGridPosition(testGridPosition))
                {
                    //格子上没有unit
                    continue;
                }
                Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(testGridPosition);
                if (targetUnit.IsEnemy() == Unit.IsEnemy())
                {
                    continue;
                }

                Vector3 unitWorldPosition = LevelGrid.Instance.GetWorldPosition(unitGridPosition);
                Vector3 shootDir = (targetUnit.GetWorldPosition() - unitWorldPosition).normalized;
                float unitShoulderHeight = 1.7f;
                
                if (Physics.Raycast(
                        unitWorldPosition + Vector3.up * unitShoulderHeight,
                        shootDir,
                        Vector3.Distance(unitWorldPosition, targetUnit.GetWorldPosition()),
                        obstaclesLayerMask))
                {
                    //有障碍物
                    continue;
                }

                validGridPositionList.Add(testGridPosition);
            }
        }
        
        return validGridPositionList;
    }

    public override Sprite GetActionSprite()
    {
        return shootSprite;
    }
    
    public override int GetActionPointsCost()
    {
        return 2;
    }
    
    public int GetMaxShootDistance()
    {
        return _maxShootDistance;
    }
    
    public override EnemyAIAction GetEnemyAIAction(GridPosition gridPosition)
    {
        Unit targetUnit = LevelGrid.Instance.GetUnitAtGridPosition(gridPosition);
        
        return new EnemyAIAction
        {
            GridPosition = gridPosition,
            ActionValue = 100 + Mathf.RoundToInt((1-targetUnit.GetHealthNormalized()) * 100f),
        };
    }

    public int GetTargetCountAtPosition(GridPosition gridPosition)
    {
        return GetValidActionGridPositionList(gridPosition).Count;
    }
    
    public override string GetActionDescription()
    {
        return "射击";
    }
}
