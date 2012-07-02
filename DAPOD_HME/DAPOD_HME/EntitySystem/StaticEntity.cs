using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using XNAPLUS;
using DAPOD_HME.Core;

namespace DAPOD_HME.EntitySystem
{
    class StaticEntity : BasicEntity
    {
        public Point SheetTile { set; get; }

        public SpriteSheet SpriteReference { set; get; }

        public StaticEntity(Point sheetTile, Vector2 position, float size, float growValue, SpriteSheet reference) : base(position, growValue, size)
        {
            SheetTile = sheetTile;
            SpriteReference = reference;
        }

        public override void Update(int delta) { }

        public override void Draw(SpriteBatch batch)
        {
            batch.DrawSprite(SpriteReference, Position - Camera.Get().Position, 0, SheetTile.X, SheetTile.Y);
        }

        public StaticEntity Clone()
        {
            return new StaticEntity(SheetTile, Position, Size, GrowValue, SpriteReference);
        }
    }
}
