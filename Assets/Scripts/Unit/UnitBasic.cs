using UnityEngine;
using UnityEngine.Serialization;

[RequireComponent(typeof(CharacterController))]
public class UnitBasic : MonoBehaviour
{
    [Header("Movement")]
    [SerializeField] private float moveSpeed = 3f;
    [SerializeField] private float stoppingDistance = 0.1f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Animator unitAnimator;

    [Header("Gravity")]
    [SerializeField] private float gravity = 9.8f;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private float checkRadius = 0.2f;

    private CharacterController _controller;
    private Vector3 _targetPosition;
    private Vector3 _velocity;
    private bool _isGrounded;
    
    private bool _canMove = true;

    private void Start()
    {
        // 订阅对话事件
        DialogueManager dialogueManager = FindObjectOfType<DialogueManager>();
        if (dialogueManager != null)
        {
            dialogueManager.onDialogueStart.AddListener(DisableMovement);
            dialogueManager.onDialogueEnd.AddListener(EnableMovement);
        }
        
        _controller = GetComponent<CharacterController>();
        _targetPosition = transform.position; // 初始化目标位置为当前坐标
    }

    private void Update()
    {
        
        if (!_canMove) return;
        
        HandleGravity();
        HandleMouseInput();
        HandleMovement();
    }

    private void HandleGravity()
    {
        _isGrounded = Physics.CheckSphere(groundCheck.position, checkRadius, groundLayer);
        
        if (_isGrounded && _velocity.y < 0)
        {
            _velocity.y = -2f; // 轻微向下的力确保接地
        }
        else
        {
            _velocity.y -= gravity * Time.deltaTime;
        }
    }

    private void HandleMouseInput()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (!_canMove) return; 
            
            // 只获取 XZ 平面坐标，Y 轴保持当前位置
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, groundLayer))
            {
                _targetPosition = hit.point;
                _targetPosition.y = transform.position.y; // 保持当前高度
            }
        }
    }

    private void HandleMovement()
    {
        Vector3 horizontalTarget = new Vector3(_targetPosition.x, transform.position.y, _targetPosition.z);
        Vector3 direction = (horizontalTarget - transform.position).normalized;
        
        // 仅当目标在停止距离外时移动
        if (Vector3.Distance(transform.position, horizontalTarget) > stoppingDistance)
        {
            Vector3 move = direction * (moveSpeed * Time.deltaTime);

            float rotateSpeed = 10f;
            transform.forward = Vector3.Lerp(transform.forward,direction,Time.deltaTime * rotateSpeed);
            _controller.Move(move); // 使用 CharacterController 的移动方法
            unitAnimator.SetBool("IsWalking",true);
        }
        else
        {
            unitAnimator.SetBool("IsWalking",false);
        }

        // 应用重力
        _controller.Move(_velocity * Time.deltaTime);
    }
    
    // 禁用移动
    private void DisableMovement()
    {
        _canMove = false;
        _targetPosition = transform.position; // 清除目标位置
        Debug.Log("移动已禁用");
    }

    // 启用移动
    private void EnableMovement()
    {
        _canMove = true;
        Debug.Log("移动已启用");
    }
}