
using System.Windows;

namespace ShapeTD
{
    interface IStructure
    {
        int Cost { get; }
        Rect RectCollider { get; }
        FrameworkElement Model { get; }
        Point Center { get; }
        Point Position { get; }
        double Width { get; set; }
        double Height { get; set; }
        void Place();
        void MoveToPoint(Point point);
        void Destroy();
    }
}
