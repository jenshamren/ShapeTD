using System.Windows;
using System.Windows.Media;

namespace ShapeTD
{
    interface ICollidableGameObject
    {
        ICollider Collider { get; set; }
        FrameworkElement Model { get; }
        Point Position { get; }
        GeneralTransform Transform { get; set; }
    }
}
