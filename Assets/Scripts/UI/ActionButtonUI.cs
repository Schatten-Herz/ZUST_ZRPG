using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
using UnityEngine.UI;

public class ActionButtonUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{

    [SerializeField] private TextMeshProUGUI textMeshPro;
    [SerializeField] private Button button;
    [SerializeField] private GameObject selectedGameObject;
    [SerializeField] private Image backgroundImage;
    
    [SerializeField] private GameObject tooltipPanel;
    [SerializeField] private TextMeshProUGUI tooltipName;

    
    private BaseAction _baseAction;
    
    private void Update()
    {
        if (tooltipPanel != null && tooltipPanel.activeSelf)
        {
            // 更新 tooltip 的位置，使其始终在鼠标附近
            tooltipPanel.transform.position = Input.mousePosition + new Vector3(20, -20, 0);
        }
    }

    public void SetBaseAction(BaseAction baseAction)
    {
        _baseAction = baseAction;
        // textMeshPro.text = baseAction.GetActionName().ToUpper();
        
        backgroundImage.sprite = baseAction.GetActionSprite();
        
        button.onClick.AddListener(() =>
        {
            UnitActionSystem.Instance.SetSelectedAction(baseAction);
        });
    }

    public void UpdateSelectedVisual()
    {
        BaseAction selectedBaseAction = UnitActionSystem.Instance.GetSelectedAction();
        selectedGameObject.SetActive(selectedBaseAction == _baseAction);
    }
    
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (_baseAction != null)
            ShowTooltip();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        HideTooltip();
    }

    private void ShowTooltip()
    {
        if (tooltipPanel != null && tooltipName != null)
        {
            tooltipName.text = _baseAction.GetActionDescription();
            tooltipPanel.SetActive(true);
            
            // 可选：更新tooltip位置到当前鼠标位置
            tooltipPanel.transform.position = Input.mousePosition + new Vector3(20, -20, 0);
        }
    }

    private void HideTooltip()
    {
        if (tooltipPanel != null)
        {
            tooltipPanel.SetActive(false);
        }
    }
    
}
