using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using DAPOD_HME.EntitySystem;
using DAPOD_HME.Core;

namespace DAPOD_HME.EntitySystem
{
    class EntityFactory
    {
        private static readonly EntityFactory INSTANCE = new EntityFactory();

        public List<StaticEntity> currentLevelList { set; get; }
        public List<StaticEntity> mapEntities { set; get; }

        private static Random random = new Random();
        private int mapWidth, mapHeight;

        public static EntityFactory Get()
        {
            return INSTANCE;
        }

        public void Init(int mapwidth, int mapheight)
        {
            mapWidth = mapwidth;
            mapHeight = mapheight;
            currentLevelList = new List<StaticEntity>();

            mapEntities = new List<StaticEntity>();
        }
        private EntityFactory() { }

        public void ResetFactory(int mapwidth, int mapheight)
        {
            mapWidth = mapwidth;
            mapHeight = mapheight;
            mapEntities.Clear();
            currentLevelList = null;
        }
        public void UpdateEnteties(int delta)
        {
            foreach (BasicEntity e in mapEntities)
            {
                e.Update(delta);
            }
        }
        public void DrawEnteties(SpriteBatch batch)
        {
            foreach (BasicEntity e in mapEntities)
            {
                e.Draw(batch);
            }
        }

        public void GenerateNewEntity()
        {
            if (currentLevelList.Count > 0)
            {
                int i = (int)(random.NextDouble() * (currentLevelList.Count() - 1));
                //Console.WriteLine(i);
                StaticEntity e = currentLevelList[i].Clone();
                e.Position = new Vector2((int)(random.NextDouble() * (mapWidth - 4)) * 16 + 32, (int)(random.NextDouble() * (mapHeight - 4)) * 16 + 32);
                
                if(!checkCollisionOnCreating(e))
                    mapEntities.Add(e);
            }
            else
                throw new NullReferenceException("no getter list set!");
        }
        private bool checkCollisionOnCreating(StaticEntity entityToCheck)
        {
            Rectangle pacman = new Rectangle(Globals.Round(Player.Get().Position.X) + 8, Globals.Round(Player.Get().Position.Y) + 8,
                (int)(Player.Get().PacmanAnimation.TileWidth / Camera.Get().Scale * 2), (int)(Player.Get().PacmanAnimation.TileHeight / Camera.Get().Scale * 2));

            Rectangle entity = new Rectangle(Globals.Round(entityToCheck.Position.X), Globals.Round(entityToCheck.Position.Y), 
                 entityToCheck.SpriteReference.TileWidth, entityToCheck.SpriteReference.TileHeight);

            if(pacman.Intersects(entity))
                return true;

            return false;
        }

        public StaticEntity CreatePassive16x16Entities(EntityTypes.Passive16x16 type)
        {
            switch (type)
            {
                case EntityTypes.Passive16x16.DOT:
                    return new StaticEntity(new Point(0, 0), new Vector2(0, 0), 0.5f, 0.001f, Globals.SHEETS["16x16"]);
                case EntityTypes.Passive16x16.APPLE_RED:
                    return new StaticEntity(new Point(1, 0), new Vector2(0, 0), 0.5f, 0.002f, Globals.SHEETS["16x16"]);
                case EntityTypes.Passive16x16.BULB:
                    return new StaticEntity(new Point(2, 0), new Vector2(0, 0), 0.5f, 0.002f, Globals.SHEETS["16x16"]);
                case EntityTypes.Passive16x16.BANANA:
                    return new StaticEntity(new Point(3, 0), new Vector2(0, 0), 0.5f, 0.003f, Globals.SHEETS["16x16"]);
                case EntityTypes.Passive16x16.APPLE_GREEN:
                    return new StaticEntity(new Point(4, 0), new Vector2(0, 0), 0.5f, 0.002f, Globals.SHEETS["16x16"]);
                case EntityTypes.Passive16x16.ORANGE:
                    return new StaticEntity(new Point(5, 0), new Vector2(0, 0), 0.5f, 0.004f, Globals.SHEETS["16x16"]);
                case EntityTypes.Passive16x16.ARTICHOKE:
                    return new StaticEntity(new Point(6, 0), new Vector2(0, 0), 0.5f, 0.003f, Globals.SHEETS["16x16"]);
                case EntityTypes.Passive16x16.BERRIES:
                    return new StaticEntity(new Point(7, 0), new Vector2(0, 0), 0.5f, 0.005f, Globals.SHEETS["16x16"]);
                case EntityTypes.Passive16x16.LEMON:
                    return new StaticEntity(new Point(8, 0), new Vector2(0, 0), 0.5f, 0.003f, Globals.SHEETS["16x16"]);
                case EntityTypes.Passive16x16.ICE:
                    return new StaticEntity(new Point(9, 0), new Vector2(0, 0), 0.5f, 0.005f, Globals.SHEETS["16x16"]);
                case EntityTypes.Passive16x16.TREE_SMALL:
                    return new StaticEntity(new Point(10, 0), new Vector2(0, 0), 2.0f, 0.020f, Globals.SHEETS["16x16"]);
                case EntityTypes.Passive16x16.DUDE:
                    return new StaticEntity(new Point(11, 0), new Vector2(0, 0), 1.9f, 0.018f, Globals.SHEETS["16x16"]);
                case EntityTypes.Passive16x16.MARIO:
                    return new StaticEntity(new Point(12, 0), new Vector2(0, 0), 1.1f, 0.015f, Globals.SHEETS["16x16"]);
                case EntityTypes.Passive16x16.LUIGI:
                    return new StaticEntity(new Point(13, 0), new Vector2(0, 0), 1.1f, 0.015f, Globals.SHEETS["16x16"]);
                case EntityTypes.Passive16x16.HAT:
                    return new StaticEntity(new Point(14, 0), new Vector2(0, 0), 1.5f, 0.012f, Globals.SHEETS["16x16"]);
                case EntityTypes.Passive16x16.POWERUP:
                    return new StaticEntity(new Point(15, 0), new Vector2(0, 0), 2.0f, 0.017f, Globals.SHEETS["16x16"]);
            }

            return null;
        }



    }
}
