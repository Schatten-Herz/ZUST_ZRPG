using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UnitActionSystemUI : MonoBehaviour
{
    [SerializeField] private Transform actionButtonPrefab;
    [SerializeField] private Transform actionButtonContainerTransform;
    [SerializeField] TextMeshProUGUI actionPointsText;
    
    private List<ActionButtonUI> _actionButtonUIList;

    private void Awake()
    {
        _actionButtonUIList = new List<ActionButtonUI>();
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystemOnSelectedUnitChanged;
        UnitActionSystem.Instance.OnSelectedActionChanged += UnitActionSystemOnSelectedActionChanged;
        UnitActionSystem.Instance.OnActionStarted += UnitActionSystemOnActionStarted;
        TurnSystem.Instance.OnTurnChanged += TurnSystemOnTurnChanged;
        Unit.OnAnyActionPointsChanged += UnitOnAnyActionPointsChanged;
        
        UpdateActionPoints();
        CreateUnitActionButtons();
        UpdateSelectedVisual();
    }
    
    private void CreateUnitActionButtons()
    {
        foreach (Transform child in actionButtonContainerTransform)
        {
            Destroy(child.gameObject);
        }
        
        _actionButtonUIList.Clear();
        
        Unit selectUnit = UnitActionSystem.Instance.GetSelectedUnit();

        foreach (BaseAction baseAction in selectUnit.GetBaseActionArray())
        {
            Transform actionButtonTransform = Instantiate(actionButtonPrefab, actionButtonContainerTransform);
            ActionButtonUI actionButtonUI = actionButtonTransform.GetComponent<ActionButtonUI>();
            actionButtonUI.SetBaseAction(baseAction);
            
            _actionButtonUIList.Add(actionButtonUI);
            
        }
    }
    
    private void UnitActionSystemOnSelectedUnitChanged(object sender, EventArgs e)
    {
        CreateUnitActionButtons();
        UpdateSelectedVisual();
        UpdateActionPoints();
    }
    
    private void UnitActionSystemOnSelectedActionChanged(object sender, EventArgs e)
    {
        UpdateSelectedVisual();
    }
    
    private void UnitActionSystemOnActionStarted(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }

    private void UpdateSelectedVisual()
    {
        foreach (ActionButtonUI actionButtonUI in _actionButtonUIList)
        {
           actionButtonUI.UpdateSelectedVisual();
        }
    }

    private void UpdateActionPoints()  
    {
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        int remainingActionPoints = selectedUnit.GetActionPoints();
    
        // 通过 Linq 生成一个包含多次“•”的列表
        IEnumerable<string> bullets = Enumerable.Repeat("•", remainingActionPoints);
    
        // 将列表中的元素用空格拼接起来，例如： "• • •"
        string bulletString = string.Join(" ", bullets);

        actionPointsText.text = bulletString;
    }

    private void TurnSystemOnTurnChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }
    
    private void UnitOnAnyActionPointsChanged(object sender, EventArgs e)
    {
        UpdateActionPoints();
    }
}
