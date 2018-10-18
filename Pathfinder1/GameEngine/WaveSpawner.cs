using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShapeTD
{
    class WaveSpawner : IUpdate
    {
        public delegate void WaveHandler();
        public event WaveHandler OnWaveCleared;
        private GameController game;
        private Queue<Enemy> wave;
        private List<Enemy> activeEnemies;
        private int spawnTicks;
        private int delayTicks;
        public bool IsRunning { get; private set; }
        public int SpawnInterval { get; set; }
        public int WaveSize { get; set; }
        public WaveSpawner(GameController game)
        {
            this.game = game;
            Initialize();
        }
        private void Initialize()
        {
            WaveSize = 100;
            SpawnInterval = 120;
            activeEnemies = new List<Enemy>();            
        }
        private void UpdateActiveEnemies()
        {
            for(int i = 0; i < activeEnemies.Count; i++)
            {
                if (activeEnemies[i].Destroyed)
                {
                    activeEnemies.Remove(activeEnemies[i]);
                }
            }
        }
        private Queue<Enemy> GetWave(int wavesCleared)
        {
            Queue<Enemy> wave = new Queue<Enemy>();
            for(int i = 0; i < WaveSize; i++)
            {
                EnemyPawn enemy = new EnemyPawn(game);
                for(int j = 0; j < wavesCleared; j++)
                {
                    enemy.IncreaseStats();
                }
                wave.Enqueue(enemy);
            }
            return wave;
        }
        private void Stop()
        {
            game.CancelUpdate(this);
            spawnTicks = 0;
            IsRunning = false;
        }
        private void DelayStop()
        {
            int eventDelayMS = 3000;
            delayTicks++;
            if (delayTicks * game.DeltaTime >= eventDelayMS)
            {
                if (OnWaveCleared != null && game.PlayerTank.HitPoints > 0)
                {
                    OnWaveCleared();
                    delayTicks = 0;
                }
                Stop();
            }
        }
        public void Reset()
        {
            for(int i = 0; i < activeEnemies.Count; i++)
            {
                activeEnemies[i].Destroy();
            }
            activeEnemies = new List<Enemy>();
            Stop();
        }
        public void Start(int wavesCleared)
        {
            wave = GetWave(wavesCleared);
            game.StartUpdate(this);
            IsRunning = true;
        }
        public void Update()
        {
            spawnTicks++;
            if (spawnTicks * game.DeltaTime >= SpawnInterval && wave.Count > 0)
            {
                Enemy enemy = wave.Dequeue();
                activeEnemies.Add(enemy);
                enemy.MoveToPoint(game.EnemySpawnPosition);
                game.AddGameObject(enemy);
                spawnTicks = 0;
            }
            UpdateActiveEnemies();
            if (activeEnemies.Count == 0)
            {
                DelayStop();
            }
        }
    }
}
