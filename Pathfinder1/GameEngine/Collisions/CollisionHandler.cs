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
    class CollisionHandler
    {
        private GameController game;
        public List<ICollidableGameObject> ObjectsInRange { get; private set; }
        public List<ICollidableGameObject> CollidingObjects { get; private set; }
        public ICollidableGameObject CollidingObject { get; private set; }
        public CollisionHandler(GameController game)
        {
            this.game = game;
        }
        private List<ICollidableGameObject> CheckCollision(List<ICollidableGameObject> objects, ICollidableGameObject testingObject)
        {
            List<ICollidableGameObject> collidingObjects = new List<ICollidableGameObject>();
            foreach(var obj in objects)
            {
                if (obj.Collider.IntersectsWith(testingObject.Collider))
                {
                    collidingObjects.Add(obj);
                }
            }
            return collidingObjects;
        }
        public bool Collision(ICollidableGameObject testingObject)
        {
            List<ICollidableGameObject> objectsInRange = game.QuadTree.FindObjectsInRange(testingObject);
            ObjectsInRange = objectsInRange;
            CollidingObjects = CheckCollision(objectsInRange, testingObject);
            if (CollidingObjects.Count > 0)
            {
                CollidingObject = CollidingObjects[0];
                return true;
            }
            return false;
        }
    }
}

