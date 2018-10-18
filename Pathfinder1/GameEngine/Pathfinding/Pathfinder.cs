using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ShapeTD
{
    class PathFinder
    {
        private Grid grid;
        private Node targetNode;
        public List<Point> Path { get; private set; }
        public PathFinder(Grid grid)
        {
            this.grid = grid;
            Initialize();
        }
        private void Initialize()
        {
            Path = new List<Point>();
        }
        private void RetracePath(Node startNode, Node endNode)
        {
            List<Node> nodePath = new List<Node>();
            Node currentNode = endNode;
            while (currentNode != startNode)
            {
                nodePath.Add(currentNode);
                currentNode = currentNode.Parent;
            }
            nodePath.Reverse();
            Path = new List<Point>();
            nodePath.ForEach(node => Path.Add(node.Position));
        }
        private int GetDistance(Node node1, Node node2)
        {
            int distanceX = Math.Abs(node1.GridX - node2.GridX);
            int distanceY = Math.Abs(node1.GridY - node2.GridY);
            if (distanceX > distanceY)
            {
                return 14 * distanceY + 10 * (distanceX - distanceY);
            }
            return (14 * distanceX) + 10 * (distanceY - distanceX);
        }
        public void FindPath(Point startPosition, Point targetPosition)
        {
            List<Node> openSet = new List<Node>();
            List<Node> closedSet = new List<Node>();
            Node startNode = grid.NodeFromWorldPoint(startPosition);
            targetNode = grid.NodeFromWorldPoint(targetPosition);
            openSet.Add(startNode);
            while(openSet.Count > 0)
            {
                Node currentNode = openSet[0];
                for (int i = 1; i < openSet.Count; i++)
                {
                    if (openSet[i].FCost < currentNode.FCost || openSet[i].FCost == currentNode.FCost && openSet[i].HCost < currentNode.HCost)
                    {
                        currentNode = openSet[i];
                    }
                }
                openSet.Remove(currentNode);
                closedSet.Add(currentNode);
                if(currentNode == targetNode)
                {
                    RetracePath(startNode, targetNode);
                    return;
                }
                foreach(var neighbour in grid.GetNeighbours(currentNode))
                {
                    if(!neighbour.Walkable || closedSet.Contains(neighbour))
                    {
                        continue;
                    }
                    int newNeighbourGCost = currentNode.GCost + GetDistance(currentNode, neighbour);
                    if(newNeighbourGCost < neighbour.GCost || !openSet.Contains(neighbour))
                    {
                        neighbour.GCost = newNeighbourGCost;
                        neighbour.HCost = GetDistance(neighbour, targetNode);
                        neighbour.Parent = currentNode;
                        if (!openSet.Contains(neighbour))
                        {
                            openSet.Add(neighbour);
                        }
                    }
                }
            }
            Path = new List<Point>();
        }
    }
}
