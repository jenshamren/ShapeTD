using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;

namespace ShapeTD
{
    class Grid
    {
        private Node[,] grid;
        private GameController game;
        private int gridSizeX;
        private int gridSizeY;
        private List<IStructure> structuresInGrid;
        public Grid(GameController game)
        {
            this.game = game;
            Initialize();
        }
        private void Initialize()
        {
            gridSizeX = GameHelper.RightOfGame / Node.Size;
            gridSizeY = GameHelper.BottomOfGame / Node.Size;
            structuresInGrid = new List<IStructure>();
            CreateGrid();
        }
        private void CreateGrid()
        {
            grid = new Node[gridSizeX, gridSizeY];
            for (int x = 0; x < gridSizeX; x++)
            {
                for (int y = 0; y < gridSizeY; y++)
                {
                    Point worldPosition = new Point(x * Node.Size, y * Node.Size);
                    grid[x, y] = new Node(game, worldPosition, x, y);
                }
            }
        }
        private Rect GetDownSizedCollider(IStructure obj)
        {
            int edgeCut = 7;
            var transform = obj.Model.TransformToAncestor(game.PlayArea);
            Point leftTop = transform.Transform(new Point(edgeCut, edgeCut));
            Point leftBottom = transform.Transform(new Point(edgeCut, obj.Model.Height - edgeCut));
            Point rightTop = transform.Transform(new Point(obj.Model.Width - edgeCut, edgeCut));
            Point rightBottom = transform.Transform(new Point(obj.Model.Width - edgeCut, obj.Model.Height - edgeCut));
            double minX = Math.Min(Math.Min(leftTop.X, rightTop.X), Math.Min(leftBottom.X, rightBottom.X));
            double minY = Math.Min(Math.Min(leftTop.Y, rightTop.Y), Math.Min(leftBottom.Y, rightBottom.Y));
            double maxX = Math.Max(Math.Max(leftTop.X, rightTop.X), Math.Max(leftBottom.X, rightBottom.X));
            double maxY = Math.Max(Math.Max(leftTop.Y, rightTop.Y), Math.Max(leftBottom.Y, rightBottom.Y));
            return new Rect(minX, minY, (maxX - minX), (maxY - minY));
        }
        private List<Node> GetCoveredNodes(IStructure obj)
        {
            List<Node> coveredNodes = new List<Node>();
            Rect objCollider = GetDownSizedCollider(obj);
            foreach(var node in grid)
            {
                if (node.Collider.IntersectsWith(objCollider))
                {
                    coveredNodes.Add(node);
                }
            }
            return coveredNodes;
        }
        private bool CheckSpaceWalkable(List<Node> nodes)
        {
            foreach(var node in nodes)
            {
                if (!node.Walkable)
                    return false;
            }
            return true;
        }
        public bool SnapObject(IStructure obj)
        {
            if (CheckSpaceWalkable(GetCoveredNodes(obj)) && obj is IStructure)
            {
                Node currentNode = NodeFromWorldPoint(obj.Position);
                double snapDistance = .03;
                Point newObjLocation = new Point(currentNode.Position.X + snapDistance, currentNode.Position.Y + snapDistance);
                obj.MoveToPoint(newObjLocation);
                structuresInGrid.Add(obj);
                UpdateNodes();
                return true;
            }
            return false;
        }
        public void RemoveObject(IStructure obj)
        {
            if(structuresInGrid.Contains(obj))
            {
                structuresInGrid.Remove(obj);
                UpdateNodes();
            }
        }
        public IStructure GridObjectCollision(Rect collider)
        {
            foreach(var obj in structuresInGrid)
            {
                if (collider.IntersectsWith(obj.RectCollider))
                {
                    return obj;
                }
            }
            return null;
        }
        public Node NodeFromWorldPoint(Point worldPosition)
        {
            float percentX = GameHelper.Clamp((float)worldPosition.X / GameHelper.RightOfGame, 0, 1);
            float percentY = GameHelper.Clamp((float)worldPosition.Y / GameHelper.BottomOfGame, 0, 1);
            int nodeX = Convert.ToInt32(gridSizeX * percentX);
            int nodeY = Convert.ToInt32(gridSizeY * percentY);
            if (nodeX == gridSizeX)
                nodeX = nodeX - 1;
            if (nodeY == gridSizeY)
                nodeY = nodeY - 1;
            return grid[nodeX, nodeY];
        }
        public List<Node> GetNeighbours(Node node)
        {
            List<Node> neighbours = new List<Node>();
            for (int x = -1; x <= 1; x++)
            {
                for (int y = -1; y <= 1; y++)
                {
                    if (x == 0 && y == 0)
                    {
                        continue;
                    }
                    int checkX = node.GridX + x;
                    int checkY = node.GridY + y;
                    if (checkX >= 0 && checkX < gridSizeX && checkY >= 0 && checkY < gridSizeY)
                    {
                        neighbours.Add(grid[checkX, checkY]);
                    }
                }
            }
            return neighbours;
        }
        public void UpdateNodes()
        {
            foreach (var node in grid)
            {
                node.SetWalkable();
                if (!node.Walkable)
                {
                    node.Display(Brushes.Red);
                }
            }
        }
    }
}
