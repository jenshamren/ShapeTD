using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;

namespace ShapeTD
{
    abstract class WeaponProjectile : MovableGameObject, IUpdate, ICollidableGameObject
    {
        private Vector targetDirection;
        private bool isFired = false;
        protected abstract int Damage { get; }
        protected abstract float Speed { get; }
        protected Weapon FiringWeapon { get; private set; }
        protected bool Collided { get; set; }
        protected CollisionHandler CollisionTester { get; set; }
        public ICollider Collider { get; set; }
        public WeaponProjectile(GameController game, Weapon firingWeapon) : base(game)
        {
            FiringWeapon = firingWeapon;
            CollisionTester = new CollisionHandler(game);
            Model.Visibility = Visibility.Hidden;
        }
        protected override void Game_OnGameUnPaused()
        {
            if (isFired)
            {
                base.Game_OnGameUnPaused();
            }
            else
            {
                return;
            }
        }
        private void SetProjectile()
        {
            RotateTransform transform = FiringWeapon.Model.RenderTransform as RotateTransform;
            float angle = (float)transform.Angle;
            Rotate(angle, 0, Height);
            targetDirection = GameHelper.GetDirection(angle);
            Model.Visibility = Visibility.Visible;
        }
        public virtual void Update()
        {
            if (!isFired)
            {
                MoveToPoint(new Point(FiringWeapon.ProjectileSpawnPoint.X, FiringWeapon.ProjectileSpawnPoint.Y - Height));
                SetProjectile();
                isFired = true;
            }
            else
            {
                MoveTowards(targetDirection, Speed * MoverDeltaTime);
                if (CollisionTester.Collision(this))
                {
                    Collided = true;
                }
                else if (OutSideOfGame())
                {
                    Destroy();
                }
            }
        }
    }
}
