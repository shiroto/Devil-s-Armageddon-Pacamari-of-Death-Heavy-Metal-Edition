using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Content;
using XNAPLUS;
using DAPOD_HME.EntitySystem;

namespace DAPOD_HME.Core
{
    class Player
    {

        private static readonly Player INSTANCE = new Player();

        public Vector2 Position;
        public DIRECTION Direction;
        public Animation PacmanAnimation { get; set; }
        public float RelativeSize { get; set; }
        public float MoveSpeed { get; set; }

        private bool isMoving;
        private Vector2 oldPos;

        private Camera cam = Camera.Get();

        public enum DIRECTION 
        {
            LEFT, RIGHT, DOWN, UP
        }

        public static Player Get()
        {
            return INSTANCE;
        }
        public void Init(ContentManager content)
        {
            Position = Vector2.Zero;
            RelativeSize = 1f;
            MoveSpeed = 0.2f;
            isMoving = false;
            Direction = DIRECTION.RIGHT;
            PacmanAnimation = new Animation(content.Load<Texture2D>("Graphics/pacamari"), 16, 16, 200);
            

        }
        private Player() { }

        public void Update(KeyboardState input, int delta)
        {
            oldPos = Position;

            if (isMoving)
                PacmanAnimation.Update(delta);
            else
                PacmanAnimation.Reset(0);
            
            //proceed input
            isMoving = false;
            if (input.IsKeyDown(Keys.Up))
            {
                Position.Y -= MoveSpeed * delta;
                Direction = DIRECTION.UP;
                isMoving = true;
                checkCollision(false);
            }
            if (input.IsKeyDown(Keys.Down))
            {
                Position.Y += MoveSpeed * delta;
                Direction = DIRECTION.DOWN;
                isMoving = true;
                checkCollision(false);
            }
            if (input.IsKeyDown(Keys.Left))
            {
                Position.X -= MoveSpeed * delta;
                Direction = DIRECTION.LEFT;
                isMoving = true;
                checkCollision(true);

            }
            if (input.IsKeyDown(Keys.Right))
            {
                Position.X += MoveSpeed * delta;
                Direction = DIRECTION.RIGHT;
                isMoving = true;
                checkCollision(true);
            }

            // bounds check
            if (Position.X < 16)
                Position.X = 16;
            if (Position.Y < 16)
                Position.Y = 16;
            if (Position.X > (cam.MapSize.X * Globals.MAXTILESIZE) - (PacmanAnimation.TileWidth / cam.Scale * 2) - 16)
                Position.X = (cam.MapSize.X * Globals.MAXTILESIZE) - (PacmanAnimation.TileWidth / cam.Scale * 2) - 16;
            if (Position.Y > (cam.MapSize.Y * Globals.MAXTILESIZE) - (PacmanAnimation.TileHeight / cam.Scale * 2) - 16)
                Position.Y = (cam.MapSize.Y * Globals.MAXTILESIZE) - (PacmanAnimation.TileHeight / cam.Scale * 2) - 16;

            //update camera
            cam.SetFixVector(Position - Globals.CENTER_PACMAN / cam.Scale);
           
        }

        // use these methods to scale the context probatly
        public void ScaleDown(float scale)
        {
            if (checkDownScale())
                return;

            RelativeSize += scale;
            cam.Scale -= scale;
            checkDownScale();

        }
        public void ScaleUp(float scale)
        {
            if (checkUpScale())
                return;

            RelativeSize -= scale;
            cam.Scale += scale;
            checkUpScale();
        }
        private bool checkDownScale()
        {
            bool set1 = false;
            bool set2 = false;
            if (cam.Scale <= 0.4f)
            {
                cam.SetFixScale(0.4f);
                set1 = true;
            }
            if (RelativeSize >= 5)
            {
                RelativeSize = 5;
                set2 = true;
            }

            return set1 & set2;
        }
        private bool checkUpScale()
        {
            bool set1 = false;
            bool set2 = false;
            if (cam.Scale >= 2)
            {
                cam.SetFixScale(2);
                set1 = true;
            }
            if (RelativeSize <= 1)
            {
                RelativeSize = 1;
                set2 = true;
            }

            return set1 & set2;
        }
        public void checkCollision(bool axis)
        {
            Rectangle pacman = new Rectangle(Globals.Round(Position.X) +8, Globals.Round(Position.Y) +8,
                (int)(PacmanAnimation.TileWidth / cam.Scale * 2), (int)(PacmanAnimation.TileHeight / cam.Scale * 2));
            Rectangle entity = new Rectangle(0, 0, 0, 0);
            List<StaticEntity> list = EntityFactory.Get().mapEntities;


            for (int i = 0; i < list.Count; i++)
            {
                entity.X = Globals.Round(list[i].Position.X);
                entity.Y = Globals.Round(list[i].Position.Y);
                entity.Width = list[i].SpriteReference.TileWidth;
                entity.Height = list[i].SpriteReference.TileHeight;


                // TODO add damage cast with getType 

                if (pacman.Intersects(entity) && RelativeSize < list[i].Size)
                {
                    // hit return pos
                    if (axis)
                        Position.X = oldPos.X;
                    else
                        Position.Y = oldPos.Y;
                    return;                        // do not to check any more entities
                }
                else if (pacman.Intersects(entity) && RelativeSize > list[i].Size)
                {
                    // eat this thing!
                    MusicManager.Get().PlayEat();
                    ScaleDown(list[i].GrowValue);
                    cam.ShakeScreen(300, 10);
                    SurvivalManager.Get().removeEntityAndAddPoints(i, list[i]);
                    return;
                }
                
            }

        }

        public void DrawPacman(SpriteBatch spriteBatch)
        {
            if(Direction == DIRECTION.LEFT)
                spriteBatch.DrawAnimation(PacmanAnimation, new Vector2(Globals.SCREENSIZE.X / 4, Globals.SCREENSIZE.Y / 4), 0, SpriteEffects.FlipHorizontally);
            if(Direction == DIRECTION.RIGHT)
                spriteBatch.DrawAnimation(PacmanAnimation, new Vector2(Globals.SCREENSIZE.X / 4, Globals.SCREENSIZE.Y / 4), 0);
            if(Direction == DIRECTION.UP)
                spriteBatch.DrawAnimation(PacmanAnimation, new Vector2(Globals.SCREENSIZE.X / 4, Globals.SCREENSIZE.Y / 4), 0.8f % MathHelper.Pi * 2, SpriteEffects.FlipHorizontally);
            if (Direction == DIRECTION.DOWN)
                spriteBatch.DrawAnimation(PacmanAnimation, new Vector2(Globals.SCREENSIZE.X / 4, Globals.SCREENSIZE.Y / 4), 0.8f % MathHelper.Pi * 2);
        }

        public void SetStartPositionTile(int x, int y)
        {
            if (x < 0)
                x = 0;
            if (x > cam.MapSize.X)
                x = cam.MapSize.X;

            if (y < 0)
                y = 0;
            if (y > cam.MapSize.Y)
                y = cam.MapSize.Y;

            Position.X = x * 16;
            Position.Y = y * 16;
        }



    }
}
