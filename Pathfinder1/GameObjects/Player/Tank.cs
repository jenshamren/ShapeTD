using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;

namespace ShapeTD
{
    class Tank : MovableGameObject, IUpdate
    {
        private Canvas tankModel;
        private TankMachineGun machineGun;
        private TankMissileLauncher missileLauncher;
        private bool moveRight;
        private bool moveLeft;
        private float moveSpeed;
        private Direction currentDirection;
        private const float maxSpeed = 15f;
        private const float minSpeed = 1f;
        private const float speedScaling = 1.04f;
        public int HitPoints { get; set; }
        public override FrameworkElement Model
        {
            get
            {
                return tankModel;
            }
        }
        public Tank(GameController game) : base(game)
        {

        }
        protected override void Initialize()
        {
            base.Initialize();
            moveSpeed = minSpeed;
            tankModel = (Canvas)GameHelper.Clone(Game.GameObjectModels["tankModel"]);
            machineGun = new TankMachineGun(Game, this);
            missileLauncher = new TankMissileLauncher(Game, this);
            Position = new Point(Canvas.GetLeft(Model), Canvas.GetTop(Model));
            StartEventListeners();
            HitPoints = 10;
            Panel.SetZIndex(Model, 1);
        }
        private void Form_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.E)
            {
                moveRight = false;
                moveSpeed = minSpeed;
            }
            if (e.Key == Key.Q)
            {
                moveLeft = false;
                moveSpeed = minSpeed;
            }

        }
        private void Form_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.E)
            {
                moveRight = true;
            }
            if (e.Key == Key.Q)
            {
                moveLeft = true;
            }
        }
        private void PlayArea_MouseRightButtonUp(object sender, MouseButtonEventArgs e)
        {
            missileLauncher.Fire = false;
        }
        private void PlayArea_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            missileLauncher.Fire = true;
        }
        private void PlayArea_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            machineGun.Fire = false;
        }
        private void PlayArea_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            machineGun.Fire = true;
        }
        protected override void Game_OnGamePaused()
        {
            base.Game_OnGamePaused();
            StopEventListeners();
        }
        protected override void Game_OnGameUnPaused()
        {
            base.Game_OnGameUnPaused();
            StartEventListeners();
        }
        private void MoveLeft()
        {
            if (Position.X > GameHelper.LeftOfGame)
            {
                if (currentDirection == Direction.Left)
                    IncreaseSpeed();
                Move(Direction.Left, moveSpeed);
                currentDirection = Direction.Left;
            }
        }
        private void IncreaseSpeed()
        {
            if (moveSpeed < maxSpeed)
            {
                moveSpeed *= speedScaling;
            }
        }
        private void MoveRight()
        {
            if (Position.X < GameHelper.RightOfGame - Model.ActualWidth)
            {
                if (currentDirection == Direction.Right)
                    IncreaseSpeed();
                Move(Direction.Right, moveSpeed);
                currentDirection = Direction.Right;
            }
        }
        private void StopEventListeners()
        {
            Game.PlayArea.MouseRightButtonUp -= PlayArea_MouseRightButtonUp;
            Game.PlayArea.MouseRightButtonDown -= PlayArea_MouseRightButtonDown;
            Game.PlayArea.MouseLeftButtonUp -= PlayArea_MouseLeftButtonUp;
            Game.PlayArea.MouseLeftButtonDown -= PlayArea_MouseLeftButtonDown;
            Game.Form.KeyDown -= Form_KeyDown;
            Game.Form.KeyUp -= Form_KeyUp;
        }
        private void StartEventListeners()
        {
            Game.PlayArea.MouseRightButtonUp += PlayArea_MouseRightButtonUp;
            Game.PlayArea.MouseRightButtonDown += PlayArea_MouseRightButtonDown;
            Game.PlayArea.MouseLeftButtonUp += PlayArea_MouseLeftButtonUp;
            Game.PlayArea.MouseLeftButtonDown += PlayArea_MouseLeftButtonDown;
            Game.Form.KeyDown += Form_KeyDown;
            Game.Form.KeyUp += Form_KeyUp;
        }
        private void UpdateWeapons()
        {
            machineGun.Update();
            missileLauncher.Update();
        }
        public void Update()
        {
            if (moveLeft)
            {
                MoveLeft();
            }
            if (moveRight)
            {
                MoveRight();
            }
            UpdateWeapons();
            if(HitPoints <= 0)
            {
                Game.GameOver = true;
            }
        }
    }
}
