using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapeTD
{
    class ExplosionParticle : MovableGameObject, IUpdate
    {
        private Shape particle;
        private const int fadeIntervalMilliseconds = 500;
        private float currentAngle;
        private int lifeTimeCounter;
        private bool isAlive = false;
        private bool firstUpdate = true;
        public override FrameworkElement Model
        {
            get
            {
                return particle;
            }
        }
        public float Speed { get; set; }
        public float SpinRate { get; set; }
        public bool FadeOut { get; set; }
        public Vector TargetDirection { get; set; }
        public int LifeTimeMilliseconds { get; set; }
        public Brush Fill
        {
            get { return particle.Fill; }
            set { particle.Fill = value; }
        }
        public ExplosionParticle(GameController game, Shape particle) : base(game)
        {
            this.particle = particle;
        }
        protected override void Initialize()
        {
            base.Initialize();
            currentAngle = 0;
            lifeTimeCounter = 0;
        }
        protected override void Game_OnGamePaused()
        {
            if (isAlive)
            {
                base.Game_OnGamePaused();
            }
            else
            {
                return;
            }
        }
        protected override void Game_OnGameUnPaused()
        {
            if (isAlive)
            {
                base.Game_OnGameUnPaused();
            }
            else
            {
                return;
            }
        }
        private bool CheckFade()
        {
            int startFadeMilliseconds = LifeTimeMilliseconds - fadeIntervalMilliseconds;
            if(FadeOut && lifeTimeCounter * Game.DeltaTime >= startFadeMilliseconds)
            {
                return true;
            }
            return false;
        }
        private void Fade()
        {
            int totalTicksToZero = fadeIntervalMilliseconds / (int)Game.DeltaTime;
            float fadeRate = (float)Model.Opacity / totalTicksToZero;
            Model.Opacity -= fadeRate;
        }
        private float GetSpinRate()
        {
            if (TargetDirection.X > 0)
                return SpinRate;
            if (TargetDirection.X < 0)
                return -SpinRate;
            return 0f;
        }
        public void Reset()
        {
            Model.Visibility = Visibility.Hidden;
            lifeTimeCounter = 0;
            firstUpdate = true;
            isAlive = false;
            Model.Opacity = 1;
            Model.RenderTransform = new RotateTransform(0, Width / 2, Height / 2);
        }
        public virtual void Update()
        {
            if (firstUpdate)
            {
                Model.Visibility = Visibility.Visible;
                isAlive = true;
            }
            lifeTimeCounter++;
            if (SpinRate > 0)
            {
                Rotate(currentAngle += GetSpinRate(), Width / 2, Height / 2);
            }
            MoveTowards(TargetDirection, Speed * MoverDeltaTime);
            if (CheckFade())
            {
                Fade();
            }
            if (OutSideOfGame() || lifeTimeCounter * Game.DeltaTime >= LifeTimeMilliseconds)
            {
                Game.ExplosionParticlePool.Insert(this);
            }
        }
    }
}
