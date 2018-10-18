
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapeTD
{
    abstract class Enemy : MovableGameObject, IDamagable, IUpdate
    {
        protected PathFollower PathFollower { get; private set; }
        protected int StatChangeDuration { get; set; }
        protected int StatChangeCounter { get; set; }
        protected int AnimationCounter { get; set; }
        protected abstract int CashForKilling { get; }
        protected abstract float Speed { get; }
        public abstract int HitPoints { get; set; }
        public bool Slowed { get; set; }
        public ICollider Collider { get; set; }
        public Enemy(GameController game) : base (game)
        {
            PathFollower = new PathFollower(game, this);
        }
        protected override void Game_OnGamePaused()
        {
            return;
        }
        protected override void Game_OnGameUnPaused()
        {
            return;
        }
        public abstract void Slow(int duration, float rate);
        public abstract void Damage(int damage, WeaponProjectile damageSource);
        public abstract void Update();

    }
}
