using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace ShapeTD
{
    class AmmunitionLeftAnimation : Animation, IUpdate
    {
        private Shape[] projectileModels;
        private Weapon targetWeapon;
        private int sizeX;
        public AmmunitionLeftAnimation(GameController game, Weapon targetWeapon) : base(game, targetWeapon)
        {
            this.targetWeapon = targetWeapon;
            Initialize();
        }
        private void Initialize()
        {
            sizeX = targetWeapon.MagazineSize;
            projectileModels = GetModels();
            TimeSpanMilliseconds = 850;
            for(int i = 0; i < projectileModels.Length; i++)
            {
                Game.PlayArea.Children.Add(projectileModels[i]);
            }
            Game.OnGamePaused += Game_OnGamePaused;
            Game.OnGameUnPaused += Game_OnGameUnPaused;
        }
        private Shape[] GetModels()
        {
            Shape[] temp = new Shape[sizeX];
            for (int x = 0; x < sizeX; x++)
            {
                temp[x] = GetProjectileModel();
            }
            return temp;
        }
        private Shape GetProjectileModel()
        {
            Type projectileType = targetWeapon.GetProjectileType();
            Shape projectileModel = null;
            if (projectileType == typeof(TankMachineGunBullet))
            {
                 projectileModel = (Shape)Game.GameObjectModels["bullet"];
            }
            if (projectileType == typeof(TankMissileLauncherMissile))
            {
                projectileModel = (Shape)Game.GameObjectModels["missile"];
            }
            Ellipse newModel = new Ellipse()
            {
                Height = projectileModel.Height * .65,
                Width = projectileModel.Width + 2,
                Fill = projectileModel.Fill,
                Stroke = projectileModel.Stroke,
                StrokeThickness = .6,
                Opacity = .5f,
                Visibility = Visibility.Hidden
            };
            return newModel;
        }
        private void MoveToTarget()
        {
            int projectileWidth = (int)projectileModels[0].Width;
            float overlap = 1.5f;
            float yDistance = (20 + (float)projectileModels[0].Height);
            float totalWidth = (projectileWidth / overlap) * sizeX;
            float pointsToCenterX = (float)(targetWeapon.Holder.Model.ActualWidth - totalWidth) / 2;
            Point startPos = new Point(targetWeapon.Holder.Position.X + pointsToCenterX, targetWeapon.Holder.Position.Y - yDistance);
            for (int x = 0; x < sizeX; x++)
            {
                Point newPos = new Point(startPos.X + (x * projectileWidth / overlap), startPos.Y);
                Canvas.SetLeft(projectileModels[x], newPos.X);
                Canvas.SetTop(projectileModels[x], newPos.Y);
            }
        }
        private void Hide()
        {
            foreach (var element in projectileModels)
            {
                element.Visibility = Visibility.Hidden;
            }
        }
        private void DisplayAtTarget()
        {
            MoveToTarget();
            int displayedProjectiles = 0;
            for (int x = 0; x < sizeX; x++)
            {
                projectileModels[x].Visibility = Visibility.Visible;
                if (displayedProjectiles >= targetWeapon.Magazine.Count)
                {
                    projectileModels[x].Visibility = Visibility.Hidden;
                }
                displayedProjectiles++;
            }
        }
        public void Update()
        {
            if (IsRunning)
            {
                DisplayAtTarget();
                AnimationCounter++;
            }
            if (AnimationCounter * Game.DeltaTime > TimeSpanMilliseconds)
            {
                Stop();
                Hide();
            }
        }
        public void Game_OnGamePaused()
        {
            foreach(var obj in projectileModels)
            {
                Game.PlayArea.Children.Remove(obj);
            }
        }
        public void Game_OnGameUnPaused()
        {
            foreach(var obj in projectileModels)
            {
                Game.PlayArea.Children.Add(obj);
            }
        }
    }
}
