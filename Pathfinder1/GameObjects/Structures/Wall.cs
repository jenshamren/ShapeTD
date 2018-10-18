using System;
using System.Collections.Generic;
using System.Diagnostics;
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
    enum SnapAlignment
    {
        RightBottom,
        TopLeft
    }
    class Wall : MovableGameObject, IStructure
    {
        private Rectangle wallModel;
        public int Cost { get { return GetCost(); } }
        public bool Vertical { get; private set; }
        public Node LeftSideNode { get; private set; }
        public Node RightSideNode { get; private set; }
        public Point Center { get; private set; }
        public bool SnappedToEdge { get; set; }
        public Point SnapPosition { get; set; }
        public SnapAlignment? SnapAlignment { get; set; }
        public override FrameworkElement Model
        {
            get
            {
                return wallModel;
            }
        }
        public Rect RectCollider { get; set; }
        public Wall(GameController game) : base(game)
        {

        }
        protected override void Initialize()
        {
            base.Initialize();
            wallModel = (Rectangle)(GameHelper.Clone(Game.GameObjectModels["wallBody"]));
            Model.Opacity = .7f;
            Center = new Point(Position.X + Width / 2, Position.Y + Height / 2);
            OnPositionChanged += On_PositionChanged;
        }
        private void On_PositionChanged()
        {
            if (Vertical)
            {
                Center = new Point(Position.X + Height / 2, Position.Y + Width / 2);
                //top
                LeftSideNode = Game.Grid.NodeFromWorldPoint(new Point(Position.X, Position.Y));
                //bottom
                RightSideNode = Game.Grid.NodeFromWorldPoint(new Point(Position.X, Position.Y + Width - Node.Size));
                RectCollider = new Rect(Position.X, Position.Y, Height, Width);
            }
            else
            {
                Center = new Point(Position.X + Width / 2, Position.Y + Height / 2);
                LeftSideNode = Game.Grid.NodeFromWorldPoint(new Point(Position.X, Position.Y));
                RightSideNode = Game.Grid.NodeFromWorldPoint(new Point(Position.X + Width - Node.Size, Position.Y));
                RectCollider = new Rect(Position.X, Position.Y, Width, Height);
            }
        }
        protected override void Game_OnGamePaused()
        {
            return;
        }
        protected override void Game_OnGameUnPaused()
        {
            return;
        }
        public new void Rotate(float angle)
        {
            if(angle == 90 || angle == 0)
            {
                if (angle == 90f)
                {
                    Vertical = true;
                }
                else
                {
                    Vertical = false;
                }
                base.Rotate(angle);
            }
        }
        public void Place()
        {
            Model.Opacity = 1f;
            OnPositionChanged -= On_PositionChanged;
        }
        public override void Destroy()
        {
            base.Destroy();
            OnPositionChanged -= On_PositionChanged;
        }
        public static int GetCost()
        {
            return 30;
        }
    }
}
