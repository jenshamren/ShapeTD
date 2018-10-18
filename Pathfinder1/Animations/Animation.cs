
namespace ShapeTD
{
    abstract class Animation
    {
        protected int AnimationCounter { get; set; }
        protected MovableGameObject Target { get; private set; }
        protected GameController Game { get; private set; }
        public int TimeSpanMilliseconds { get; set; }
        public bool IsRunning { get; private set; }
        public Animation(GameController game, MovableGameObject target)
        {
            Target = target;
            Game = game;
        }
        public virtual void Start()
        {
            IsRunning = true;
            AnimationCounter = 0;
        }
        public virtual void Stop()
        {
            IsRunning = false;
            AnimationCounter = 0;
        }

    }
}
