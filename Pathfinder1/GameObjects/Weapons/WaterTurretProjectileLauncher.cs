using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapeTD
{
    class WaterTurretLauncher : Weapon
    {
        private Queue<WeaponProjectile> magazine;
        private Canvas launcherModel;
        private const int magazineSize = 1;
        private const int reloadInterval = 1;
        private const int fireInterVal = 700;
        public override Queue<WeaponProjectile> Magazine
        {
            get
            {
                return magazine;
            }

            protected set
            {
                magazine = value;
            }
        }
        public override int MagazineSize
        {
            get
            {
                return magazineSize;
            }
        }
        protected override int FireInterval
        {
            get
            {
                return fireInterVal;
            }
        }
        protected override int ReloadInterval
        {
            get
            {
                return reloadInterval;
            }
        }
        public override Point Position
        {
            get
            {
                return new Point(Canvas.GetLeft(Holder.Model), Canvas.GetTop(Holder.Model));
            }

            protected set
            {
                base.Position = value;
            }
        }
        public override FrameworkElement Model
        {
            get
            {
                return launcherModel;
            }
        }
        public WaterTurretLauncher(GameController game, WaterTurret holderTurret): base(game, holderTurret)
        {

        }
        protected override void Initialize()
        {
            magazine = new Queue<WeaponProjectile>();
            launcherModel = (Canvas)GameHelper.FindCanvasChild(Holder.Model as Canvas, "waterTurretProjectileLauncher");
        }
        protected override Shape GetProjectileSpawnPoint()
        {
            return (Shape)GameHelper.FindCanvasChild(launcherModel, "projectileSpawnPoint");
        }
        public override Type GetProjectileType()
        {
            return typeof(WaterTurretProjectile);
        }
        protected override void Reload()
        {
            magazine.Enqueue(new WaterTurretProjectile(Game, this));
        }
        public override void Update()
        {
            base.Update();
            double angle = LookAt(Target);
            Model.RenderTransform = new RotateTransform(angle, Width / 2, launcherModel.Height / 2);
        }
    }
}
