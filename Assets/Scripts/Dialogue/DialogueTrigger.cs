using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class DialogueTrigger : MonoBehaviour
{
    [SerializeField] private List<dialogueString> dialogueStrings = new List<dialogueString>();
    [SerializeField] private Transform npcTransform;
    [SerializeField] private BoxCollider triggerCollider; // 手动绑定Cube触发器
    
    private Outline _outline;
    private DialogueManager _dialogueManager;
    private int _currentStartIndex = 0;   // 控制对话起始索引
    [SerializeField] private int _nextStartIndex = 0;
    private bool _isDialogueActive = false; // 防止重复触发
    
    private void Awake()
    {
        // 自动查找并绑定触发器（如果忘记手动绑定）
        if (triggerCollider == null)
        {
            triggerCollider = GetComponent<BoxCollider>();
            if (triggerCollider != null)
            {
                triggerCollider.isTrigger = true; // 确保是触发器
            }
            else
            {
                Debug.LogError("未找到 BoxCollider 触发器！请挂载到带有 BoxCollider 的物体上。");
            }
        }
        
        // 获取挂载在角色上的 Outline 组件，并确保初始为禁用状态
        _outline = GetComponent<Outline>();
        if (_outline != null)
            _outline.enabled = false;
        
        // 获取 DialogueManager 并订阅事件
        _dialogueManager = FindObjectOfType<DialogueManager>();
        if (_dialogueManager != null)
        {
            _dialogueManager.onDialogueEnd.AddListener(OnDialogueEnd);
        }
        else
        {
            Debug.LogWarning("DialogueManager 未找到，请确保场景中存在该组件！");
        }
    }
    
    // 对话结束时触发
    private void OnDialogueEnd()
    {
        _isDialogueActive = false;

        // 根据需求设置下一次对话的起始索引
        _currentStartIndex = _nextStartIndex;
    }

    // 鼠标悬停时启用 Outline
    private void OnMouseEnter()
    {
        if (_outline != null)
            _outline.enabled = true;
    }

    // 鼠标离开时禁用 Outline
    private void OnMouseExit()
    {
        if (_outline != null)
            _outline.enabled = false;
    }


    [Obsolete("Obsolete")]
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player") && !_isDialogueActive)
        {
            _isDialogueActive = true;
            _dialogueManager.DialogueStart(dialogueStrings, npcTransform, _currentStartIndex);
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (triggerCollider != null)
        {
            Gizmos.color = Color.cyan;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(triggerCollider.center, triggerCollider.size);
        }
    }
    
}

[System.Serializable]
public class dialogueString
{
    public string text;
    public bool isEnd;
    
    [Header("Branch")]
    public bool isQuestion;
    public string answerOption1;
    public string answerOption2;
    public int option1IndexJump;
    public int option2IndexJump;
    
    public int nextIndexJump;
    
    [Header("Triggered Events")]
    public UnityEvent startDialogueEvent;
    public UnityEvent endDialogueEvent;
}