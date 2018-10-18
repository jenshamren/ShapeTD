using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace ShapeTD
{
    class WaterTurret : Turret
    {
        private List<Enemy> enemies;
        private Canvas turretModel;
        private WaterTurretLauncher projectileLauncher;
        private Enemy currentEnemyTarget;
        private const float range = 200f;
        protected override float Range
        {
            get
            {
                return range;
            }
        }
        public override FrameworkElement Model
        {
            get
            {
                return turretModel;
            }
        }
        public override int Cost
        {
            get
            {
                return GetCost();
            }
        }
        public WaterTurret(GameController game) : base(game)
        {
            
        }
        protected override void Initialize()
        {
            base.Initialize();
            turretModel = (Canvas)GameHelper.Clone(Game.GameObjectModels["waterTurretModel"]);
            projectileLauncher = new WaterTurretLauncher(Game, this);
            enemies = Game.GetEnemies();
        }
        private Enemy FindTarget()
        {
            if (enemies.Count > 0)
            {
                Enemy nearestEnemy = enemies[0];
                double nearestEnemyDistance = GameHelper.GetDistance(Position, nearestEnemy.Position);
                foreach(var enemy in enemies)
                {
                    double distance = GameHelper.GetDistance(Position, enemy.Position);
                    if(distance < nearestEnemyDistance)
                    {
                        nearestEnemy = enemy;
                        nearestEnemyDistance = distance;
                    }
                }
                if(nearestEnemyDistance < range)
                {
                    return nearestEnemy;
                }
            }
            return null;
        }
        public override void Destroy()
        {
            base.Destroy();
            projectileLauncher.Destroy();
        }
        public static int GetCost()
        {
            return 300;
        }
        public override void Update()
        {
            base.Update();
            if (currentEnemyTarget == null || currentEnemyTarget != null && currentEnemyTarget.Destroyed || GameHelper.GetDistance(Position, currentEnemyTarget.Position) > range) 
            {
                projectileLauncher.Fire = false;
                currentEnemyTarget = FindTarget();
            }
            else
            {
                projectileLauncher.Target = currentEnemyTarget.Collider.Center;
                projectileLauncher.Fire = true;
            }
            projectileLauncher.Update();
        }
    }
}
