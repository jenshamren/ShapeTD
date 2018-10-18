using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Shapes;

namespace ShapeTD
{
    class ExplosionParticlePool
    {
        private GameController game;
        private Queue<ExplosionParticle> rectangles;
        private Queue<ExplosionParticle> ellipses;
        public ExplosionParticlePool(GameController game, int particlesPerType)
        {
            this.game = game;
            rectangles = GetExplosionParticles(typeof(Rectangle), particlesPerType);
            ellipses = GetExplosionParticles(typeof(Ellipse), particlesPerType);
        }
        private Queue<ExplosionParticle> GetExplosionParticles(Type particleType, int particlesPerType)
        {
            Queue<ExplosionParticle> particles = new Queue<ExplosionParticle>();
            if(particleType == typeof(Rectangle))
            {
                for(int i = 0; i < particlesPerType; i++)
                {
                    ExplosionParticle particle = new ExplosionParticle(game, (Rectangle)GameHelper.Clone(game.GameObjectModels["rectangleExplosionParticle"]));
                    particles.Enqueue(particle);
                    particle.Model.Visibility = System.Windows.Visibility.Hidden;
                    game.PlayArea.Children.Add(particle.Model);
                }
            }
            if(particleType == typeof(Ellipse))
            {
                for (int i = 0; i < particlesPerType; i++)
                {
                    ExplosionParticle particle = new ExplosionParticle(game, (Ellipse)GameHelper.Clone(game.GameObjectModels["ellipseExplosionParticle"]));
                    particles.Enqueue(particle);
                    particle.Model.Visibility = System.Windows.Visibility.Hidden;
                    game.PlayArea.Children.Add(particle.Model);
                }
            }
            return particles;
        }
        public Queue<ExplosionParticle> Retrieve(Type particleType, int amount)
        {
            Queue<ExplosionParticle> particles = new Queue<ExplosionParticle>();
            if(particleType == typeof(Rectangle))
            {
                for(int i = 0; i < amount; i++)
                {
                    ExplosionParticle particle = rectangles.Dequeue();
                    particles.Enqueue(particle);
                }   
            }
            else if (particleType == typeof(Ellipse))
            {
                for (int i = 0; i < amount; i++)
                {
                    ExplosionParticle particle = ellipses.Dequeue();
                    particles.Enqueue(particle);
                }
            }
            return particles;
        }
        public void Insert(ExplosionParticle particle)
        {
            particle.Reset();
            game.CancelUpdate(particle);
            if(particle.Model.GetType() == typeof(Rectangle))
            {
                rectangles.Enqueue(particle);
            }
            else if(particle.Model.GetType() == typeof(Ellipse))
            {
                ellipses.Enqueue(particle);
            }
        }
    }

}
