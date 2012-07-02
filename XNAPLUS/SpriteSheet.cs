using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNAPLUS
{
    /// <summary>
    ///  This Class represents a texture with tiles. The Tiles are sorted in Lines:
    ///  
    ///  [0][1][2][3][4]
    ///  [5][6][7][8][9]
    ///  
    ///  This means if you want to draw a Tile use the GetTile(int id) method!
    ///  However the Renderer provides a method to use x, y as index too!
    ///  
    /// </summary>
    public class SpriteSheet
    {
        public int TilesAcross { get; set; }
        public int TilesDown { get; set; }
        public int TileWidth { get; set; }
        public int TileHeight { get; set; }
        public Texture2D Texture { get; set; }

        public SpriteSheet(Texture2D texture, int tileWidth, int tileHeight)
        {
            Texture = texture;

            this.TileWidth = tileWidth;
            this.TileHeight = tileHeight;

            TilesAcross = Texture.Width / this.TileWidth;
            TilesDown = Texture.Height / this.TileHeight;
        }

        public Point GetTile(int id)
        {
            return new Point(id % TilesAcross, id / TilesAcross);
        }

        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[TilesAcross: " + TilesAcross);
            sb.Append(", TilesDown: " + TilesDown);
            sb.Append(", TileWidth: " + TileWidth);
            sb.Append(", TileHeight: " + TileHeight + "]");

            return sb.ToString();

        }
    }
}
