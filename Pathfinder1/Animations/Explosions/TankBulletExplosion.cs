using System;

namespace ShapeTD
{
    internal class TankBulletExplosion : Explosion
    {
        protected override bool FadeOut { get; set; }
        protected override int MaximumParticleSpeed { get; set; }
        protected override int MinimumParticleSpeed { get; set; }
        protected override float SpinRate { get; set; }
        protected override int TimeSpanMilliseconds { get; set; }
        protected override float ParticleSpeed { get; set; }
        protected override Type ParticleType { get; set; }
        public TankBulletExplosion(GameController game, MovableGameObject target) : base(game, target)
        {

        }
        protected override void Initialize()
        {
            MinimumParticleSpeed = 5;
            MaximumParticleSpeed = 8;
            FadeOut = true;
            SpinRate = 0f;
            TimeSpanMilliseconds = 100;
            ParticleSpeed = Game.Rand.Next(MinimumParticleSpeed, MaximumParticleSpeed);
            ParticleType = typeof(ExplosionParticle);
            ParticleSize = 5;
        }
    }
}