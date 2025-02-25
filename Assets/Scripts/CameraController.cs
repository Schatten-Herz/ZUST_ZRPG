using System;
using Unity.Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController Instance { get; private set; }
    
    private Vector3 _targetFollowOffset;
    private CinemachineFollow _cinemachineFollow;
    
    [SerializeField] 
    private float _minFollowYOffset = 1f;
    [SerializeField] 
    private float _maxFollowYOffset = 15f;
    [SerializeField] 
    private float _minFollowZOffset = -12f;
    [SerializeField] 
    private float _maxFollowZOffset = -3f;

// 提供只读属性
    public float MinFollowYOffset => _minFollowYOffset;
    public float MaxFollowYOffset => _maxFollowYOffset;
    public float MinFollowZOffset => _minFollowZOffset;
    public float MaxFollowZOffset => _maxFollowZOffset;
    
    [SerializeField] private CinemachineCamera cinemachineCamera;
    
    private Transform _focusTarget;
    
    /// <summary>
    /// 设置需要关注的单位。当一个单位被选中或者敌人 AI 活动时调用此方法。
    /// </summary>
    /// <param name="target">目标单位的 Transform</param>
    public void SetFocusTarget(Transform target)
    {
        _focusTarget = target;
    }

    /// <summary>
    /// 清除关注目标，当不需要关注时调用（例如取消选中）。
    /// </summary>
    public void ClearFocusTarget()
    {
        _focusTarget = null;
    }
    
    // 当脚本启用时，订阅 UnitActionSystem 的事件
    private void OnEnable()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged += HandleSelectedUnitChanged;
        TurnSystem.Instance.OnTurnChanged += HandleTurnChanged;
    }

    // 当脚本禁用时，取消订阅事件
    private void OnDisable()
    {
        UnitActionSystem.Instance.OnSelectedUnitChanged -= HandleSelectedUnitChanged;
        TurnSystem.Instance.OnTurnChanged += HandleTurnChanged;
    }

    // 事件回调：当选中的 unit 发生变化时调用
    private void HandleSelectedUnitChanged(object sender, EventArgs e)
    {
        // 获取当前选中的 unit
        Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
        if (selectedUnit != null)
        {
            // 将摄像机的关注目标设置为选中 unit 的 transform
            SetFocusTarget(selectedUnit.transform);
        }
        else
        {
            // 如果没有选中 unit，则清除关注目标
            ClearFocusTarget();
        }
    }
    
    private void HandleTurnChanged(object sender, EventArgs e)
    {
        if (TurnSystem.Instance.IsPlayerTurn())
        {
            Unit selectedUnit = UnitActionSystem.Instance.GetSelectedUnit();
            if (selectedUnit != null)
            {
                SetFocusTarget(selectedUnit.transform);
            }
            else
            {
                ClearFocusTarget();
            }
        }
    }

    private void Awake()
    {
        
         if (Instance != null && Instance != this)
         {
             Destroy(gameObject);
             return;
         }
         Instance = this;
    }
    

    private void Start()
    {
        _cinemachineFollow = cinemachineCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineFollow;
        if (_cinemachineFollow != null)
        {
            _targetFollowOffset = _cinemachineFollow.FollowOffset;
        }
    }

    private void Update()
    {
        // 如果玩家按下任意 WASD 键，则取消锁定目标
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || 
            Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            ClearFocusTarget();
        }

        if (_focusTarget != null)
        {
            FocusOnTarget();
        }
        else
        {
            HandleMovement();
        }
    
        HandleRotation();
        HandleZoom();
    }


    private void HandleMovement()
    {
        Vector3 inputMoveDir = Vector3.zero;
        
        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDir.z = +1f;
        }
        
        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDir.x = -1f;
        }
        
        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDir.z = -1f;
        }
        
        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDir.x = +1f;
        }

        float moveSpeed = 7f;
        Vector3 moveVector = transform.forward * inputMoveDir.z + transform.right * inputMoveDir.x;
        transform.position += moveVector * (moveSpeed * Time.deltaTime);
    }

    private void HandleRotation()
    {
        Vector3 rotationVector = Vector3.zero;
        
        if (Input.GetKey(KeyCode.Q))
        {
            rotationVector.y = -1f;
        }
        
        if (Input.GetKey(KeyCode.E))
        {
            rotationVector.y = +1f;
        }
        
        float rotationSpeed = 100f;
        transform.eulerAngles += rotationVector * (rotationSpeed * Time.deltaTime);
    }

    private void HandleZoom()
    {
        if (_cinemachineFollow != null)
        {
            float zoomAmountY = 1.5f;
            float zoomAmountZ = 0.75f;
            
            if (Input.mouseScrollDelta.y > 0)
            {
                _targetFollowOffset.y -= zoomAmountY;
                _targetFollowOffset.z += zoomAmountZ;
            }
            if (Input.mouseScrollDelta.y < 0)
            {
                _targetFollowOffset.y += zoomAmountY;
                _targetFollowOffset.z -= zoomAmountZ;
            }
            
            _targetFollowOffset.y = Mathf.Clamp(_targetFollowOffset.y, MinFollowYOffset, MaxFollowYOffset);
            _targetFollowOffset.z = Mathf.Clamp(_targetFollowOffset.z, MinFollowZOffset, MaxFollowZOffset);

            float zoomSpeed = 5f;
            _cinemachineFollow.FollowOffset = Vector3.Lerp(
                _cinemachineFollow.FollowOffset,
                _targetFollowOffset,
                Time.deltaTime * zoomSpeed);
        }
    }
    
    /// <summary>
    /// 当存在关注目标时，平滑将摄像机平移至目标上方
    /// （这里调整 X 和 Z 坐标，保持当前的 Y 坐标及旋转角度不变）
    /// </summary>
    private void FocusOnTarget()
    {
        // 计算目标位置：使用目标的 X/Z 坐标，保持当前摄像机的 Y 坐标
        Vector3 targetPosition = new Vector3(
            _focusTarget.position.x,
            transform.position.y,
            _focusTarget.position.z);
        
        float focusSpeed = 5f;
        transform.position = Vector3.Lerp(transform.position, targetPosition, focusSpeed * Time.deltaTime);
    }
}
