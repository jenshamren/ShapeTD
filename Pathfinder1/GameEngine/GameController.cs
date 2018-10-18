using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ShapeTD
{
    class GameController
    {
        public delegate void GameEventHandler();
        public event GameEventHandler OnGamePaused;
        public event GameEventHandler OnGameUnPaused;
        private List<IUpdate> updatingObjects;
        private List<ICollidableGameObject> collidableGameObjects;
        private List<Enemy> enemies;
        private Canvas playPauseButton;
        private TextBlock playerGoldText;
        private MazeBuilder mazeBuilder;
        private TextBlock playerHealthText;
        private TextBlock currentLevelText;
        private DateTime currentTime;
        private int playerCash;
        private int wavesCleared;
        private const int startingCash = 600;
        public Canvas PlayArea { get; private set; }
        public MainWindow Form { get; private set; }
        public Random Rand { get; private set; }
        public Grid Grid { get; private set; }
        public PathFinder PathFinder { get; private set; }
        public QuadTree QuadTree { get; private set; }
        public ExplosionParticlePool ExplosionParticlePool { get; private set; }
        public Tank PlayerTank { get; private set; }
        public Dictionary<string, FrameworkElement> GameObjectModels { get; private set; }
        public Point EnemySpawnPosition { get; private set; }
        public Point EnemyTargetPosition { get; private set; }
        public float DeltaTime { get; private set; }
        public WaveSpawner WaveSpawner { get; private set; }
        public bool GameOver { get; set; }
        public int PlayerCash
        {
            get
            {
                return playerCash;
            }
            set
            {
                playerCash = value;
                playerGoldText.Text = playerCash.ToString();
            }
        }
        public GameController(Canvas playArea, MainWindow form, Dictionary<string, FrameworkElement> gameModels)
        {
            PlayArea = playArea;
            Form = form;
            GameObjectModels = gameModels;
            Initialize();
        }
        private void Initialize()
        {
            InitializeObjects();
            LoadModels();
            StartEventListeners();
            PlayerCash = startingCash;
            UpdateTextBlocks();
            Form.WindowStyle = WindowStyle.SingleBorderWindow;
            Pause();
            mazeBuilder.Start();
        }
        private void Form_Loaded(object sender, RoutedEventArgs e)
        {
            QuadTree = new QuadTree(new BoxCollider(0, 0, PlayArea.ActualWidth, PlayArea.ActualHeight), this);
            Form.Loaded -= Form_Loaded;
        }
        private void CompositionTarget_Rendering(object sender, EventArgs e)
        {
            UpdateGame();
        }
        private void PlayPauseButton_MouseDown(object sender, MouseButtonEventArgs e)
        {
            Shape playButton = (Shape)GameHelper.FindCanvasChild(playPauseButton, "playButtonModel");
            Canvas pauseButton = (Canvas)GameHelper.FindCanvasChild(playPauseButton, "pauseButtonModel");
            if (playButton.Visibility == Visibility.Visible)
            {
                if (mazeBuilder.IsRunning)
                {
                    mazeBuilder.Stop();
                    Grid.UpdateNodes();
                    PathFinder.FindPath(EnemySpawnPosition, EnemyTargetPosition);
                }
                if(PathFinder.Path.Count == 0)
                {
                    MessageBox.Show("No path  was found.", "Path not found!");
                    mazeBuilder.Start();
                    return;
                }
                WaveSpawner.Start(wavesCleared);
                UnPause();
                playButton.Visibility = Visibility.Hidden;
                pauseButton.Visibility = Visibility.Visible;
            }
            else if (pauseButton.Visibility == Visibility.Visible)
            {
                Pause();
                playButton.Visibility = Visibility.Visible;
                pauseButton.Visibility = Visibility.Hidden;
            }
        }
        private void WaveSpawner_OnWaveCleared()
        {
            Pause();
            mazeBuilder.Start();
            ShowPlayButton();
            wavesCleared++;
        }
        private void InitializeObjects()
        {
            collidableGameObjects = new List<ICollidableGameObject>();
            updatingObjects = new List<IUpdate>();
            enemies = new List<Enemy>();
            Grid = new Grid(this);
            Rand = new Random();
            PlayerTank = new Tank(this);
            AddGameObject(PlayerTank);
            PathFinder = new PathFinder(Grid);
            mazeBuilder = new MazeBuilder(this);
            WaveSpawner = new WaveSpawner(this);
            EnemySpawnPosition = new Point(GameHelper.LeftOfGame, GameHelper.TopOfGame);
            EnemyTargetPosition = new Point(GameHelper.RightOfGame, GameHelper.BottomOfGame);
            ExplosionParticlePool = new ExplosionParticlePool(this, 400);
        }
        private void LoadModels()
        {
            Canvas playerGold = (Canvas)GameObjectModels["playerGoldModel"];
            playerGoldText = (TextBlock)GameHelper.FindCanvasChild(playerGold, "playerGoldTextBlock");
            PlayArea.Children.Add(playerGold);
            currentLevelText = (TextBlock)GameObjectModels["levelText"];
            PlayArea.Children.Add(currentLevelText);
            playPauseButton = (Canvas)GameObjectModels["playPauseButtonModel"];
            PlayArea.Children.Add((Canvas)GameObjectModels["playerHealthModel"]);
            playerHealthText = (TextBlock)GameHelper.FindCanvasChild((Canvas)GameObjectModels["playerHealthModel"], "playerHealthTextBlock");
            PlayArea.Children.Add(playPauseButton);
        }
        private void StartEventListeners()
        {
            playPauseButton.MouseDown += PlayPauseButton_MouseDown;
            WaveSpawner.OnWaveCleared += WaveSpawner_OnWaveCleared;
            Form.Loaded += Form_Loaded;
        }
        private void ShowPlayButton()
        {
            Shape playButton = (Shape)GameHelper.FindCanvasChild(playPauseButton, "playButtonModel");
            Canvas pauseButton = (Canvas)GameHelper.FindCanvasChild(playPauseButton, "pauseButtonModel");
            playButton.Visibility = Visibility.Visible;
            pauseButton.Visibility = Visibility.Hidden;
        }
        private void ShowPauseButton()
        {
            Shape playButton = (Shape)GameHelper.FindCanvasChild(playPauseButton, "playButtonModel");
            Canvas pauseButton = (Canvas)GameHelper.FindCanvasChild(playPauseButton, "pauseButtonModel");
            playButton.Visibility = Visibility.Hidden;
            pauseButton.Visibility = Visibility.Visible;
        }
        private void UpdateGame()
        {
            DeltaTime = GetDeltaTime();
            UpdateCollisionTree();
            for (int i = 0; i < updatingObjects.Count; i++)
            {
                updatingObjects[i].Update();
            }
            UpdateTextBlocks();
            if (GameOver)
            {
                ResetGame();
            }
        }
        private void UpdateTextBlocks()
        {
            playerGoldText.Text = PlayerCash.ToString();
            playerHealthText.Text = PlayerTank.HitPoints.ToString();
            currentLevelText.Text = "Level: " + (wavesCleared + 1);
        }
        private void ResetGame()
        {
            wavesCleared = 0;
            playerCash = startingCash;
            playerGoldText.Text = startingCash.ToString();
            PlayerTank.HitPoints = 10;
            playerHealthText.Text = PlayerTank.HitPoints.ToString();
            currentLevelText.Text = "Level :" + (wavesCleared + 1);
            mazeBuilder.Reset();
            WaveSpawner.Reset();
            ShowPlayButton();
            Pause();
            GameOver = false;
            Grid.UpdateNodes();
            mazeBuilder.Start();
        }
        private void UpdateCollisionTree()
        {
            QuadTree.Reset();
            foreach (var obj in collidableGameObjects)
            {
                obj.Transform = obj.Model.TransformToAncestor(PlayArea);
                obj.Collider = new BoxCollider(obj, obj.Transform);
                QuadTree.Insert(obj);
            }
        }
        private float GetDeltaTime()
        {
            if (currentTime == null)
            {
                currentTime = DateTime.Now;
                return 0;
            }
            else
            {
                DateTime previous = currentTime;
                currentTime = DateTime.Now;
                TimeSpan deltaTime = currentTime - previous;
                return deltaTime.Milliseconds;
            }
        }
        public void AddGameObject(MovableGameObject obj)
        {
            PlayArea.Children.Add(obj.Model);
            if (obj is Enemy)
                enemies.Add(obj as Enemy);
            if (obj is IUpdate)
                updatingObjects.Add(obj as IUpdate);
            if (obj is ICollidableGameObject)
                collidableGameObjects.Add(obj as ICollidableGameObject);
        }   
        public void RemoveGameObject(MovableGameObject obj)
        {
            PlayArea.Children.Remove(obj.Model);
            if (obj is Enemy)
                enemies.Remove(obj as Enemy);
            if (obj is IUpdate)
                updatingObjects.Remove(obj as IUpdate);
            if (obj is ICollidableGameObject)
                collidableGameObjects.Remove(obj as ICollidableGameObject);
        }
        public void CancelUpdate(IUpdate obj)
        {
            if(updatingObjects.Contains(obj))
                updatingObjects.Remove(obj);
        }
        public void StartUpdate(IUpdate obj)
        {
            if(!updatingObjects.Contains(obj))
                updatingObjects.Add(obj);
        }
        public List<Enemy> GetEnemies()
        {
            return enemies;
        }
        public void Pause()
        {
            if(OnGamePaused != null)
            {
                OnGamePaused();
            }
            CompositionTarget.Rendering -= CompositionTarget_Rendering;
        }
        public void UnPause()
        {
            if(OnGameUnPaused != null)
            {
                OnGameUnPaused();
            }
            currentTime = DateTime.Now;
            CompositionTarget.Rendering += CompositionTarget_Rendering;
        }
    }
}
