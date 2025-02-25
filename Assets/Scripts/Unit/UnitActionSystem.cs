using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public class UnitActionSystem : MonoBehaviour
{
   public static UnitActionSystem Instance { get; private set; }
   
   public event EventHandler OnSelectedUnitChanged;
   public event EventHandler OnSelectedActionChanged;
   public event EventHandler OnActionStarted;
   
   [FormerlySerializedAs("selectUnit")] [SerializeField] private Unit selectedUnit;
   [SerializeField] private LayerMask unitLayerMask;

   private BaseAction _selectedAction;
   private bool _isBusy;

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
      SetSelectedUnit(selectedUnit);
   }

   private void Update()
   {
      if (_isBusy)
      {
         return;
      }
      
      if(!TurnSystem.Instance.IsPlayerTurn())
      {
         return;
      }

      if (EventSystem.current.IsPointerOverGameObject())
      {
         return;
      }
      
      if (TryHandleUnitSelection()) return;  //选中人物后，才处理移动

      HandleSelectedAction();
   }

   private void HandleSelectedAction()
   {
      if (Input.GetMouseButtonDown(0))
      {
         GridPosition mouseGridPosition = LevelGrid.Instance.GetGridPosition(MouseWorld.GetPosition());

         if (!_selectedAction.IsValidMoveGridPosition(mouseGridPosition))
         {
            return;
         }

         if (!selectedUnit.TrySpendActionPointToTakeAction(_selectedAction))
         {
            return;
         }

         SetBusy();
         _selectedAction.TakeAction(mouseGridPosition, ClearBusy);
         
         OnActionStarted?.Invoke(this,EventArgs.Empty);
      }
   }

   private void SetBusy()
   {
      _isBusy = true;
   }

   private void ClearBusy()
   {
      _isBusy = false;
   }

   private bool TryHandleUnitSelection()
   {
      if (Input.GetMouseButtonDown(0))
      {
         Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
         if (Physics.Raycast(ray, out RaycastHit raycastHit, float.MaxValue, unitLayerMask)) //raycast人物
         {
            if (raycastHit.transform.TryGetComponent(out Unit unit)) //获取人物
            {
               // if(unit == selectedUnit)
               // {
               //    return false;
               // }

               if (unit.IsEnemy()) //敌人无法选中
               {
                  // TurnSystemUI.Instance.UpdateEndTurnButtonVisibility();
                  // // 更新UI：
                  // TurnSystemUI.Instance.UpdateEndTurnUnitActionSystemUIVisibility();

                  return false;
               }
               
               SetSelectedUnit(unit);
               return true;
            }
         }
      }

      return false;
   }

   private void SetSelectedUnit(Unit unit)
   {
      selectedUnit = unit;
      SetSelectedAction(unit.GetMoveAction());
      
      OnSelectedUnitChanged?.Invoke(this,EventArgs.Empty);
   }

   public void SetSelectedAction(BaseAction baseAction)
   {
      _selectedAction = baseAction;
      
      OnSelectedActionChanged?.Invoke(this,EventArgs.Empty);
   }

   public Unit GetSelectedUnit()
   {
      return selectedUnit;
   }
   
   public BaseAction GetSelectedAction()
   {
      return _selectedAction;
   }
}
