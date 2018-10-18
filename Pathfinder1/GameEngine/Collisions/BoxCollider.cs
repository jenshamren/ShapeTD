using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapeTD
{
    struct BoxCollider : ICollider
    {
        public Point Position { get; private set; }
        public Point Center { get; private set; }
        public Point TopLeft { get; private set; }
        public Point TopRight { get; private set; }
        public Point BottomLeft { get; private set; }
        public Point BottomRight { get; private set; }
        public Rect RectCollider { get; private set; }
        public double Height { get; private set; }
        public double Width { get; private set; }
        public double Y { get; private set; }
        public double X { get; private set; }
        public BoxCollider(ICollidableGameObject target, GeneralTransform transform)
        {
            Position = target.Position;
            Y = Position.Y;
            X = Position.X;
            Height = target.Model.Height;
            Width = target.Model.Width;
            Center = transform.Transform(new Point(Width / 2, Height / 2));
            TopLeft = transform.Transform(new Point());
            TopRight = transform.Transform(new Point(target.Model.Width, 0));
            BottomLeft = transform.Transform(new Point(0, target.Model.Height));
            BottomRight = transform.Transform(new Point(target.Model.Width, target.Model.Height));
            float minX = (float)Math.Min(Math.Min(TopLeft.X, TopRight.X), Math.Min(BottomLeft.X, BottomRight.X));
            float minY = (float)Math.Min(Math.Min(TopLeft.Y, TopRight.Y), Math.Min(BottomLeft.Y, BottomRight.Y));
            float maxX = (float)Math.Max(Math.Max(TopLeft.X, TopRight.X), Math.Max(BottomLeft.X, BottomRight.X));
            float maxY = (float)Math.Max(Math.Max(TopLeft.Y, TopRight.Y), Math.Max(BottomLeft.Y, BottomRight.Y));
            RectCollider = new Rect(minX, minY, (maxX - minX), (maxY - minY));
        }
        public BoxCollider(double x, double y, double width, double height)
        {
            Position = new Point(x, y);
            X = x;
            Y = y;
            Width = width;
            Height = height;
            Center = new Point(Position.X + Width / 2, Position.Y + Height / 2);
            TopLeft = Position;
            TopRight = new Point(Position.X + Width, Position.Y);
            BottomLeft = new Point(Position.X, Position.Y + Height);
            BottomRight = new Point(Position.X + Width, Position.Y + Height);
            RectCollider = new Rect(Position.X, Position.Y, Width, Height);
        }
        private bool IntersectsWith(BoxCollider boxCollider)
        {
            return RectCollider.IntersectsWith(boxCollider.RectCollider);
        }
        private bool IntersectsWith(CirlceCollider circleCollider)
        {
            int xDistance = (int)Math.Abs(circleCollider.Center.X - Center.X);
            int yDistance = (int)Math.Abs(circleCollider.Center.Y - Center.Y);
            if (xDistance > (RectCollider.Width / 2 + circleCollider.Radius))
            {
                return false;
            }
            if (yDistance > (RectCollider.Height / 2 + circleCollider.Radius))
            {
                return false;
            }
            if (xDistance <= (RectCollider.Width / 2))
            {
                return true;
            }
            if (yDistance <= (RectCollider.Height / 2))
            {
                return true;
            }
            double dX = xDistance - RectCollider.Width / 2;
            double dY = yDistance - RectCollider.Height / 2;
            double cornerDistance = Math.Sqrt((dX * dX + dY * dY));
            return (cornerDistance <= (circleCollider.Radius * circleCollider.Radius));
        }
        public bool IntersectsWith(ICollider collider)
        {
            if(collider.GetType() == typeof(BoxCollider))
            {
                return IntersectsWith((BoxCollider)collider);
            }
            else if(collider.GetType() == typeof(CirlceCollider))
            {
                return IntersectsWith((CirlceCollider)collider);
            }
            return false;
        }
    }
}
