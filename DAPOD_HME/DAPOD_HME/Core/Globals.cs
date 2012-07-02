using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using XNAPLUS;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace DAPOD_HME.Core
{
    class Globals
    {
        public static Point SCREENSIZE = new Point(800, 600);
        public static Vector2 CENTER_PACMAN = new Vector2((SCREENSIZE.X / 2) - 16, (SCREENSIZE.Y / 2) - 16);
        public static int MAXTILESIZE = 16;
        public static long HIGHSCORE;
        public static readonly int GAMEPLAYSTATE = 1;
        public static readonly int MENUSTATE = 2;

        public static Dictionary<string, SpriteSheet> SHEETS;
        public static Texture2D blackBlender;
        

        public static void loadTextures(ContentManager content)
        {
            if (SHEETS == null)
            {
                SHEETS = new Dictionary<string, SpriteSheet>();
                SHEETS.Add("16x16", new SpriteSheet(content.Load<Texture2D>("Graphics/16x16Member"), 16, 16));
                SHEETS.Add("32x32", new SpriteSheet(content.Load<Texture2D>("Graphics/32x32Member"), 32, 32));
                blackBlender = content.Load<Texture2D>("Graphics/Blend");
            }
        }

        public static int Round(float value)
        {
            if (value < 0)
                return (int)(value - 0.5f);
            else
                return (int)(value + 0.5f);
        }
        public static void DrawBlendIn(SpriteBatch batch, int timer, int startTime)
        {
            float alpha;
            if (timer <= 0)
                alpha = 0;
            else
                alpha = 1 - 1f / 100f * (((float)startTime - (float)timer) / (float)startTime * 100f);

            batch.Begin();
            batch.Draw(blackBlender, new Vector2(-20, -20), new Color(0, 0, 0, alpha));
            batch.End();
        }
        public static void DrawBlendOut(SpriteBatch batch, int timer, int startTime)
        {
            float alpha;
            if (timer <= 0)
                alpha = 1;
            else
                alpha = 1f / 100f * (((float)startTime - (float)timer) / (float)startTime * 100f);

            batch.Begin();
            batch.Draw(blackBlender, new Vector2(-20, -20), new Color(1, 1, 1, alpha));
            batch.End();
        }

        public static void WriteHighscore()
        {
            TextWriter writer = new StreamWriter("highscore.dat");
            writer.WriteLine("score:" + HIGHSCORE);
            writer.Close();
        }
        public static void ReadHighscore()
        {
            if(File.Exists("highscore.dat"))
            {

                TextReader reader = new StreamReader("highscore.dat");

                string temp = reader.ReadLine();
                string[] _temp = temp.Split(new char[] {':'});
                int score;
                if (_temp[0].Equals("score") && Int32.TryParse(_temp[1], out score))
                {
                    HIGHSCORE = score;
                }
                else
                    HIGHSCORE = 0;
            }
            else
            {
                WriteNewFileIfNotExists();
            }
        }
        private static void WriteNewFileIfNotExists()
        {
            TextWriter writer = new StreamWriter("highscore.dat");
            writer.WriteLine("score:0");
            writer.Close();
        }



    }
}
