using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapeTD
{
    class Node
    {
        public Rectangle outline;
        private GameController game;
        private IStructure structureInsideNode;
        public static int Size { get { return 24; } }
        public int FCost { get { return GCost + HCost; } }
        public Point Position { get; set; }
        public Rect Collider { get; set; }
        public int GridX { get; private set; }
        public int GridY { get; private set; }
        public bool Walkable { get; private set; }
        public int GCost { get; set; }
        public int HCost { get; set; }
        public Node Parent { get; set; }
        public Node(GameController game, Point location, int gridX, int gridY)
        {
            this.game = game;
            Position = location;
            GridX = gridX;
            GridY = gridY;
            Initialize();
        }
        private void Initialize()
        {
            outline = new Rectangle() { Width = Size, Height = Size };
            Collider = new Rect(Position.X, Position.Y, Size, Size);
            Walkable = true;
            game.PlayArea.Children.Add(outline);
        }
        public void SetWalkable()
        {
            IStructure collidingStructure = game.Grid.GridObjectCollision(Collider);
            if(collidingStructure != null)
            {
                structureInsideNode = collidingStructure;
                Walkable = false;
            }
            else
            {
                structureInsideNode = null;
                outline.Visibility = Visibility.Hidden;
                Walkable = true;
            }
        }
        public void Display(Brush color)
        {
            Canvas.SetLeft(outline, Position.X);
            Canvas.SetTop(outline, Position.Y);
            outline.Visibility = Visibility.Visible;
            outline.Fill = color;
        }
        public IStructure GetStructure()
        {
            return structureInsideNode;
        }
    }
}
