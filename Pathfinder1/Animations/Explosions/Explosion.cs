
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapeTD
{
    abstract class Explosion : Animation
    {
        private ExplosionParticle[,] explosionParticles;
        protected abstract float SpinRate { get; set; }
        protected abstract bool FadeOut { get; set; }
        protected abstract int MinimumParticleSpeed { get; set; }
        protected abstract int MaximumParticleSpeed { get; set; }
        protected new abstract int TimeSpanMilliseconds { get; set; }
        protected abstract float ParticleSpeed { get; set; }
        protected abstract Type ParticleType { get; set; }
        protected double ParticleSize { get; set; }
        public Explosion(GameController game, MovableGameObject target) : base(game,target)
        {
            Initialize();
            if(ParticleSize == 0)
            {
                ParticleSize = 7.5;
            }
            explosionParticles = GetExplosionParticles();
            StartExplosion();
        } 
        private void StartExplosion()
        {
            int sizeX = (int)Target.Width / (int)ParticleSize;
            int sizeY = (int)Target.Height / (int)ParticleSize;
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    Point targetPosition = Target.Transform.Transform(new Point((x * ParticleSize), (y * ParticleSize)));
                    explosionParticles[x, y].MoveToPoint(targetPosition);
                    Game.StartUpdate(explosionParticles[x, y]);
                }
            }
        }
        private ExplosionParticle[,] GetExplosionParticles()
        {
            int sizeX = (int)Target.Width / (int)ParticleSize;
            int sizeY = (int)Target.Height / (int)ParticleSize;
            int shapesToSpawn = sizeX * sizeY;
            if (Target.Model is Shape)
            {
                Shape targetModel = Target.Model as Shape;
                Queue<ExplosionParticle> objects = Game.ExplosionParticlePool.Retrieve(Target.Model.GetType(), shapesToSpawn);
                ExplosionParticle[,] particles = new ExplosionParticle[sizeX, sizeY];
                for (int x = 0; x < sizeX; x++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        particles[x, y] = objects.Dequeue();
                        particles[x, y].Speed = ParticleSpeed;
                        particles[x, y].SpinRate = this.SpinRate;
                        particles[x, y].FadeOut = this.FadeOut;
                        particles[x, y].Width = ParticleSize;
                        particles[x, y].Height = ParticleSize;
                        particles[x, y].Fill = targetModel.Fill;
                        particles[x, y].TargetDirection = GameHelper.GetDirection(Game.Rand);
                        particles[x, y].LifeTimeMilliseconds = TimeSpanMilliseconds;
                    }
                }
                return particles;
            }
            return null;
        }
        protected abstract void Initialize();
    }
}
