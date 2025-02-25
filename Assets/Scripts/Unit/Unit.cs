using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Unit : MonoBehaviour
{
    private const int ActionPointsMax = 3;

    public static event EventHandler OnAnyActionPointsChanged;
    public static event EventHandler OnAnyUnitSpawned;
    public static event EventHandler OnAnyUnitDead;


    [SerializeField] private bool isEnemy;
    
    private GridPosition _gridPosition;
    private HealthSystem _healthSystem;
    private MoveAction _moveAction;
    private SpinAction _spinAction;
    private ShootAction _shootAction;
    private BaseAction[] _baseActionArray;

    private int _actionPoint = ActionPointsMax;

    private void Awake()
    {
        _healthSystem = GetComponent<HealthSystem>();
        _moveAction = GetComponent<MoveAction>();
        _spinAction = GetComponent<SpinAction>();
        _shootAction = GetComponent<ShootAction>();
        _baseActionArray = GetComponents<BaseAction>();
    }

    private void Start()
    {
        _gridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        LevelGrid.Instance.AddUnitAtGridPosition(_gridPosition, this);

        TurnSystem.Instance.OnTurnChanged += TurnSystemOnTurnChanged;
        
        _healthSystem.OnDead += HealthSystemOnDead;
        
        OnAnyUnitSpawned?.Invoke(this, EventArgs.Empty);
    }

    private void Update()
    {
        GridPosition newGridPosition = LevelGrid.Instance.GetGridPosition(transform.position);
        if (newGridPosition != _gridPosition)
        {
            GridPosition oldGridPosition = _gridPosition;
            
            _gridPosition = newGridPosition;
            //
            LevelGrid.Instance.UnitMoveGridPosition(this, oldGridPosition, newGridPosition);
            
        }
    }

    public MoveAction GetMoveAction()
    {
        return _moveAction;
    }

    public SpinAction GetSpinAction()
    {
        return _spinAction;
    }
    
    public ShootAction GetShootAction()
    {
        return _shootAction;
    }

    public GridPosition GetGridPosition()
    {
        return _gridPosition;
    }
    
    public Vector3 GetWorldPosition()
    {
        return transform.position;
    }

    public BaseAction[] GetBaseActionArray()
    {
        return _baseActionArray;
    }

    public bool TrySpendActionPointToTakeAction(BaseAction baseAction)
    {
        if (CanSpendActionPointToTakeAction(baseAction))
        {
            SpendActionPoint(baseAction.GetActionPointsCost());
            return true;
        }
        else
        {
            return false;
        }
    }
    
    public bool CanSpendActionPointToTakeAction(BaseAction baseAction)
    {
        if (_actionPoint >= baseAction.GetActionPointsCost())
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    private void SpendActionPoint(int amount) //消耗行动点
    {
        _actionPoint -= amount;
        
        OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
    }
    
    public int GetActionPoints() //获取行动点
    {
        return _actionPoint;
    }

    private void TurnSystemOnTurnChanged(object sender, EventArgs e)
    {
        if ((IsEnemy() && !TurnSystem.Instance.IsPlayerTurn()) || (!IsEnemy() && TurnSystem.Instance.IsPlayerTurn()))
        {
            _actionPoint = ActionPointsMax;
        
            OnAnyActionPointsChanged?.Invoke(this, EventArgs.Empty);
        }
    }
    
    public bool IsEnemy()
    {
        return isEnemy;
    }
    
    public void Damage(int damageAmount)
    {
        _healthSystem.Damage(damageAmount);
    }

    private void HealthSystemOnDead(object sender, EventArgs e)
    {
        LevelGrid.Instance.RemoveUnitAtGridPosition(_gridPosition, this);
        
        Destroy(gameObject);
        
        OnAnyUnitDead?.Invoke(this, EventArgs.Empty);
    }
    
    public float GetHealthNormalized() //获取生命值
    {
        return _healthSystem.GetHealthNormalized();
    }
}
