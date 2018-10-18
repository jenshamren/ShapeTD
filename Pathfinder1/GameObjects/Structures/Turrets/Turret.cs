using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapeTD
{
    abstract class Turret : MovableGameObject, IUpdate, IStructure
    {
        private bool showRange;
        private Ellipse rangeModel;
        protected abstract float Range { get; }
        public abstract int Cost { get; }
        public Point Center
        {
            get
            {
                return new Point(Position.X + Width / 2, Position.Y + Height / 2);
            }
        }
        public Rect RectCollider { get; private set; }
        public Turret(GameController game) : base(game)
        {
            rangeModel = GetRangeModel();
            rangeModel.Visibility = Visibility.Hidden;
            Game.PlayArea.Children.Add(rangeModel);
            Game.PlayArea.MouseMove += PlayArea_MouseMove;
            OnPositionChanged += Turret_OnPositionChanged;
        }
        protected override void Game_OnGamePaused()
        {
            Game.PlayArea.Children.Remove(rangeModel);
        }
        protected override void Game_OnGameUnPaused()
        {
            if(showRange && !Game.PlayArea.Children.Contains(rangeModel))
                Game.PlayArea.Children.Add(rangeModel);
        }
        private void Turret_OnPositionChanged()
        {
            RectCollider = new Rect(Position.X, Position.Y, Width, Height);
        }
        private void PlayArea_MouseMove(object sender, MouseEventArgs e)
        {
            Point cursorPos = e.GetPosition(Game.PlayArea);
            showRange = ShowRange(cursorPos);
        }
        private bool ShowRange(Point cursorPos)
        {
            Rect cursor = new Rect(cursorPos.X, cursorPos.Y, 5, 5);
            if (cursor.IntersectsWith(RectCollider))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
        private Ellipse GetRangeModel()
        {
            Ellipse rangeModel = new Ellipse()
            {
                Width = Range * 2,
                Height = Range * 2,
                Fill = Brushes.Red,
                Stroke = Brushes.Red,
                Opacity = .3f,
            };
            return rangeModel;
        }
        public void DisplayRange()
        {
            Canvas.SetLeft(rangeModel, Position.X - rangeModel.Width / 2 + Width / 2);
            Canvas.SetTop(rangeModel, Position.Y - (rangeModel.Height / 2 - Height / 2));
            if (!Game.PlayArea.Children.Contains(rangeModel))
            {
                Game.PlayArea.Children.Add(rangeModel);
            }
            rangeModel.Visibility = Visibility.Visible;
        }
        public override void Destroy()
        {
            OnPositionChanged -= Turret_OnPositionChanged;
            Game.PlayArea.MouseMove -= PlayArea_MouseMove;
            Game.OnGamePaused -= Game_OnGamePaused;
            Game.OnGameUnPaused -= Game_OnGameUnPaused;
            Game.PlayArea.Children.Remove(rangeModel);
            base.Destroy();
        }
        public virtual void Update()
        {
            if (showRange)
            {
                DisplayRange();
            }
            else
            {
                Game.PlayArea.Children.Remove(rangeModel);
            }
            
        }
        public void Place()
        {
            Model.Opacity = 1f;
            OnPositionChanged -= Turret_OnPositionChanged;
            rangeModel.Visibility = Visibility.Hidden;
        }
    }
}
