
namespace ShapeTD
{
    interface IDamagable : ICollidableGameObject
    {
        void Damage(int damage, WeaponProjectile damageSource);
        int HitPoints { get; }
    }
}
