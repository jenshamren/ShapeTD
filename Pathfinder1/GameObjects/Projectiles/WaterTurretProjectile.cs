using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapeTD
{
    internal class WaterTurretProjectile : WeaponProjectile
    {
        private Ellipse model;
        private const float slowRate = .5f;
        private const int slowDurationMilliseconds = 3000;
        private const int slowRadius = 50;
        protected override int Damage
        {
            get
            {
                return 10;
            }
        }
        protected override float Speed
        {
            get
            {
                return 50f;
            }
        }
        public override FrameworkElement Model
        {
            get
            {
                return model;
            }
        }
        public WaterTurretProjectile(GameController game, Weapon firingWeapon) : base(game, firingWeapon)
        {
           
        }
        protected override void Initialize()
        {
            base.Initialize();
            Shape projectileModel = (Shape)Game.GameObjectModels["waterTurretProjectile"];
            model = new Ellipse()
            {
                Height = projectileModel.Height,
                Width = projectileModel.Width,
                Fill = projectileModel.Fill,
            };
        }
        private void SlowEnemiesInRange()
        {
            var center = Transform.Transform(new Point(Width / 2, Height / 2));
            Collider = new CirlceCollider(this, Transform, center, slowRadius);
            if (CollisionTester.Collision(this))
            {
                foreach (var obj in CollisionTester.CollidingObjects)
                {
                    Enemy enemy = obj as Enemy;
                    if (enemy != null)
                    {
                        enemy.Slow(slowDurationMilliseconds, slowRate);
                    }
                }
            }
        }
        public override void Update()
        {
            base.Update();
            if (Collided && CollisionTester.CollidingObject is Enemy)
            {
                SlowEnemiesInRange();          
                ExplosionAnimator.StartExplosion(Game, this, typeof(WaterTurretProjectileExplosion));
                Destroy();
            }
        }

    }
}