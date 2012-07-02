using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNAPLUS
{
    /// <summary>
    /// Simple FrameConter. Display the Frames in the top-left corner.
    /// </summary>
    public class FrameCounter
    {
        private static readonly FrameCounter INSTANCE = new FrameCounter();

        private int fps;
        private int fpsCounter;
        private TimeSpan delta;

        public static FrameCounter Get()
        {
            return INSTANCE;
        }
        public void Init()
        {
            fps = 0;
            fpsCounter = 0;
            delta = TimeSpan.Zero;
        }
        private FrameCounter() { }

        public void Update(GameTime gameTime)
        {
            delta += gameTime.ElapsedGameTime;

            if (delta > TimeSpan.FromSeconds(1))
            {
                delta -= TimeSpan.FromSeconds(1);
                fps = fpsCounter;
                fpsCounter = 0;
            }
            fpsCounter++;
        }
        public void DrawFPS(SpriteFont font, SpriteBatch g)
        {
            g.DrawString(font, string.Format("FPS: {0}", this.fps), new Vector2(4, 2), Color.White);

        }

    }
}
