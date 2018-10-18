using System.Windows;
using System.Windows.Shapes;

namespace ShapeTD
{
    class TankMissileLauncherMissile : WeaponProjectile
    {
        private Shape missile;
        private const int explosionRadius = 100;
        protected override int Damage { get { return 1000; } }
        protected override float Speed { get { return 50f; } }
        public override FrameworkElement Model
        {
            get
            {
                return missile;
            }
        }
        public TankMissileLauncherMissile(GameController game, Weapon firingWeapon) : base(game, firingWeapon)
        {

        }
        protected override void Initialize()
        {
            base.Initialize();
            missile = CreateMissile();
        }
        private Shape CreateMissile()
        {
            Shape model = (Shape)(Game.GameObjectModels["missile"]);
            Ellipse missileModel = new Ellipse()
            {
                Width = model.Width,
                Height = model.Height,
                Fill = model.Fill,
                Stroke = model.Stroke,
                StrokeThickness = model.StrokeThickness
            };
            return missileModel;
        }
        private void DamageEnemiesInRange()
        {
            var centerOfExplosion = CollisionTester.CollidingObject.Collider.Center;
            Collider = new CirlceCollider(this, Transform, centerOfExplosion, explosionRadius);
            if (CollisionTester.Collision(this))
            {
                foreach (var obj in CollisionTester.CollidingObjects)
                {
                    Enemy enemy = obj as Enemy;
                    if (enemy != null)
                    {
                        enemy.Damage(Damage, this);
                    }
                }
            }
        }
        public override void Update()
        {
            base.Update();
            if (Collided)
            {
                if (CollisionTester.CollidingObject is Enemy)
                {
                    DamageEnemiesInRange();
                    ExplosionAnimator.StartExplosion(Game, this, typeof(BigExplosion));
                    Destroy();
                }
            }
        }
    }
}
