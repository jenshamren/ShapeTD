using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace ShapeTD
{
    class EnemyPawn : Enemy
    {
        private Rectangle pawnModel;
        private float speed;
        private bool animatingDamage = false;
        private const int maxHitPoints = 300;
        private const float normalSpeed = 3f;
        public override FrameworkElement Model
        {
            get
            {
                return pawnModel;
            }
        }
        public override int HitPoints { get; set; }
        protected override float Speed
        {
            get
            {
                return speed;
            }
        }
        protected override int CashForKilling
        {
            get
            {
                return 5;
            }
        }
        public EnemyPawn(GameController game) : base(game)
        {
            
        }
        protected override void Initialize()
        {
            base.Initialize();
            pawnModel = (Rectangle)GameHelper.Clone(Game.GameObjectModels["enemyPawnModel"]);
            HitPoints = maxHitPoints;
            speed = normalSpeed;
        }
        private void CheckStatChange()
        {
            if (Slowed)
            {
                StatChangeCounter++;
                if (StatChangeCounter * Game.DeltaTime >= StatChangeDuration)
                {
                    speed = normalSpeed;
                    Slowed = false;
                    StatChangeCounter = 0;
                    StatChangeDuration = 0;
                }
            }
        }
        private void CancelDamageAnimation()
        {
            Shape originalModel = (Shape)Game.GameObjectModels["enemyPawnModel"];
            pawnModel.Fill = originalModel.Fill;
            AnimationCounter = 0;
            animatingDamage = false;
        }
        private void AnimateDamage()
        {
            animatingDamage = true;
            pawnModel.Fill = Brushes.WhiteSmoke;
            int animationTimeSpan = 30;
            AnimationCounter++;
            bool colorAnimationFinished = AnimationCounter * Game.DeltaTime > animationTimeSpan;
            if (colorAnimationFinished)
            {
                CancelDamageAnimation();
            }
        }
        public override void Damage(int damage, WeaponProjectile damageSource)
        {
            HitPoints -= damage;
            if(HitPoints > 0)
            {
                AnimateDamage();
            }
            else
            {
                if (animatingDamage)
                    CancelDamageAnimation();
                ExplosionAnimator.StartExplosion(Game, this, damageSource);
                Game.PlayerCash += CashForKilling;
                Destroy();
            }
        }
        public override void Slow(int duration, float rate)
        {
            if (!Slowed)
            {
                speed *= rate;
                StatChangeDuration = duration;
                StatChangeCounter++;
                Slowed = true;
            }
            else
            {
                StatChangeCounter = 0;
            }
        }
        public void IncreaseStats()
        {
            float speedScaling = 1.05f;
            float hpScaling = 1.5f;
            HitPoints = Convert.ToInt32(HitPoints * hpScaling);
            speed *= speedScaling;
        }
        public override void Update()
        {
            PathFollower.FollowPath(Speed * MoverDeltaTime);
            float angle = LookAt(PathFollower.CurrentWaypoint);
            Rotate(angle, Width / 2, Height / 2);
            CheckStatChange();
            if (animatingDamage)
            {
                AnimateDamage();
            }
            if (PathFollower.PathEnded)
            {
                Destroy();
                Game.PlayerTank.HitPoints -= 1;
            }
        }
    }
}
