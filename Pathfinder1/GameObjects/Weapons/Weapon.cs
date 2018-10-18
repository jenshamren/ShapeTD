using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapeTD
{
    abstract class Weapon : MovableGameObject, IUpdate
    {
        private int fireCounter;
        private Shape projectileSpawnPoint;
        protected abstract int FireInterval { get; }
        protected abstract int ReloadInterval { get; }
        protected AmmunitionLeftAnimation ProjectilesLeftAnimation { get; set; }
        protected bool Reloading { get; set; }
        public abstract int MagazineSize { get; }
        public abstract Queue<WeaponProjectile> Magazine { get; protected set; }
        public Point ProjectileSpawnPoint { get; private set; }
        public Point Target { get; set; }
        public bool Fire { get; set; }
        public MovableGameObject Holder { get; protected set; }
        public override Point Position
        {
            get; protected set;
        }
        public Weapon(GameController game, MovableGameObject holder) : base(game)
        {
            Holder = holder;
            Initialize();
            projectileSpawnPoint = GetProjectileSpawnPoint();
            OnPositionChanged += Weapon_OnPositionChanged;
        }
        private void Weapon_OnPositionChanged()
        {
            Position = projectileSpawnPoint.TranslatePoint(new Point(), Game.PlayArea);
        }
        protected new abstract void Initialize();
        protected override void Game_OnGamePaused()
        {
            return;
        }
        protected override void Game_OnGameUnPaused()
        {
            return;
        }
        private void FireProjectile(Point target)
        {
            WeaponProjectile projectile = Magazine.Dequeue();
            Game.AddGameObject(projectile);
            if (ProjectilesLeftAnimation != null)
            {
                ProjectilesLeftAnimation.Start();
            }
        }
        private bool CanShoot()
        {
            return fireCounter * Game.DeltaTime > FireInterval && Magazine.Count > 0;
        }
        protected abstract Shape GetProjectileSpawnPoint();
        protected abstract void Reload();
        public abstract Type GetProjectileType();
        public virtual void Update()
        {
            ProjectileSpawnPoint = projectileSpawnPoint.TranslatePoint(new Point(), Game.PlayArea);
            Rotate(LookAt(Target), Width / 2, Height / 1.25);
            if (Magazine.Count == 0)
            {
                Reload();
            }
            if (Fire)
            {
                fireCounter++;
                if (CanShoot())
                {
                    FireProjectile(Target);
                    fireCounter = 0;
                }
            }
            if(ProjectilesLeftAnimation != null)
            {
                ProjectilesLeftAnimation.Update();
            }
        }
    }
}
