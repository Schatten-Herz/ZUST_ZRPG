using System;
using UnityEngine;

public class BulletProjectile : MonoBehaviour
{
    private Vector3 _targetPosition;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private Transform bulletHitFvxPrefab;
    
    public void Setup(Vector3 targetPosition)
    {
        this._targetPosition = targetPosition;
        
    }

    private void Update()
    {
        Vector3 moveDirection = (_targetPosition - transform.position).normalized;
        
        float distanceBeforeMoving = Vector3.Distance(transform.position, _targetPosition);
        
        float moveSpeed = 200f;
        transform.position += moveDirection * (Time.deltaTime * moveSpeed);
        
        float distanceAfterMoving = Vector3.Distance(transform.position, _targetPosition);
        
        if(distanceBeforeMoving < distanceAfterMoving)
        {
            transform.position = _targetPosition;
            
            trailRenderer.transform.parent = null;
            Destroy(gameObject);
        }
        
        Instantiate(bulletHitFvxPrefab, _targetPosition, Quaternion.identity);
    }
}
