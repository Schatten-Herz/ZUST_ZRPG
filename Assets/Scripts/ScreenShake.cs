using System;
using UnityEngine;
using Unity.Cinemachine;
public class ScreenShake : MonoBehaviour
{
    public static ScreenShake Instance { get; private set; }
    
    private CinemachineImpulseSource _cinemachineImpulseSource;

    private void Awake()
    {
        if (Instance != null)
        {
            Debug.LogError("有不止一个 ScreenShake" + transform + "-" + Instance);
            Destroy(gameObject);
            return;
        }
        Instance = this;
        _cinemachineImpulseSource = GetComponent<CinemachineImpulseSource>();
    }
    
    public void Shake(float intensity = 1f)
    {
        _cinemachineImpulseSource.GenerateImpulse(intensity);
    }
}
