using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNAPLUS
{

    /// <summary>
    /// This class adds methods for the SpriteBatch to draw Animations
    /// and SpriteSheets. To use it just add in the using-directive.
    /// </summary>
    public static class Renderer
    {

        public static void DrawSprite(this SpriteBatch spriteBatch, SpriteSheet sheet, int id)
        {
            DrawSprite(spriteBatch, sheet, Vector2.Zero, 0, SpriteEffects.None, sheet.GetTile(id).X, sheet.GetTile(id).Y);
        }
        public static void DrawSprite(this SpriteBatch spriteBatch, SpriteSheet sheet, Vector2 position, float angle, int id)
        {
            DrawSprite(spriteBatch, sheet, position, angle, SpriteEffects.None, sheet.GetTile(id).X, sheet.GetTile(id).Y);
        }
        public static void DrawSprite(this SpriteBatch spriteBatch, SpriteSheet sheet, Vector2 position, float angle, SpriteEffects effect, int id)
        {
            DrawSprite(spriteBatch, sheet, position, angle, effect, sheet.GetTile(id).X, sheet.GetTile(id).Y);
        }
        public static void DrawSprite(this SpriteBatch spriteBatch, SpriteSheet sheet, int x, int y)
        {
            DrawSprite(spriteBatch, sheet, Vector2.Zero, 0, SpriteEffects.None, x, y);
        }
        public static void DrawSprite(this SpriteBatch spriteBatch, SpriteSheet sheet, Vector2 position, float angle, int x, int y)
        {
            DrawSprite(spriteBatch, sheet, position, angle, SpriteEffects.None, x, y);
        }
        public static void DrawSprite(this SpriteBatch spriteBatch, SpriteSheet sheet, Vector2 position, float angle, SpriteEffects effect, int x, int y)
        {
            Rectangle source = new Rectangle(x * sheet.TileWidth, y * sheet.TileHeight, sheet.TileWidth, sheet.TileHeight);
            Rectangle dest = new Rectangle(Round(position.X), Round(position.Y), sheet.TileWidth, sheet.TileHeight);
            spriteBatch.Draw(sheet.Texture, dest, source, Color.White, angle, new Vector2(sheet.TileWidth / 2, sheet.TileHeight / 2), effect, 0);
        }

        public static void DrawAnimation(this SpriteBatch spriteBatch, Animation animation)
        {
            DrawAnimation(spriteBatch, animation, Vector2.Zero, 0);
        }
        //draw with angle and position
        public static void DrawAnimation(this SpriteBatch spriteBatch, Animation animation, Vector2 position, float angle)
        {
            DrawAnimation(spriteBatch, animation, position, angle, SpriteEffects.None);
        }
        // draw with sprite effect
        public static void DrawAnimation(this SpriteBatch spriteBatch, Animation animation, Vector2 position, float angle, SpriteEffects effect)
        {
            Point tile = animation.GetTile(animation.CurrentFrame);
            Rectangle source = new Rectangle(tile.X * animation.TileWidth, tile.Y * animation.TileHeight, animation.TileWidth, animation.TileHeight);
            Rectangle dest = new Rectangle(Round(position.X), Round(position.Y), animation.TileWidth, animation.TileHeight);
            spriteBatch.Draw(animation.Texture, dest, source, Color.White, angle, new Vector2(animation.TileWidth / 2, animation.TileHeight / 2), effect, 0);

        }

        //private support methods
        private static int Round(float value)
        {
            if (value < 0)
                return (int)(value - 0.5f);
            else
                return (int)(value + 0.5f);
        }
    }
}
