using UnityEngine;

public class CameraFollowWithoutRotation : MonoBehaviour
{
    // 要跟随的目标，例如角色
    public Transform target;

    // 控制是否锁定旋转（默认锁定）
    public bool lockRotation = true;

    private Vector3 offset;

    private void Start()
    {
        if (target != null)
        {
            offset = transform.position - target.position;
        }
    }

    private void LateUpdate()
    {
        if (target != null)
        {
            // 始终跟随目标位置
            transform.position = target.position + offset;

            // 如果锁定旋转，保持摄像机始终朝向角色
            if (lockRotation)
            {
                transform.LookAt(target);
            }
        }
    }
}