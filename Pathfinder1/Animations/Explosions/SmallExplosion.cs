using System;

namespace ShapeTD
{
    internal class SmallExplosion : Explosion
    {
        protected override bool FadeOut { get; set; }
        protected override int MaximumParticleSpeed { get; set; }
        protected override int MinimumParticleSpeed { get; set; }
        protected override float SpinRate { get; set; }
        protected override int TimeSpanMilliseconds { get; set; }
        protected override float ParticleSpeed { get; set; }
        protected override Type ParticleType { get; set; }
        public SmallExplosion(GameController game, MovableGameObject target) : base(game, target)
        {

        }
        protected override void Initialize()
        {
            FadeOut = true;
            MinimumParticleSpeed = 2;
            MaximumParticleSpeed = 5;
            TimeSpanMilliseconds = 300;
            ParticleSpeed = Game.Rand.Next(MinimumParticleSpeed, MaximumParticleSpeed);
            SpinRate = 0f;
            ParticleType = typeof(ExplosionParticle);
        }
    }
}