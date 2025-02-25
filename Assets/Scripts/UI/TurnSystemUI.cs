using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class TurnSystemUI : MonoBehaviour
{
    // public static TurnSystemUI Instance { get; private set; }

    [SerializeField] private Button endTurnButton;
    [SerializeField] private TextMeshProUGUI turnNumberText;
    [SerializeField] private GameObject unitActionSystemUI;

    [SerializeField] private GameObject enemyTurnVisualGameObject;
    
    // private void Awake()
    // {
    //     // 单例赋值（确保场景中只有一个 TurnSystemUI 实例）
    //     if (Instance != null && Instance != this)
    //     {
    //         Destroy(gameObject);
    //         return;
    //     }
    //     Instance = this;
    // }

    private void Start()
    {
        endTurnButton.onClick.AddListener(() =>
        {
            TurnSystem.Instance.NextTurn();
        });

        TurnSystem.Instance.OnTurnChanged += TurnSystemOnTurnChanged;
        
        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
        UpdateEndTurnUnitActionSystemUIVisibility();
    }

    private void TurnSystemOnTurnChanged(object sender ,EventArgs e)
    {
        UpdateTurnText();
        UpdateEnemyTurnVisual();
        UpdateEndTurnButtonVisibility();
        UpdateEndTurnUnitActionSystemUIVisibility();
    }

    private void UpdateTurnText()
    {
        turnNumberText.text = "Turn " + TurnSystem.Instance.GetTurnNumber();
    }
    
    private void UpdateEnemyTurnVisual()
    {
        enemyTurnVisualGameObject.SetActive(!TurnSystem.Instance.IsPlayerTurn());
    }
    
    public void UpdateEndTurnButtonVisibility()
    {
        endTurnButton.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }

    public void UpdateEndTurnUnitActionSystemUIVisibility()
    {
        unitActionSystemUI.gameObject.SetActive(TurnSystem.Instance.IsPlayerTurn());
    }
}
