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
        readonly int COOLDOWN = 3000;
        int spawnerCooldown;

        EnemySpawner(Player target)
            : base(Vector3.Zero, Vector3.Zero) 
        {
            spawnerCooldown = 0;
            this.target = target;

        }
        public override void Update(GameTime gametime)
        {
            spawnerCooldown+=gametime.ElapsedGameTime.Milliseconds;
            if(spawnerCooldown>COOLDOWN) {
                spawnEnemyOffscreen();
                spawnerCooldown=0;
            }
            
        }

        private void spawnEnemyOffscreen()
        {
            Vector3 center = BraceGame.get().getPlayer().position;
            float radius = Math.Max(BraceGame.get().GraphicsDevice.Viewport.Width, BraceGame.get().GraphicsDevice.Viewport.Height);

        }

        private void spawnEnemy(Vector3 position)
        {
            BraceGame.get().AddActor(new Enemy(position, Vector3.Zero));
        }
        public override void Draw(GraphicsDevice context, Matrix view, Matrix projection, Effect effect)
        {
            return;
        }
    }
}
