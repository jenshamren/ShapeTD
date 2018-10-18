using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapeTD
{
    class QuadTree
    {
        private enum QuadPosition
        {
            NorthWest,
            SouthWest,
            NorthEast,
            SouthEast
        }
        private const int maxCapacity = 5;
        private List<ICollidableGameObject> objectsInQuad;
        private GameController game;
        private QuadTree northWest;
        private QuadTree northEast;
        private QuadTree southWest;
        private QuadTree southEast;
        private bool divided;
        private BoxCollider Boundary { get; set; }
        public QuadTree(BoxCollider boundary, GameController game)
        {
            this.game = game;
            Boundary = boundary;
            objectsInQuad = new List<ICollidableGameObject>();
            divided = false;
        }
        private void Subdivide()
        {
            BoxCollider northWestBoundary = new BoxCollider(Boundary.X, Boundary.Y, Boundary.Width / 2, Boundary.Height / 2);
            BoxCollider northEastBoundary = new BoxCollider(Boundary.X + Boundary.Width / 2, Boundary.Y, Boundary.Width / 2, Boundary.Height / 2);
            BoxCollider southWestBoundary = new BoxCollider(Boundary.X, Boundary.Y + Boundary.Height / 2, Boundary.Width / 2, Boundary.Height / 2);
            BoxCollider southEastBoundary = new BoxCollider(Boundary.X + Boundary.Width / 2, Boundary.Y + Boundary.Height / 2, Boundary.Width / 2, Boundary.Height / 2);
            northWest = new QuadTree(northWestBoundary, game);
            northEast = new QuadTree(northEastBoundary, game);
            southWest = new QuadTree(southWestBoundary, game);
            southEast = new QuadTree(southEastBoundary, game);
            divided = true;
        }
        public bool Insert(ICollidableGameObject obj)
        {
            if (!Boundary.IntersectsWith(obj.Collider))
            {
                return false;
            }
            if(objectsInQuad.Count < maxCapacity)
            {
                objectsInQuad.Add(obj);
                return true;
            }
            if (!divided)
            {
                Subdivide();
            }
            if (northWest.Insert(obj))
                return true;
            if (northEast.Insert(obj))
                return true;
            if (southWest.Insert(obj))
                return true;
            if (southEast.Insert(obj))
                return true;
            return false;
        }
        public List<ICollidableGameObject> FindObjectsInRange(ICollidableGameObject range)
        {
            List<ICollidableGameObject> objectsInRange = new List<ICollidableGameObject>();
            if (!Boundary.IntersectsWith(range.Collider))
            {
                return objectsInRange;
            }
            objectsInRange.AddRange(objectsInQuad);
            if (divided)
            {
                objectsInRange.AddRange(northWest.FindObjectsInRange(range));
                objectsInRange.AddRange(northEast.FindObjectsInRange(range));
                objectsInRange.AddRange(southWest.FindObjectsInRange(range));
                objectsInRange.AddRange(southEast.FindObjectsInRange(range));
            }
            return objectsInRange;
        }
        public void Reset()
        { 
            objectsInQuad = new List<ICollidableGameObject>();
            if (divided)
            {
                northWest.Reset();
                northEast.Reset();
                southWest.Reset();
                southEast.Reset();
                northWest = null;
                northEast = null;
                southWest = null;
                southEast = null;
                divided = false;
            }
        }
    }
}
