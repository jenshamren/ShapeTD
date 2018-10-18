using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapeTD
{
    struct CirlceCollider : ICollider
    {
        public Point Center { get; private set; }
        public double Radius { get; private set; }
        public Point Position { get; private set; }
        public CirlceCollider(ICollidableGameObject target, GeneralTransform transform, Point centerOfCollider, double radius)
        {
            Center = centerOfCollider;
            Position = Center;
            Radius = radius;
        }
        public bool IntersectsWith(BoxCollider boxCollider)
        {
            int xDistance = (int)Math.Abs(Center.X - boxCollider.Center.X);
            int yDistance = (int)Math.Abs(Center.Y - boxCollider.Center.Y);
            if (xDistance > (boxCollider.Width / 2 + Radius))
            {
                return false;
            }
            if (yDistance > (boxCollider.Height / 2 + Radius))
            {
                return false;
            }
            if (xDistance <= (boxCollider.Width / 2))
            {
                return true;
            }
            if (yDistance <= (boxCollider.Height / 2))
            {
                return true;
            }
            float dX = (float)(xDistance - (boxCollider.Width / 2));
            float dY = (float)(yDistance - (boxCollider.Height / 2));
            float cornerDistance = (float)Math.Sqrt((dX * dX + dY * dY));
            return (cornerDistance <= (Radius * Radius));
        }
        public bool IntersectsWith(CirlceCollider circleCollider)
        {
            float radius = (float)(Radius + circleCollider.Radius);
            float deltaX = (float)(Center.X - circleCollider.Center.X);
            float deltaY = (float)(Center.Y - circleCollider.Center.Y);
            return deltaX * deltaX + deltaY * deltaY <= radius * radius;
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
