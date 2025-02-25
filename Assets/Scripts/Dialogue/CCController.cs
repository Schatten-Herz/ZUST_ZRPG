using System.Collections;
using UnityEngine;
using Unity.Cinemachine;

public class CCController : MonoBehaviour
{
    [Header("摄像机设置")]
    [SerializeField] private CinemachineCamera cinemachineCamera;
    [SerializeField] private Vector3 cameraOffset = new Vector3(0f, 1.8f, -1.2f);
    [SerializeField] private float transitionTime = 0.8f;

    private CinemachineTransposer transposer;
    private Vector3 originalOffset;
    private Transform originalFollowTarget;
    private Quaternion originalRotation;

    private void Awake()
    {
        if (cinemachineCamera == null)
        {
            Debug.LogError("请为 CinemachineCameraController 分配 Cinemachine 摄像机");
            return;
        }
        
        transposer = cinemachineCamera.GetCinemachineComponent(CinemachineCore.Stage.Body) as CinemachineTransposer;
        if (transposer == null)
        {
            Debug.LogError("摄像机缺少 CinemachineTransposer 组件");
            return;
        }

        originalOffset = transposer.m_FollowOffset;
        originalFollowTarget = cinemachineCamera.Follow;
        originalRotation = cinemachineCamera.transform.rotation;
    }

    /// <summary>
    /// 开始平滑过渡，转向指定 NPC 的焦点
    /// </summary>
    /// <param name="npcFocus">NPC 的焦点 Transform</param>
    public void FocusOnNPC(Transform npcFocus)
    {
        StartCoroutine(TransitionToNPC(npcFocus));
    }

    private IEnumerator TransitionToNPC(Transform focusPoint)
    {
        // 设置摄像机目标
        cinemachineCamera.LookAt = focusPoint;
        cinemachineCamera.Follow = focusPoint;
        
        // 计算目标偏移（基于 NPC 的局部方向）
        Vector3 targetOffset = focusPoint.TransformDirection(cameraOffset);
        Vector3 startOffset = transposer.m_FollowOffset;
        
        // 计算旋转过渡
        Quaternion startRotation = cinemachineCamera.transform.rotation;
        // 目标旋转：让摄像机正对 NPC 的位置
        Quaternion targetRotation = Quaternion.LookRotation(focusPoint.position - cinemachineCamera.transform.position);
        
        float elapsed = 0f;
        while (elapsed < transitionTime)
        {
            float t = elapsed / transitionTime;
            // 平滑过渡偏移
            transposer.m_FollowOffset = Vector3.Lerp(startOffset, targetOffset, t);
            // 平滑过渡旋转
            cinemachineCamera.transform.rotation = Quaternion.Lerp(startRotation, targetRotation, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transposer.m_FollowOffset = targetOffset;
        cinemachineCamera.transform.rotation = targetRotation;
    }

    /// <summary>
    /// 恢复摄像机到初始状态，同时平滑过渡偏移和旋转
    /// </summary>
    public void ResetCamera()
    {
        StartCoroutine(ResetCameraCoroutine());
    }

    private IEnumerator ResetCameraCoroutine()
    {
        Vector3 currentOffset = transposer.m_FollowOffset;
        Quaternion currentRotation = cinemachineCamera.transform.rotation;
        float elapsed = 0f;
        while (elapsed < transitionTime)
        {
            float t = elapsed / transitionTime;
            transposer.m_FollowOffset = Vector3.Lerp(currentOffset, originalOffset, t);
            cinemachineCamera.transform.rotation = Quaternion.Lerp(currentRotation, originalRotation, t);
            elapsed += Time.deltaTime;
            yield return null;
        }
        transposer.m_FollowOffset = originalOffset;
        cinemachineCamera.transform.rotation = originalRotation;

        // 恢复摄像机原始的跟随和朝向设置
        cinemachineCamera.LookAt = null;
        cinemachineCamera.Follow = originalFollowTarget;
    }
}
