using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Content;
using XNAPLUS;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace DAPOD_HME.Core
{
    class HUD
    {
        private static readonly HUD INSTANCE = new HUD();

        private SpriteSheet pointsSheet;
        private Texture2D pointsName, comboName;

        private Vector2 endVectorPoints, target;
        private float endVectorNames;

        public static HUD Get()
        {
            return INSTANCE;
        }
        public void Init(ContentManager content)
        {
            pointsSheet = new SpriteSheet(content.Load<Texture2D>("Graphics/Points"), 16, 16);
            pointsName = content.Load<Texture2D>("Graphics/PointsName");
            comboName = content.Load<Texture2D>("Graphics/Combo");
            endVectorNames = 0;
            endVectorPoints = Vector2.Zero;
            target = endVectorPoints;
        }

        public void Reset()
        {
            endVectorNames = 0;
            endVectorPoints = Vector2.Zero;
            target = Vector2.Zero;
        }
        public void UpdateEnd(int delta, int timer, int startTime)
        {
            endVectorNames -= 0.05f * (float)delta;

            target = new Vector2((Globals.SCREENSIZE.X / 2) - ((SurvivalManager.Get().GetPoints().ToString().Length * pointsSheet.TileWidth) / 2) - 101,
                (Globals.SCREENSIZE.Y / 2) - (pointsSheet.TileHeight / 2));

            if (timer <= 0)
                endVectorPoints = target;
            else
            {
                endVectorPoints.X = target.X / 100f * (((float)startTime - (float)timer) / (float)startTime * 100f);
                endVectorPoints.Y = target.Y / 100f * (((float)startTime - (float)timer) / (float)startTime * 100f);
            }
            
        }
        public void DrawHUD(SpriteBatch batch)
        {
            batch.Begin(SpriteSortMode.BackToFront,
                          BlendState.AlphaBlend,
                          SamplerState.PointClamp,
                          null,
                          null,
                          null,
                          Matrix.CreateTranslation(2, 2 + endVectorNames ,0));
            batch.Draw(pointsName, Vector2.Zero, Color.White);
            batch.End();
            batch.Begin(SpriteSortMode.BackToFront,
                          BlendState.AlphaBlend,
                          SamplerState.PointClamp,
                          null,
                          null,
                          null,
                          Matrix.CreateTranslation(2, 2, 0));
            DrawPoints(batch);
            batch.End();

            if (SurvivalManager.Get().GetCombo() > 1)
            {
                batch.Begin(SpriteSortMode.BackToFront,
                         BlendState.AlphaBlend,
                         SamplerState.PointClamp,
                         null,
                         null,
                         null,
                         Matrix.CreateTranslation(2, 21 + endVectorNames, 0));
                batch.Draw(comboName, Vector2.Zero, Color.White);
                DrawCombo(batch);
                batch.End();
            }
        }

        private void DrawPoints(SpriteBatch batch)
        {
            string pointsDigits = SurvivalManager.Get().GetPoints().ToString();

            Vector2 pos = new Vector2(99 + 8, 8);
            DrawNumberString(batch, pointsDigits, pos + endVectorPoints);
        }
        private void DrawCombo(SpriteBatch batch)
        {
            string pointsDigits = SurvivalManager.Get().GetCombo().ToString();
            Vector2 pos = new Vector2(109 + 8, 8);
            DrawNumberString(batch, pointsDigits, pos);

        }
        public void DrawNumberString(SpriteBatch batch, string numbers, Vector2 pos)
        {
            foreach (char c in numbers)
            {
                batch.DrawSprite(pointsSheet, pos, 0, (int)Char.GetNumericValue(c), 0);
                pos += new Vector2(16, 0);
            }
        }
    }
}
