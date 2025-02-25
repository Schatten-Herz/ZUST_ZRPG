using System;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    [SerializeField] private bool invert;
    private Transform _cameraTransform;

    private void Awake()
    {
        _cameraTransform = Camera.main.transform;
    }
    
    private void LateUpdate()
    {
        // 计算目标位置：摄像机位置，但将 y 坐标保持为当前物体的 y 坐标
        Vector3 targetPosition = new Vector3(_cameraTransform.position.x, transform.position.y, _cameraTransform.position.z);

        // 让物体朝向目标位置，这样只会在 y 轴旋转
        transform.LookAt(targetPosition);
        
        // 如果需要反转（即背对摄像机），则再旋转 180 度
        if (invert)
        {
            transform.Rotate(0, 180, 0);
        }
    }
}
