using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace.GameLogic
{
    class EnemySpawner : Actor
    {
        Player target;
        readonly int COOLDOWN = 1000;
        int spawnerCooldown;
        Random rand;
        public EnemySpawner(Player target)
            : base(Vector3.Zero, Vector3.Zero) 
        {
            spawnerCooldown = 0;
            this.target = target;
            rand = new Random();
        }
        public override void Update(GameTime gametime)
        {
            spawnerCooldown+=gametime.ElapsedGameTime.Milliseconds;
            if(spawnerCooldown>COOLDOWN) {
                int enemyCount = EnemyCount();
                for (int i = enemyCount; i < 20; ++i)
                {
                    spawnEnemyOffscreen();
                }
                spawnerCooldown = 0;

            }
            
        }

        private int EnemyCount()
        {
            int count=0;
            foreach (Actor a in BraceGame.get().actors)
            {
                if (a.GetType() == typeof(Enemy))
                {
                    ++count;
                }
            }
            return count;

        }

        private void spawnEnemyOffscreen()
        {
            Vector3 center = BraceGame.get().getPlayer().position;
            float radius =40;
            float angle = rand.NextFloat(0, (float)Math.PI*2);
            spawnEnemy(center + radius * new Vector3((float)Math.Cos(angle), 0, (float)Math.Sin(angle)));

        }

        private void spawnEnemy(Vector3 position)
        {
            BraceGame.get().AddActor(new Enemy(position, Vector3.Zero));
        }
        public override void Draw(GraphicsDevice context, Matrix view, Matrix projection, Effect effect, List<Light> lights)
        {
            return;
        }
    }
}
