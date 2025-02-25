using System;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class UnitWorldUI : MonoBehaviour
{
    [SerializeField] private Unit unit;
    [SerializeField] private Image healthBarImage;
    [SerializeField] private HealthSystem healthSystem;

    private void Start()
    {
        Unit.OnAnyActionPointsChanged += UnitOnAnyActionPointsChanged;
        healthSystem.OnDamaged += HealthSystemOnDamaged;
        
        UpdateHealthBar();
    }

    private void UnitOnAnyActionPointsChanged(object sender, EventArgs e)
    {
        
    }

    private void UpdateHealthBar()
    {
        healthBarImage.fillAmount = healthSystem.GetHealthNormalized();
    }

    private void HealthSystemOnDamaged(object sender, EventArgs e)
    {
        UpdateHealthBar();
    }
}
