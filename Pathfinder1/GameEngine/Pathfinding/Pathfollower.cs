using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShapeTD
{
    class PathFollower
    {
        private GameController game;
        private List<Point> waypoints;
        private MovableGameObject follower;
        public bool PathEnded { get; private set; }
        public Point CurrentWaypoint { get; private set; }
        public PathFollower(GameController game, MovableGameObject follower)
        {
            this.game = game;
            this.follower = follower;
            Initialize();
        }
        private void Initialize()
        {
            waypoints = new List<Point>();
            game.PathFinder.Path.ForEach(waypoint => waypoints.Add(waypoint));
        }
        public void FollowPath(float speed)
        {
            if (!PathEnded)
            {
                if (waypoints.Count == 0)
                {
                    PathEnded = true;
                    return;
                }
                CurrentWaypoint = waypoints[0];
                if (follower.MoveTowards(CurrentWaypoint, speed))
                {
                    waypoints.Remove(CurrentWaypoint);
                    FollowPath(speed);
                }
            }
        }
    }
}

