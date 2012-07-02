using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using XNAPLUS.Squared.Tiled;
using XNAPLUS;
using System.IO;
using DAPOD_HME.Core;
using DAPOD_HME.EntitySystem;

namespace DAPOD_HME
{

    class GamePlayState : BasicGameState
    {
        private Camera cam = Camera.Get();
        private Player pacman = Player.Get();
        private EntityFactory factory = EntityFactory.Get();
        private SurvivalManager survialManager = SurvivalManager.Get();
        private MusicManager musicManager = MusicManager.Get();
        private HUD hud = HUD.Get();

        private Map gameMap;
        private int startTimer;
        private GAMEPLAYSTATES currentState;
        private Texture2D highscore;

        private enum GAMEPLAYSTATES
        {
            START, PLAY, END, ENDRESULT
        }

        public GamePlayState(int stateID) : base(stateID) { }

        public override void Init(StateBasedGame container)
        {
            pacman.Init(container.Content);
            gameMap = Map.Load(Path.Combine(container.Content.RootDirectory, "test.tmx"), container.Content);
            cam.MapSize = new Point(gameMap.Width, gameMap.Height);
            factory.Init(gameMap.Width, gameMap.Height);
            survialManager.Init();
            hud.Init(container.Content);
            highscore = container.Content.Load<Texture2D>("Graphics/Highscore");

        }
        public override void Enter(StateBasedGame container)
        {
            startTimer = 4000;
            pacman.SetStartPositionTile(50, 50);
            currentState = GAMEPLAYSTATES.START;
            cam.SetFixVector(Vector2.Zero);
            cam.CamSpeed = 0.2f;
            cam.Position = pacman.Position - Globals.CENTER_PACMAN / cam.Scale;
            MediaPlayer.IsRepeating = false;
            MediaPlayer.Volume = 1f;
            cam.ShakeScreen(2, 1);
            factory.ResetFactory(gameMap.Width, gameMap.Height);
            survialManager.ResetSurivalMode();
            hud.Reset();
        }
        public override void Exit(StateBasedGame container) { }

        public override void Update(GameTime gameTime, StateBasedGame container)
        {
            switch (currentState)
            {
                case GAMEPLAYSTATES.START:
                    UpdateStart(gameTime.ElapsedGameTime.Milliseconds);
                    break;
                case GAMEPLAYSTATES.PLAY:
                    UpdatePlay(gameTime.ElapsedGameTime.Milliseconds, container);
                    break;
                case GAMEPLAYSTATES.END:
                    UpdateEnd(gameTime.ElapsedGameTime.Milliseconds);
                    break;
                case GAMEPLAYSTATES.ENDRESULT:
                    UpdateResult(gameTime.ElapsedGameTime.Milliseconds, container);
                    break;
                default:
                    throw new NullReferenceException("GAMEPLAYSTATE is invalid: " + currentState);
            }
        }
        public override void Draw(SpriteBatch batch, GameTime gameTime, StateBasedGame container)
        {

            DrawMap(batch, container.GraphicsDevice);
            DrawEntities(batch);
            DrawPlayer(batch);

            if (currentState == GAMEPLAYSTATES.END || currentState == GAMEPLAYSTATES.ENDRESULT)
                Globals.DrawBlendOut(batch, startTimer, 2000);

            if (currentState == GAMEPLAYSTATES.ENDRESULT)
                DrawEndResults(batch);
            hud.DrawHUD(batch);

            if (startTimer > 0)
            {
                if (currentState == GAMEPLAYSTATES.START)
                    Globals.DrawBlendIn(batch, startTimer, 4000);
            }
           
        }

        // internal updates
        private void UpdateStart(int delta)
        {
            if (startTimer <= 0)
            {
                musicManager.PlayGameplayMusic();
                cam.CamSpeed = 0.8f;
                currentState = GAMEPLAYSTATES.PLAY;
            }
            else
            {
                if (startTimer >= 4000)
                {
                    MusicManager.Get().PlaySound("start", 0.5f);
                    startTimer -= delta;
                }
                else
                    startTimer -= delta;
            }
            cam.Update(delta);

        }
        private void UpdatePlay(int delta, StateBasedGame container)
        {
            pacman.Update(Keyboard.GetState(), delta);
            cam.Update(delta);
            survialManager.Update(delta);

            if (Keyboard.GetState().IsKeyDown(Keys.Escape))
            {
                startTimer = 2000;
                survialManager.NullCombo();
                currentState = GAMEPLAYSTATES.END;
                musicManager.PlayWinMusic();
            }

               //container.EnterState(Globals.MENUSTATE);

            if (/*musicManager.isSongStopped()*/ false)
            {
                startTimer = 2000;
                currentState = GAMEPLAYSTATES.END;
            }
               
        }
        private void UpdateEnd(int delta)
        {
            if (startTimer <= 0)
                currentState = GAMEPLAYSTATES.ENDRESULT;
            else
            {
                startTimer -= delta;
                hud.UpdateEnd(delta, startTimer, 2000);
            }

                
        }
        private void UpdateResult(int delta, StateBasedGame container)
        {
            if (Keyboard.GetState().IsKeyDown(Keys.Enter))
            {
                container.EnterState(Globals.MENUSTATE);
                Globals.HIGHSCORE = survialManager.GetPoints();
                Globals.WriteHighscore();
            }
                
        }

        // internal draws
        private void DrawPlayer(SpriteBatch batch)
        {
            if (currentState != GAMEPLAYSTATES.START)
            {
                batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, cam.TranslatePacman());
                pacman.DrawPacman(batch);
                batch.End();
            }
        }
        private void DrawEntities(SpriteBatch batch)
        {
            batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, cam.TranslateMapAndEntities());
            factory.DrawEnteties(batch);

            if (currentState == GAMEPLAYSTATES.START)
                batch.DrawSprite(pacman.PacmanAnimation, new Vector2(50 * 16 + 8, 50 * 16 + 8) - cam.Position, 0, 0, 0);

            batch.End();
        }
        private void DrawMap(SpriteBatch batch, GraphicsDevice gDivice)
        {
            batch.Begin(SpriteSortMode.BackToFront, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, cam.TranslateMapAndEntities());
            gameMap.Draw(batch, new Rectangle(0, 0, (int)(gDivice.Viewport.Width / cam.Scale),
                                                    (int)(gDivice.Viewport.Height / cam.Scale)), cam.Position);
            batch.End();
        }
        private void DrawEndResults(SpriteBatch batch)
        {
            batch.Begin();
            batch.Draw(highscore, new Vector2((Globals.SCREENSIZE.X / 2) - (highscore.Width / 2), (Globals.SCREENSIZE.Y / 2) - (highscore.Height / 2) - 16), Color.White);
            batch.End();
        }

    }
}
