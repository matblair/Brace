using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1
{
    using System.Diagnostics;
    using SharpDX;
    using SharpDX.Toolkit.Graphics;

    class FPSRenderer
    {
        SpriteBatch spriteBatch;
        SpriteFont font;
        Stopwatch clock;
        long lastTime=0;

        public FPSRenderer(Project1Game game)
        {
            this.font = game.DefaultFont;
            spriteBatch = new SpriteBatch(game.GraphicsDevice);
            clock = Stopwatch.StartNew();
        }

        public void Draw()
        {
            var milliseconds = clock.ElapsedTicks - lastTime;
            lastTime = clock.ElapsedTicks;
            float fps = 1f / milliseconds * Stopwatch.Frequency;

            spriteBatch.Begin();
            spriteBatch.DrawString(font, string.Format("FPS: {0:F1}", fps), new Vector2(10, 10), Color.White);
            spriteBatch.End();
        }
    }
}
