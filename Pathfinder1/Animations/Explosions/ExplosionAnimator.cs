using System;

namespace ShapeTD
{
    static class ExplosionAnimator
    {
        private static Explosion GetExplosionType(GameController game, MovableGameObject target, WeaponProjectile explosionSource)
        {
            Explosion explosionAnimation = null;
            Type projectileType = explosionSource.GetType();
            if(projectileType == typeof(TankMachineGunBullet))
            {
                explosionAnimation = new MediumExplosion(game, target);
            }else if(projectileType == typeof(TankMissileLauncherMissile))
            {
                explosionAnimation = new BigExplosion(game, target);
            }
            return explosionAnimation;
        }
        public static void StartExplosion(GameController game, MovableGameObject target, WeaponProjectile explosionSource)
        {
            Explosion explosionAnimation = GetExplosionType(game, target, explosionSource);
            explosionAnimation.Start();
        }
        public static void StartExplosion(GameController game, MovableGameObject target, Type explosionType)
        {
            object[] parameters = new object[2];
            parameters[0] = game;
            parameters[1] = target;
            Explosion explosion = (Explosion)Activator.CreateInstance(explosionType, parameters);
        }
    }
}
