using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DAPOD_HME.EntitySystem
{
    abstract class BasicEntity
    {
        public Vector2 Position { set; get; }
        public float GrowValue { get; private set; }                  // then number of pixels pacman grows when eating the entity 
        public float Size { get; private set; }

        public BasicEntity(Vector2 position, float growValue, float size)
        {
            Position = position + new Vector2(8, 8);
            this.GrowValue = growValue;
            this.Size = size;
        }

        public abstract void Update(int delta);
        public abstract void Draw(SpriteBatch batch);
    }
}
