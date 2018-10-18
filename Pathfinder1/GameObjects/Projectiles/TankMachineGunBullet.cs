using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Shapes;

namespace ShapeTD
{
    class TankMachineGunBullet : WeaponProjectile
    {
        private const int damage = 200;
        private const float speed = 40f;
        private Shape bullet;
        public override FrameworkElement Model
        {
            get
            {
                return bullet;
            }
        }
        protected override int Damage
        {
            get
            {
                return damage;
            }
        }
        protected override float Speed
        {
            get
            {
                return speed;
            }
        }
        public TankMachineGunBullet(GameController game, Weapon firingWeapon) : base(game, firingWeapon)
        {

        }
        protected override void Initialize()
        {
            base.Initialize();
            Shape bulletModel = (Shape)Game.GameObjectModels["bullet"];
            bullet = new Ellipse()
            {
                Width = 5,
                Fill = bulletModel.Fill,
                Height = 40,
                Stroke = bulletModel.Stroke,
                StrokeThickness = 1,
            };
        }
        public override void Update()
        {
            base.Update();
            if (Collided)
            {
                Enemy collidingEnemy = CollisionTester.CollidingObject as Enemy;
                if (collidingEnemy != null)
                {
                    collidingEnemy.Damage(Damage, this);
                    ExplosionAnimator.StartExplosion(Game, this, typeof(TankBulletExplosion));
                    Destroy();
                }
            }
        }

    }
}
