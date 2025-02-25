using System;
using UnityEngine;

public class UnitAnimator : MonoBehaviour
{
    private static readonly int IsWalking = Animator.StringToHash("IsWalking");
    private static readonly int Shoot = Animator.StringToHash("Shoot");
    [SerializeField] private Animator animator;
    [SerializeField] private Transform bulletProjectilePrefab;
    [SerializeField] private Transform shootPointTransform;

    private void Awake()
    {
        if (TryGetComponent(out MoveAction moveAction))
        {
            moveAction.OnStartMoving += MoveActionOnStartMoving;
            moveAction.OnStopMoving += MoveActionOnStopMoving;
        }
        if (TryGetComponent(out ShootAction shootAction))
        {
            shootAction.OnShoot += ShootActionOnShoot;
        }
    }
    
    private void MoveActionOnStartMoving(object sender, EventArgs e)
    {
        animator.SetBool(IsWalking, true);
    }
    
    private void MoveActionOnStopMoving(object sender, EventArgs e)
    {
        animator.SetBool(IsWalking, false);
    }
    
    private void ShootActionOnShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        animator.SetTrigger(Shoot);

        Transform bulletProjectileTransform = Instantiate(bulletProjectilePrefab, shootPointTransform.position, Quaternion.identity);
        BulletProjectile bulletProjectile = bulletProjectileTransform.GetComponent<BulletProjectile>();
        
        Vector3 targetUnitShootAtPosition = e.TargetUnit.GetWorldPosition();
        
        targetUnitShootAtPosition.y = shootPointTransform.position.y;
        
        bulletProjectile.Setup(targetUnitShootAtPosition);
    }
}
