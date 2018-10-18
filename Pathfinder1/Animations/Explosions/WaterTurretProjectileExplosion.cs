using System;

namespace ShapeTD
{
    class WaterTurretProjectileExplosion : Explosion
    {
        protected override bool FadeOut { get; set; }
        protected override int MaximumParticleSpeed { get; set; }
        protected override int MinimumParticleSpeed { get; set; }
        protected override float ParticleSpeed { get; set; }
        protected override Type ParticleType { get; set; }
        protected override float SpinRate { get; set; }
        protected override int TimeSpanMilliseconds { get; set; }
        public WaterTurretProjectileExplosion(GameController game, MovableGameObject target) : base(game, target)
        {

        }
        protected override void Initialize()
        {
            FadeOut = false;
            MinimumParticleSpeed = 4;
            MaximumParticleSpeed = 7;
            ParticleSpeed = Game.Rand.Next(MinimumParticleSpeed, MaximumParticleSpeed);
            ParticleType = typeof(ExplosionParticle);
            SpinRate = 0f;
            TimeSpanMilliseconds = 200;
        }
    }
}
