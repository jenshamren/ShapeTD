using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace ShapeTD
{
    class MazeBuilder
    {
        private GameController game;
        private IStructure currentStructure;
        private Stack<IStructure> placedStructures;
        private Canvas buyWallModel;
        private Canvas buyWaterTurretModel;
        private Canvas regretPurchaseModel;
        private Point cursorPosition;
        public bool IsRunning { get; private set; }
        public MazeBuilder(GameController game)
        {
            this.game = game;
            placedStructures = new Stack<IStructure>();
            Initialize();
        }
        private void BuyWaterTurretModel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(currentStructure == null && game.PlayerCash >= WaterTurret.GetCost())
            {
                currentStructure = PurchaseStructure(typeof(WaterTurret));
            }
        }
        private void BuyWallModel_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if(currentStructure == null && game.PlayerCash >= Wall.GetCost())
            {
                currentStructure = PurchaseStructure(typeof(Wall));
            }
        }
        private void RegretPurchaseModel_MouseDown(object sender, MouseButtonEventArgs e)
        {
            RegretPurchase();
        }
        private void PlayArea_MouseMove(object sender, MouseEventArgs e)
        {
            cursorPosition = e.GetPosition(game.PlayArea);
            if (currentStructure != null)
            {
                if (currentStructure.GetType() == typeof(Wall))
                {
                    MoveWall(currentStructure as Wall);
                }
                else if (currentStructure != null && currentStructure is Turret)
                {
                    MoveTurret(currentStructure as Turret);
                }
            }
        }
        private void currentStructure_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (currentStructure.GetType() == typeof(Wall) && PlaceWall(currentStructure as Wall) || currentStructure is Turret && game.Grid.SnapObject(currentStructure))
            {
                currentStructure.Place();
                currentStructure.Model.MouseDown -= currentStructure_MouseDown;
                placedStructures.Push(currentStructure);
                if(game.PlayerCash >= currentStructure.Cost)
                {
                    currentStructure = PurchaseStructure(currentStructure.GetType());
                }
                else
                {
                    currentStructure = null;
                }
            }
        }
        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (currentStructure == null && e.Key == Key.D && game.PlayerCash >= Wall.GetCost())
            {
                currentStructure = PurchaseStructure(typeof(Wall));
            }
            if (currentStructure == null && e.Key == Key.F && game.PlayerCash >= WaterTurret.GetCost())
            {
                currentStructure = PurchaseStructure(typeof(WaterTurret));
            }
            if (e.Key == Key.B)
            {
                RegretPurchase();
            }
            if (e.Key == Key.Tab)
            {
                RotateWall(currentStructure as Wall);
            }
        }
        private void Initialize()
        {
            buyWallModel = (Canvas)game.GameObjectModels["buyWallModel"];
            buyWaterTurretModel = (Canvas)game.GameObjectModels["buyWaterTurretModel"];
            regretPurchaseModel = (Canvas)game.GameObjectModels["regretPurchaseModel"];
            var buyWallText = (TextBlock)GameHelper.FindCanvasChild(buyWallModel, "buyWallTextBlock");
            var buyWTurretText = (TextBlock)GameHelper.FindCanvasChild(buyWaterTurretModel, "buyWaterTurretTextBlock");
            buyWallText.Text = buyWallText.Text + Wall.GetCost().ToString();
            buyWTurretText.Text = buyWTurretText.Text + WaterTurret.GetCost().ToString();
        }
        private void StartEventListeners()
        {
            game.Form.KeyDown += Form_KeyDown;
            game.PlayArea.MouseMove += PlayArea_MouseMove;
            buyWallModel.MouseLeftButtonDown += BuyWallModel_MouseLeftButtonDown;
            buyWaterTurretModel.MouseLeftButtonDown += BuyWaterTurretModel_MouseLeftButtonDown;
            regretPurchaseModel.MouseDown += RegretPurchaseModel_MouseDown;
        }
        private void StopEventListeners()
        {
            game.Form.KeyDown -= Form_KeyDown;
            game.PlayArea.MouseMove -= PlayArea_MouseMove;
            if (currentStructure != null)
            {
                currentStructure.Model.MouseDown -= currentStructure_MouseDown;
            }
            buyWallModel.MouseLeftButtonDown -= BuyWallModel_MouseLeftButtonDown;
            buyWaterTurretModel.MouseLeftButtonDown -= BuyWaterTurretModel_MouseLeftButtonDown;
            regretPurchaseModel.MouseDown -= RegretPurchaseModel_MouseDown;
        }
        private void DisplayMazeBuilderObjects()
        {
            game.PlayArea.Children.Add(buyWallModel);
            game.PlayArea.Children.Add(buyWaterTurretModel);
            game.PlayArea.Children.Add(regretPurchaseModel);
            game.PlayArea.Children.Remove(game.GameObjectModels["playerHealthModel"]);
        }
        private void HideModels()
        {
            game.PlayArea.Children.Remove(buyWallModel);
            game.PlayArea.Children.Remove(buyWaterTurretModel);
            game.PlayArea.Children.Remove(regretPurchaseModel);
        }
        private void MoveTurret(Turret turret)
        {
            turret.MoveToPoint(GetCenterToCursor(turret));
            turret.DisplayRange();
        }
        private void MoveWall(Wall wall)
        {
            var nearbyEdge = GetMoveAlongBorderResult(wall);
            if (nearbyEdge != null)
            {
                wall.MoveToPoint(nearbyEdge.Item1);
                wall.SnapAlignment = nearbyEdge.Item2;
                wall.SnappedToEdge = true;
            }
            else
            {
                wall.MoveToPoint(GetCenterToCursor(wall));
                wall.SnappedToEdge = false;
            }
        }
        private void RotateWall(Wall wall)
        {
            if (wall != null)
            {
                if (!wall.Vertical)
                {
                    wall.Rotate(90f);
                }
                else
                {
                    wall.Rotate(0f);
                }
                var relativePosition = App.Current.MainWindow.PointToScreen(wall.Center);
                GameHelper.SetCursor((int)relativePosition.X, (int)relativePosition.Y);
            }
        }
        private void RemoveStructure(IStructure structure)
        {
            structure.Destroy();
            game.Grid.RemoveObject(structure);
        }
        private void MoveToPreviousWall(Wall currentWall, Wall previousWall)
        {
            if (currentWall != null && previousWall != null)
            {
                currentWall.MoveToPoint(previousWall.SnapPosition);
                if (previousWall.Vertical)
                {
                    if (currentWall != null)
                    {
                        currentWall.Rotate(90f);
                    }
                }
            }
        }
        private bool PlaceWall(Wall wall)
        {
            if (!wall.SnappedToEdge)
            {
                wall.SnapAlignment = GetSnapAlignment(wall);
            }
            if (wall.SnapAlignment != null && game.Grid.SnapObject(currentStructure))
            {
                wall.SnapPosition = GetSnapPosition(wall).Value;
                return true;
            }
            else
            {
                Console.WriteLine("invalid location");
            }
            return false;
        }
        private void RegretPurchase()
        {
            if (currentStructure != null)
            {
                game.PlayerCash += currentStructure.Cost;
                RemoveStructure(currentStructure);
                currentStructure = null;
            }
            else if (placedStructures.Count > 0)
            {
                var previousStructure = placedStructures.Pop();
                game.PlayerCash += previousStructure.Cost;
                RemoveStructure(previousStructure);
            }
        }
        private Wall CreateWall()
        {
            Wall wall = new Wall(game);
            if (placedStructures.Count > 0)
            {
                var previousWall = placedStructures.Peek() as Wall;
                MoveToPreviousWall(wall, previousWall);
                Point relativePoint = App.Current.MainWindow.PointToScreen(wall.Center);
                GameHelper.SetCursor((int)relativePoint.X, (int)relativePoint.Y);
            }
            else
            {
                wall.MoveToPoint(GetCenterToCursor(wall));
            }
            wall.Model.MouseDown += currentStructure_MouseDown;
            return wall;
        }
        private IStructure PurchaseStructure(Type structureType)
        {
            if (structureType == typeof(Wall))
            {
                Wall wall = CreateWall();
                game.AddGameObject(wall as MovableGameObject);
                game.PlayerCash -= wall.Cost;
                return wall;
            }
            else if (structureType == typeof(WaterTurret))
            {
                WaterTurret turret = new WaterTurret(game);
                turret.Model.MouseDown += currentStructure_MouseDown;
                turret.MoveToPoint(GetCenterToCursor(turret));
                game.AddGameObject(turret);
                game.PlayerCash -= turret.Cost;
                return turret;
            }
            return null;
        }
        private Point GetCenterToCursor(IStructure objToCenter)
        {
            double centerX = objToCenter.Center.X - objToCenter.Position.X;
            double centerY = objToCenter.Center.Y - objToCenter.Position.Y;
            return new Point(cursorPosition.X - centerX, cursorPosition.Y - centerY);
        }
        private Tuple<Point, SnapAlignment> GetMoveAlongBorderResult(Wall wall)
        {
            double actualWidth = (wall.Center.X - wall.Position.X) * 2;
            double actualHeight = (wall.Center.Y - wall.Position.Y) * 2;
            double maxDistance = 22;
            Point leftBorderPos = new Point(GameHelper.LeftOfGame, cursorPosition.Y - actualHeight / 2);
            Point topBorderPos = new Point(cursorPosition.X - actualWidth / 2, GameHelper.TopOfGame);
            Point rightBorderPos = new Point(GameHelper.RightOfGame - actualWidth, cursorPosition.Y - actualHeight / 2);
            if (GameHelper.GetDistance(leftBorderPos, new Point(cursorPosition.X - (actualWidth / 2), cursorPosition.Y - (actualHeight / 2))) < maxDistance)
            {
                return new Tuple<Point, SnapAlignment>(leftBorderPos, SnapAlignment.RightBottom);
            }
            if (GameHelper.GetDistance(topBorderPos, new Point(cursorPosition.X - (actualWidth / 2), cursorPosition.Y - (actualHeight / 2))) < maxDistance)
            {
                return new Tuple<Point, SnapAlignment>(topBorderPos, SnapAlignment.RightBottom);
            }
            if (GameHelper.GetDistance(rightBorderPos, new Point(cursorPosition.X - (actualWidth / 2), cursorPosition.Y - (actualHeight / 2))) < maxDistance)
            {
                return new Tuple<Point, SnapAlignment>(rightBorderPos, SnapAlignment.TopLeft);
            }
            return null;
        }
        private SnapAlignment? GetSnapAlignment(Wall wall)
        {
            Wall leftSideWall = FindSnappableWall(wall.LeftSideNode);
            if (leftSideWall != null)
            {
                return SnapAlignment.RightBottom;
            }
            else
            {
                Wall rightSideWall = FindSnappableWall(wall.RightSideNode);
                if (rightSideWall != null)
                {
                    return SnapAlignment.TopLeft;
                }
            }
            return null;
        }
        private Wall FindSnappableWall(Node targetNode)
        {
            List<Node> neighbours = game.Grid.GetNeighbours(targetNode);
            foreach (var node in neighbours)
            {
                int xDistance = Math.Abs(node.GridX - targetNode.GridX);
                int yDistance = Math.Abs(node.GridY - targetNode.GridY);
                if ((xDistance + yDistance) > 1)
                {
                    continue;
                }
                else
                {
                    Wall snappableWall = node.GetStructure() as Wall;
                    if (snappableWall != null)
                    {
                        return snappableWall;
                    }
                }
            }
            return null;
        }
        private Point? GetSnapPosition(Wall wall)
        {
            switch (wall.SnapAlignment)
            {
                case SnapAlignment.RightBottom:
                    if (wall.Vertical)
                    {
                        return new Point(wall.Position.X, wall.Position.Y + wall.Width);
                    }
                    else
                    {
                        return new Point(wall.Position.X + wall.Width, wall.Position.Y);
                    }
                case SnapAlignment.TopLeft:
                    if (wall.Vertical)
                    {
                        return new Point(wall.Position.X, wall.Position.Y - wall.Width);
                    }
                    else
                    {
                        return new Point(wall.Position.X - wall.Width, wall.Position.Y);
                    }
            }
            return null;
        }
        public void Reset()
        {
            foreach(var obj in placedStructures)
            {
                obj.Destroy();
                game.Grid.RemoveObject(obj);
            }
            placedStructures = new Stack<IStructure>();
        }
        public void Stop()
        {
            if (currentStructure != null)
            {
                currentStructure.Destroy();
                currentStructure = null;
            }
            HideModels();
            StopEventListeners();
            game.PlayArea.Children.Add(game.GameObjectModels["playerHealthModel"]);
            IsRunning = false;
        }
        public void Start()
        {
            DisplayMazeBuilderObjects();
            StartEventListeners();
            IsRunning = true;
        }
    }
}
