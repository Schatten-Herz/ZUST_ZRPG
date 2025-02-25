using System;
using UnityEngine;

public class MouseWorld : MonoBehaviour
{
    private static MouseWorld _instance;
    
    [SerializeField] private LayerMask mousePlaneLayerMask;
    
    private void Awake()
    {
        _instance = this;
    }

    private void Update()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.Log(Physics.Raycast(ray, out RaycastHit raycastHit,float.MaxValue, _instance.mousePlaneLayerMask));
        transform.position = raycastHit.point;
    }

    public static Vector3 GetPosition()  //鼠标世界位置
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        Debug.Log(Physics.Raycast(ray, out RaycastHit raycastHit,float.MaxValue, _instance.mousePlaneLayerMask));
        return raycastHit.point;
    }
}
