using System;
using UnityEngine;

public class GrenadeProjectile : MonoBehaviour
{
    public static event EventHandler OnAnyGrenadeExploded;

    [SerializeField] private Transform grenadeExplodeVfxPrefab;
    [SerializeField] private TrailRenderer trailRenderer;
    [SerializeField] private AnimationCurve arcYAnimationCurve;
    
    private Vector3 _targetPosition;
    private Action _onGrenadeBehaviourComplete;
    private float totalDistance;
    private Vector3 positionXZ;

    private void Update()
    {
        Vector3 moveDir = (_targetPosition - positionXZ).normalized;
        
        float moveSpeed = 15f;
        
        positionXZ += moveDir * (moveSpeed * Time.deltaTime);
        
        float distance = Vector3.Distance(positionXZ, _targetPosition);
        float distanceNormalized = 1 - (distance / totalDistance);

        float maxHeight = totalDistance / 4f;
        float positionY = arcYAnimationCurve.Evaluate(distanceNormalized) * maxHeight;
        transform.position = new Vector3(positionXZ.x, positionY, positionXZ.z);
        
        float reachedTargetDistance = 0.1f;
        if (Vector3.Distance(positionXZ, _targetPosition) < reachedTargetDistance)
        {
            float damageRadius = 4f;
            Collider[] colliderArray = Physics.OverlapSphere(_targetPosition, damageRadius);
            
            foreach (Collider collider in colliderArray)
            {
                if (collider.TryGetComponent(out Unit targetUnit))
                {
                    targetUnit.Damage(30);
                }
                
                if (collider.TryGetComponent<DestructibleCreate>(out DestructibleCreate destructibleCreate))
                {
                    destructibleCreate.Damage();
                }
            }
            OnAnyGrenadeExploded?.Invoke(this, EventArgs.Empty);

            trailRenderer.transform.parent = null;
            
            Instantiate(grenadeExplodeVfxPrefab, _targetPosition + Vector3.up * 1f, Quaternion.identity);
            Destroy(gameObject);

            _onGrenadeBehaviourComplete();
        }
    }

    public void Setup(GridPosition targetGridPosition,Action onGrenadeBehaviourComplete)
    {
        this._onGrenadeBehaviourComplete = onGrenadeBehaviourComplete;
        _targetPosition = LevelGrid.Instance.GetWorldPosition(targetGridPosition);

        positionXZ = transform.position;
        positionXZ.y = 0;
        totalDistance = Vector3.Distance(transform.position, _targetPosition);
    }
}
