using System.Windows;

namespace ShapeTD
{
    interface ICollider
    {
        Point Center { get; }
        Point Position { get; }
        bool IntersectsWith(ICollider collider);
    }
}
