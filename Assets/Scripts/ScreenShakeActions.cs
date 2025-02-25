using System;
using UnityEngine;

public class ScreenShakeActions : MonoBehaviour
{
    private void Start()
    {
        ShootAction.OnAnyShoot += ShootActionOnAnyShoot;
        GrenadeProjectile.OnAnyGrenadeExploded += GrenadeProjectileOnAnyGrenadeExploded;
    }
    
    private void ShootActionOnAnyShoot(object sender, ShootAction.OnShootEventArgs e)
    {
        ScreenShake.Instance.Shake();
    }
    
    private void GrenadeProjectileOnAnyGrenadeExploded(object sender, EventArgs e)
    {
        ScreenShake.Instance.Shake(5f);
    }
}
