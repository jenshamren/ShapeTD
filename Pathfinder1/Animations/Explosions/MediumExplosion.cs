using System;

namespace ShapeTD
{
    class MediumExplosion : Explosion
    {
        protected override bool FadeOut { get; set; }
        protected override int MaximumParticleSpeed { get; set; }
        protected override int MinimumParticleSpeed { get; set; }
        protected override float SpinRate { get; set; }
        protected override int TimeSpanMilliseconds { get; set; }
        protected override float ParticleSpeed { get; set; }
        protected override Type ParticleType { get; set; }
        public MediumExplosion(GameController game, MovableGameObject target) : base(game, target)
        {

        }
        protected override void Initialize()
        {
            FadeOut = true;
            ParticleSize = 7.5;
            MinimumParticleSpeed = 3;
            MaximumParticleSpeed = 9;
            TimeSpanMilliseconds = 600;
            ParticleSpeed = Game.Rand.Next(MinimumParticleSpeed, MaximumParticleSpeed);
            SpinRate = ParticleSpeed / 2f;
            ParticleType = typeof(ExplosionParticle);
        }
    }
}
