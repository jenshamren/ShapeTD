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
    class TankMachineGun : Weapon
    {
        private Canvas launcherModel;
        private Queue<WeaponProjectile> magazine;
        private int reloadCounter;
        protected override int FireInterval { get { return 100; } }
        protected override int ReloadInterval { get { return 1000; } }
        public override int MagazineSize { get { return 15; } }
        public override Queue<WeaponProjectile> Magazine
        {
            get { return magazine; }
            protected set { magazine = value; }
        }
        public override FrameworkElement Model
        {
            get
            {
                return launcherModel;
            }
        }
        public TankMachineGun(GameController game, MovableGameObject holder) : base(game, holder)
        {

        }
        protected override void Initialize()
        {
            magazine = new Queue<WeaponProjectile>();
            ProjectilesLeftAnimation = new AmmunitionLeftAnimation(Game, this);
            for (int i = 0; i < MagazineSize; i++)
            {
                magazine.Enqueue(new TankMachineGunBullet(Game, this));
            }
            launcherModel = (Canvas)GameHelper.FindCanvasChild(Holder.Model as Canvas, "tankProjectileLauncher");
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
            if (!Reloading)
            {
                Reloading = true;
            }
            reloadCounter++;
            if(reloadCounter * Game.DeltaTime > ReloadInterval)
            {
                for (int i = 0; i < MagazineSize; i++)
                {
                    magazine.Enqueue(new TankMachineGunBullet(Game, this));
                }
                Reloading = false;
                ProjectilesLeftAnimation.Start();
                reloadCounter = 0;
            }
        }
        public override Type GetProjectileType()
        {
            return typeof(TankMachineGunBullet);
        }
    }
}
