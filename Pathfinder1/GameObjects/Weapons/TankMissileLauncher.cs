using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapeTD
{
    class TankMissileLauncher : Weapon
    {
        private Canvas launcherModel;
        private Queue<WeaponProjectile> magazine;
        private int reloadTicks;
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
        public override int MagazineSize { get { return 1; } }
        protected override int FireInterval { get { return 1; } }
        protected override int ReloadInterval { get { return 3000; } }
        public override FrameworkElement Model
        {
            get
            {
                return launcherModel;
            }
        }
        public TankMissileLauncher(GameController game, MovableGameObject holder) : base (game, holder)
        {

        }
        protected override void Initialize()
        {
            ProjectilesLeftAnimation = new AmmunitionLeftAnimation(Game, this);
            magazine = new Queue<WeaponProjectile>();
            launcherModel = (Canvas)GameHelper.FindCanvasChild(Holder.Model as Canvas, "tankProjectileLauncher");
            Magazine.Enqueue(new TankMissileLauncherMissile(Game, this));
            Game.PlayArea.MouseMove += PlayArea_MouseMove;
        }
        protected override void Game_OnGamePaused()
        {
            Game.PlayArea.MouseMove -= PlayArea_MouseMove;
        }
        protected override void Game_OnGameUnPaused()
        {
            Game.PlayArea.MouseMove += PlayArea_MouseMove;
        }
        private void PlayArea_MouseMove(object sender, System.Windows.Input.MouseEventArgs e)
        {
            Target = e.GetPosition(Game.PlayArea);
        }
        protected override Shape GetProjectileSpawnPoint()
        {
            return (Shape)GameHelper.FindCanvasChild(launcherModel, "projectileSpawnPoint");
        }
        protected override void Reload()
        {
            float deltaTime = Game.DeltaTime;
            reloadTicks++;
            if ((reloadTicks * deltaTime) >= ReloadInterval)
            {
                Magazine.Enqueue(new TankMissileLauncherMissile(Game, this));
                reloadTicks = 0;
                ProjectilesLeftAnimation.Start();
            }
        }
        public override Type GetProjectileType()
        {
            return typeof(TankMissileLauncherMissile);
        }
    }
}
