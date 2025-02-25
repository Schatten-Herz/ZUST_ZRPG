using System;
using UnityEngine;

public class UnitSelectVisual : MonoBehaviour
{
    [SerializeField] private Unit unit;

    private MeshRenderer _meshRenderer;
    // private Outline _outline; // 引用 Outline 组件

    private void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        // _outline = gameObject.AddComponent<Outline>(); // 添加 Outline 组件
        // _outline.OutlineColor = Color.yellow; // 设置轮廓颜色
        // _outline.OutlineWidth = 5f; // 设置轮廓宽度
        // _outline.enabled = false; // 初始状态禁用 Outline
    }

    private void Start()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += UnitActionSystemOnSelectedUnitChanged;
        UpdateVisual();
    }

    private void UnitActionSystemOnSelectedUnitChanged(object sender, EventArgs empty)
    {
        UpdateVisual();
    }

    private void UpdateVisual()
    {
        bool isSelected = UnitActionSystem.Instance.GetSelectedUnit() == unit;
        _meshRenderer.enabled = isSelected;
        // _outline.enabled = isSelected; // 根据选中状态启用或禁用 Outline
    }

    private void OnDestroy()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged -= UnitActionSystemOnSelectedUnitChanged;
    }
}