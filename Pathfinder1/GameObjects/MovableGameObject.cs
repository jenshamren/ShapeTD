
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ShapeTD
{
    abstract class MovableGameObject 
    {
        public delegate void ObjectHandler();
        public event ObjectHandler OnPositionChanged;
        public abstract FrameworkElement Model { get; }
        protected float MoverDeltaTime { get { return Game.DeltaTime / 16; } }
        protected GameController Game { get; private set; }
        public bool Destroyed { get; private set; }
        public virtual Point Position { get; protected set; }
        public virtual double Height
        {
            get { return Model.Height; }
            set { Model.Height = value; }
        }
        public virtual double Width
        {
            get { return Model.Width; }
            set { Model.Width = value; }
        }
        public GeneralTransform Transform { get; set; }
        public MovableGameObject(GameController game)
        {
            Game = game;
            Initialize();
        }
        protected virtual void Game_OnGameUnPaused()
        {
            if(!Game.PlayArea.Children.Contains(Model))
                Game.PlayArea.Children.Add(Model);
        }
        protected virtual void Game_OnGamePaused()
        {
            Game.PlayArea.Children.Remove(Model);
        }
        protected virtual void Initialize()
        {
            Game.OnGamePaused += Game_OnGamePaused;
            Game.OnGameUnPaused += Game_OnGameUnPaused;
        }
        private void SetProperties()
        {
            Position = new Point(Canvas.GetLeft(Model), Canvas.GetTop(Model));
            if(OnPositionChanged != null)
            {
                OnPositionChanged();
            }
        }
        public void Move(Direction direction, float speed)
        {
            if(direction == Direction.Left)
            {
                MoveToPoint(new Point(Position.X - speed, Position.Y));
            }
            else if(direction == Direction.Up)
            {
                MoveToPoint(new Point(Position.X, Position.Y - speed));
            }
            else if(direction == Direction.Right)
            {
                MoveToPoint(new Point(Position.X + speed, Position.Y));
            }
            else if(direction == Direction.Down)
            {
                MoveToPoint(new Point(Position.X, Position.Y + speed));
            }
            SetProperties();
        }
        public void MoveToPoint(Point position)
        {
            Canvas.SetTop(Model, position.Y);
            Canvas.SetLeft(Model, position.X);
            SetProperties();
        }
        public bool MoveTowards(Point target, float speed)
        {
            Vector vector = GameHelper.GetVector(Position, target);
            Vector direction = new Vector(vector.X / vector.Length, vector.Y / vector.Length);
            if (vector.Length > speed)
            {
                Point newMoverPosition = new Point(Position.X + (direction.X * speed), Position.Y + (direction.Y * speed));
                MoveToPoint(newMoverPosition);
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool MoveTowards(Vector direction, float speed)
        {
            Point newMoverPosition = new Point(Position.X + (direction.X * speed), Position.Y + (direction.Y * speed));
            MoveToPoint(newMoverPosition);
            if (OutSideOfGame())
            {
                return true;
            }
            return false;
        }
        public void Rotate(float angle)
        {
            Model.LayoutTransform = new RotateTransform(angle);
            SetProperties();
        }
        public virtual void Rotate(float angle, double centerX, double centerY)
        {
            Model.RenderTransform = new RotateTransform(angle, centerX, centerY);
            SetProperties();
        }
        protected bool OutSideOfGame()
        {
            if (Position.X < GameHelper.LeftOfGame || Position.X > GameHelper.RightOfGame || Position.Y < GameHelper.TopOfGame - Model.Height ||
                Position.Y > GameHelper.BottomOfGame)
            {
                return true;
            }
            return false;
        }
        public float LookAt(Point target)
        {
            Vector vector = GameHelper.GetVector(Position, target);
            float angle = (float)Math.Atan2(vector.Y, vector.X) * (float)(180 / Math.PI) + 90;
            return angle;
        }
        public float LookAt(Vector direction)
        {
            float angle = (float)Math.Atan2(direction.Y, direction.X) * (float)(180 / Math.PI) + 90;
            return angle;
        }
        public virtual void Destroy()
        {
            Destroyed = true;
            Game.RemoveGameObject(this);
            Game.OnGamePaused -= Game_OnGamePaused;
            Game.OnGameUnPaused -= Game_OnGameUnPaused;
        }
    }
}
