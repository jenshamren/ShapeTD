using System;
using System.Windows;
using System.Windows.Controls;

namespace ShapeTD
{
    class EnemyHealthBar : MovableGameObject
    {
        private ProgressBar healthBar;
        private Enemy target;
        private bool isActive = false;
        public override FrameworkElement Model
        {
            get
            {
                return healthBar;
            }
        }
        public EnemyHealthBar(GameController game, Enemy target) : base(game)
        {
            this.target = target;
            healthBar.Maximum = target.HitPoints;
            healthBar.Value = target.HitPoints;
        }
        protected override void Initialize()
        {
            base.Initialize();
            ProgressBar healthBarModel = (ProgressBar)(Game.GameObjectModels["enemyHealthBarModel"]);
            healthBar = new ProgressBar();
            healthBar.Width = healthBarModel.Width;
            healthBar.Height = healthBarModel.Height;
            healthBar.Background = healthBarModel.Background;
            healthBar.Foreground = healthBarModel.Foreground;
            healthBar.BorderBrush = healthBarModel.BorderBrush;
            Panel.SetZIndex(healthBar, 1);
        }
        public void Show()
        {
            int yDistance = (int)target.Height;
            double centerX = target.Position.X + (target.Width / 2) - (healthBar.Width / 2);
            MoveToPoint(new Point(centerX, target.Position.Y - yDistance));
            healthBar.Value = target.HitPoints;
            if(!isActive)
                Game.PlayArea.Children.Add(Model);
            isActive = true;
        }
        public void Hide()
        {
            Game.PlayArea.Children.Remove(Model);
            isActive = false;
        }
    }
}
