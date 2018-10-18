using System;

namespace ShapeTD
{
    internal class BigExplosion : Explosion
    {
        protected override bool FadeOut { get; set; }
        protected override float SpinRate { get; set; }
        protected override int MinimumParticleSpeed { get; set; }
        protected override int MaximumParticleSpeed { get; set; }
        protected override int TimeSpanMilliseconds { get; set; }
        protected override float ParticleSpeed { get; set; }
        protected override Type ParticleType { get; set; }
        public BigExplosion(GameController game, MovableGameObject target) : base(game, target)
        {

        }
        protected override void Initialize()
        {
            FadeOut = true;
            MinimumParticleSpeed = 9;
            MaximumParticleSpeed = 17;
            TimeSpanMilliseconds = 1500;
            ParticleSpeed = Game.Rand.Next(MinimumParticleSpeed, MaximumParticleSpeed);
            SpinRate = ParticleSpeed / 1.2f;
            ParticleType = typeof(ExplosionParticle);
        }
    }
}