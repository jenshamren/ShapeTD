using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.Windows.Threading;
using System.Xml;

namespace ShapeTD
{
    enum Direction
    {
        Left,
        Right,
        Down,
        Up
    }
    static class GameHelper
    {
        [DllImport("User32.dll")]
        private static extern bool SetCursorPos(int x, int y);
        public static int TopOfGame { get { return 0; } }
        public static int LeftOfGame { get { return 0; } }
        public static int BottomOfGame { get { return 696; } }
        public static int RightOfGame { get { return 1274; } }
        public static float GetDistance(Point location1, Point location2)
        {
            float dx = (float)location1.X - (float)location2.X;
            float dy = (float)location1.Y - (float)location2.Y;
            return (float)Math.Abs(Math.Sqrt(((dx * dx + dy * dy))));
        }
        public static Vector GetVector(Point curLocation, Point targetLocation)
        {
            double dX = targetLocation.X - curLocation.X;
            double dY = targetLocation.Y - curLocation.Y;
            return new Vector(dX, dY);
        }
        public static Vector GetDirection(Random rand)
        {
            bool flip = rand.Next(1, 3) == 2;
            Vector vector = new Vector();
            if (flip)
            {
                bool both = rand.Next(1, 3) == 2;
                if (both)
                {
                    vector = new Vector(-rand.NextDouble(), -rand.NextDouble());
                }
                else
                {
                    bool left = rand.Next(1, 3) == 2;
                    if (left)
                    {
                        vector = new Vector(-rand.NextDouble(), rand.NextDouble());
                    }
                    else
                    {
                        vector = new Vector(rand.NextDouble(), -rand.NextDouble());
                    }
                }
            }
            else
            {
                vector = new Vector(rand.NextDouble(), rand.NextDouble());
            }
            return vector;
        }
        public static Vector GetDirection(float angle)
        {
            double radians = (angle - 90) * (Math.PI / 180);
            return new Vector(Math.Cos(radians), Math.Sin(radians));
        }
        public static FrameworkElement Clone(FrameworkElement objToClone)
        {
            string stringClone = XamlWriter.Save(objToClone);
            StringReader stringReader = new StringReader(stringClone);
            XmlTextReader xmlReader = new XmlTextReader(stringReader);
            FrameworkElement clone = (FrameworkElement)XamlReader.Load(xmlReader);
            return clone;
        }
        public static float Clamp(float value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }
        public static void SetCursor(int x, int y)
        {
            SetCursorPos(x, y);
        }
        public static UIElement FindCanvasChild(Canvas parent, string tag)
        {
            foreach(var obj in parent.Children)
            {
                FrameworkElement element = obj as FrameworkElement;
                if (element != null && (string)element.Tag == tag)
                {
                    return element;
                }
            }
            return null;
        }
    }
}
