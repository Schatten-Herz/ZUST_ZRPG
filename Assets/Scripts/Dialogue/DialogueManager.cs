using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using TMPro;
using UnityEngine.Serialization;

public class DialogueManager : MonoBehaviour
{
    [SerializeField] private GameObject dialogueParent;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField] private Button option1Button;
    [SerializeField] private Button option2Button;
    [SerializeField] private GameObject mouseWorld;
    
    [SerializeField] private float typeSpeed = 0.1f;
    [SerializeField] private float turnSpeed = 2f;

    private List<dialogueString> _dialogueList;
    
    [Header("Events")]
    public UnityEvent onDialogueEnd = new UnityEvent(); // 公开的对话结束事件
    public UnityEvent onDialogueStart = new UnityEvent();
    
    [SerializeField] private Vector3 offset = new Vector3(0, 2, 2);

    [Header("Player")] 
    private Transform _playerCamera;
    
    // 用于记录摄像机初始位置和旋转
    private Vector3 _initialCameraPosition;
    private Quaternion _initialCameraRotation;
    
    private int _currentDialogueIndex;

    private void Start()
    {
        dialogueParent.SetActive(false);
        _playerCamera = Camera.main.transform;
        mouseWorld.SetActive(false);
        
        _initialCameraPosition = _playerCamera.position;
        _initialCameraRotation = _playerCamera.rotation;
    }


    /// <summary>
    /// 开始对话，指定起始索引
    /// </summary>
    /// <param name="textToPrint">对话文本列表</param>
    /// <param name="npc">NPC Transform</param>
    /// <param name="startIndex"></param>
    public void DialogueStart(List<dialogueString> textToPrint, Transform npc, int startIndex = 0)
    {
        dialogueParent.SetActive(true);
        StartCoroutine(TurnCameraTowardsNpc(npc));
        _dialogueList = textToPrint;
        _currentDialogueIndex = startIndex;
        _optionSelected = false;
        DisableButtons();
        StartCoroutine(PrintDialogue());
        onDialogueStart.Invoke();

    }

    private void DisableButtons()
    {
        // 移除所有旧的监听器
        option1Button.onClick.RemoveAllListeners();
        option2Button.onClick.RemoveAllListeners();
        
        option1Button.interactable = false;
        option2Button.interactable = false;

        option1Button.GetComponentInChildren<TMP_Text>().text = ".";
        option2Button.GetComponentInChildren<TMP_Text>().text = ".";
    }

    private IEnumerator TurnCameraTowardsNpc(Transform npc)
    {
        Transform npcHead = npc.Find("Cube"); // 确保名字匹配
        if (npcHead == null)
        {
            Debug.LogError("找不到 NPC 的 Head 子对象！");
            yield break;
        }
    
        Vector3 targetPosition = npc.position + npc.TransformDirection(offset);

        Vector3 startPosition = _playerCamera.transform.position;
        Quaternion startRotation = _playerCamera.transform.rotation;
        
        Quaternion targetRotation = Quaternion.LookRotation(npcHead.position - targetPosition);

        float elapsedTime = 0f;
        float duration = 1f;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            _playerCamera.transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            _playerCamera.transform.rotation = Quaternion.Slerp(startRotation, targetRotation, t);

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _playerCamera.transform.position = targetPosition;
        _playerCamera.transform.rotation = targetRotation;
    }

    
    private bool _optionSelected;

    private IEnumerator PrintDialogue()
    {
        while (_currentDialogueIndex < _dialogueList.Count)
        {
            dialogueString line = _dialogueList[_currentDialogueIndex];
            
            line.startDialogueEvent?.Invoke();

            if (line.isQuestion)
            {
                yield return StartCoroutine(TypeText(line.text));
                
                option1Button.interactable = true;
                option2Button.interactable = true;

                option1Button.GetComponentInChildren<TMP_Text>().text = line.answerOption1;
                option2Button.GetComponentInChildren<TMP_Text>().text = line.answerOption2;
                
                option1Button.onClick.AddListener(() => HandleOptionSelected(line.option1IndexJump));
                option2Button.onClick.AddListener(() => HandleOptionSelected(line.option2IndexJump));

                yield return new WaitUntil((() => _optionSelected));
                
            }
            else
            {
                yield return StartCoroutine(TypeText(line.text));

                yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
                _currentDialogueIndex = line.nextIndexJump >= 0 ? line.nextIndexJump : _currentDialogueIndex + 1;
            }
            line.endDialogueEvent?.Invoke();
            _optionSelected = false;
        }
        
        DialogueStop();
    }


    private IEnumerator TypeText(String text)
    {
        dialogueText.text = "";
        foreach (char letter in text)
        {
            dialogueText.text += letter;
            yield return new WaitForSeconds(typeSpeed);
        }
        
        if(!_dialogueList[_currentDialogueIndex].isQuestion)
        {
            yield return new WaitUntil((() => Input.GetMouseButtonDown(0)));
        }

        if (_dialogueList[_currentDialogueIndex].isEnd)
            DialogueStop();

        //currentDialogueIndex++;
    }
    
    
    private void HandleOptionSelected(int indexJump)
    {
        DisableButtons();
        _currentDialogueIndex = indexJump;
        _optionSelected = true;
    }

    private void DialogueStop()
    {
        StopAllCoroutines();
        dialogueText.text = "";
        dialogueParent.SetActive(false);
        
        // 启动摄像机返回的协程
        StartCoroutine(ReturnCameraCoroutine());
        
        onDialogueEnd?.Invoke();
    }
    
    private IEnumerator ReturnCameraCoroutine()
    {
        float duration = 1.5f;
        float elapsedTime = 0f;
        Vector3 startPos = _playerCamera.position;
        Quaternion startRot = _playerCamera.rotation;

        while (elapsedTime < duration)
        {
            float t = elapsedTime / duration;
            _playerCamera.position = Vector3.Lerp(startPos, _initialCameraPosition, t);
            _playerCamera.rotation = Quaternion.Slerp(startRot, _initialCameraRotation, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        _playerCamera.position = _initialCameraPosition;
        _playerCamera.rotation = _initialCameraRotation;
    }
}
