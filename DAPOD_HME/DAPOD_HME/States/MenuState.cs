using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using XNAPLUS;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using DAPOD_HME.Core;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Threading;

namespace DAPOD_HME.States
{
    class MenuState : BasicGameState
    {

        private Camera cam = Camera.Get();

        private Texture2D background, blend, highscore;
        private Texture2D title_devil, title_arma, title_pacamari, title_of, title_death, title_hme;
        private Texture2D menu_start, menu_fullscreen, menu_windowed, menu_end;

        private Vector2 testPos = new Vector2(0, 0);

        private KeyboardState oldState;

        private int introWaitTimer, flashTimer, menuTimer;
        private MENUSTATES currentState;
        private bool pacmanState;
        private int menuSelection;
        
        private enum MENUSTATES
        {
            START_0, START_1, START_2, START_3, START_4, START_5, START_6, START_7, SELECT
        }

        public MenuState(int stateID) : base(stateID) { }

        public override void Init(StateBasedGame container)
        {
            background = container.Content.Load<Texture2D>("Graphics/Menu/bgmenu");
            title_devil = container.Content.Load<Texture2D>("Graphics/Menu/start_1");
            title_arma = container.Content.Load<Texture2D>("Graphics/Menu/start_2");
            title_pacamari = container.Content.Load<Texture2D>("Graphics/Menu/start_3");
            title_of = container.Content.Load<Texture2D>("Graphics/Menu/start_4");
            title_death = container.Content.Load<Texture2D>("Graphics/Menu/start_5");
            title_hme = container.Content.Load<Texture2D>("Graphics/Menu/start_6");
            menu_start = container.Content.Load<Texture2D>("Graphics/Menu/start");
            menu_fullscreen = container.Content.Load<Texture2D>("Graphics/Menu/fullscreen");
            menu_windowed = container.Content.Load<Texture2D>("Graphics/Menu/windowed");
            menu_end = container.Content.Load<Texture2D>("Graphics/Menu/end");
            blend = container.Content.Load<Texture2D>("Graphics/BlendWhite");
            highscore = container.Content.Load<Texture2D>("Graphics/Highscore");

            currentState = MENUSTATES.START_0;
            introWaitTimer = 1700;
            flashTimer = 1000;
            cam.ShakeScreen(3000, 10);
            menuSelection = 0;
            menuTimer = 200;
            oldState = Keyboard.GetState();
           

        }
        public override void Enter(StateBasedGame container)
        {
            menuSelection = 0;
            currentState = MENUSTATES.SELECT;
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.5f;
            MusicManager.Get().PlayMenuMusic();
            flashTimer = 1000;
            menuTimer = 200;
            oldState = Keyboard.GetState();
        }
        public override void Exit(StateBasedGame container) {}

        public override void Update(GameTime gameTime, StateBasedGame container)
        {
            switch (currentState)
            {
                case MENUSTATES.START_0:
                    UpdateMusicIntro(gameTime.ElapsedGameTime.Milliseconds);
                    break;
                case MENUSTATES.START_1:
                case MENUSTATES.START_2:
                case MENUSTATES.START_3:
                case MENUSTATES.START_4:
                case MENUSTATES.START_5:
                case MENUSTATES.START_6:
                    if (Keyboard.GetState().IsKeyDown(Keys.Escape))
                    {
                        MusicManager.Get().PlaySound("cursor", 1f);
                        currentState = MENUSTATES.SELECT;
                        return;
                    }
                    UpdateIntro(gameTime.ElapsedGameTime.Milliseconds);
                    break;
                case MENUSTATES.SELECT:
                    UpdateSelect(gameTime.ElapsedGameTime.Milliseconds, container);
                    UpdateFlash(gameTime.ElapsedGameTime.Milliseconds);
                    break;
            }
            
        }
        public override void Draw(SpriteBatch batch, GameTime gameTime, StateBasedGame container)
        {
            if (container.SwitchingStates)
                return;
            switch (currentState)
            {
                case MENUSTATES.START_1:
                    DrawIntro(batch, title_devil);
                    break;
                case MENUSTATES.START_2:
                    DrawIntro(batch, title_arma);
                    break;
                case MENUSTATES.START_3:
                     DrawIntro(batch, title_pacamari);
                    break;
                case MENUSTATES.START_4:
                     DrawIntro(batch, title_of);
                    break;
                case MENUSTATES.START_5:
                     DrawIntro(batch, title_death);
                    break;
                case MENUSTATES.START_6:
                     DrawIntro(batch, title_hme);
                    break;
                case MENUSTATES.SELECT:
                    DrawSelect(batch, container);
                    break;
            }
        }

        // internal updates
        private void UpdateMusicIntro(int delta)
        {
            if (introWaitTimer == 1700)
            {
                MusicManager.Get().PlayMenuMusic();
                introWaitTimer -= delta;
            }
            else
            {
                if (introWaitTimer <= 0)
                {
                    currentState = MENUSTATES.START_1;
                    introWaitTimer = 1700;
                    cam.ShakeScreen(3000, 10);
                }
                else
                    introWaitTimer -= delta;
            }
            
        }
        private void UpdateIntro(int delta)
        {
            cam.Update(delta);
            if (introWaitTimer <= 0)
            {
                switch (currentState)
                {
                    case MENUSTATES.START_1:
                        currentState = MENUSTATES.START_2;
                        break;
                    case MENUSTATES.START_2:
                        currentState = MENUSTATES.START_3;
                        break;
                    case MENUSTATES.START_3:
                        currentState = MENUSTATES.START_4;
                        break;
                    case MENUSTATES.START_4:
                        currentState = MENUSTATES.START_5;
                        break;
                    case MENUSTATES.START_5:
                        currentState = MENUSTATES.START_6;
                        break;
                    case MENUSTATES.START_6:
                        MediaPlayer.Volume = 0.5f;
                        currentState = MENUSTATES.SELECT;
                        break;
                }
                introWaitTimer = 2000;
                cam.ShakeScreen(4000, 10);
            }
            else
                introWaitTimer -= delta;      
        }
        private void UpdateSelect(int delta, StateBasedGame container)
        {
            if(menuTimer <= 0)
            {
                pacmanState = !pacmanState;
                menuTimer = 200;
            }
            else
                menuTimer -= delta;

            KeyboardState newState = Keyboard.GetState();

            if (newState.IsKeyDown(Keys.Down) && !oldState.IsKeyDown(Keys.Down))
            {
                MusicManager.Get().PlaySound("cursor", 1f);
                menuSelection++;
                if (menuSelection > 2)
                    menuSelection = 0;
            }
            if (newState.IsKeyDown(Keys.Up) && !oldState.IsKeyDown(Keys.Up))
            {
                MusicManager.Get().PlaySound("cursor", 1f);
                menuSelection--;
                if (menuSelection < 0)
                    menuSelection = 2;
            }
            if (newState.IsKeyDown(Keys.Enter) && !oldState.IsKeyDown(Keys.Enter))
            {
                MusicManager.Get().PlaySound("enter", 1f);
                switch (menuSelection)
                {
                    case 0:
                        MediaPlayer.Stop();
                        container.EnterState(Globals.GAMEPLAYSTATE);
                        break;
                    case 1:
                        container.Graphics.ToggleFullScreen();
                        break;
                    case 2:
                        Thread.Sleep(600);
                        container.Exit();
                        break;
                }
            }

            oldState = newState;
        }
        private void UpdateFlash(int delta)
        {
            if (flashTimer <= 0)
            {
                return;
            }
            else
                flashTimer -= delta;
        }

        // internal draws
        private void DrawIntro(SpriteBatch batch, Texture2D image)
        {
            Vector2 pos = new Vector2((Globals.SCREENSIZE.X / 2) - (image.Width / 2), (Globals.SCREENSIZE.Y / 2) - (image.Height / 2));
            if (currentState != MENUSTATES.START_6)
                pos += new Vector2(30, 0);
            
            batch.Begin(SpriteSortMode.BackToFront,BlendState.AlphaBlend,SamplerState.PointClamp, null, null, null, cam.TranslateShake());
            batch.Draw(image, pos, Color.White);
            batch.End();
        }
        private void DrawSelect(SpriteBatch batch, StateBasedGame container)
        {

            Vector2 titelpos = new Vector2(15, 13);

            batch.Begin();
            batch.Draw(background, Vector2.Zero, Color.White);
            batch.Draw(title_devil, titelpos, Color.White);
            batch.Draw(title_arma, titelpos, Color.White);
            batch.Draw(title_pacamari, titelpos, Color.White);
            batch.Draw(title_of, titelpos, Color.White);
            batch.Draw(title_death, titelpos, Color.White);
            batch.Draw(title_hme, new Vector2(99, 271), Color.White);

            batch.Draw(menu_start, new Vector2(551, 259), Color.White);
            if(container.Graphics.IsFullScreen)
                batch.Draw(menu_windowed, new Vector2(552, 306), Color.White);
            else
            batch.Draw(menu_fullscreen, new Vector2(552, 306), Color.White);

            batch.Draw(menu_end, new Vector2(553, 352), Color.White);

            batch.End();

            batch.Begin(SpriteSortMode.Texture, BlendState.AlphaBlend, SamplerState.PointClamp, null, null, null, Matrix.CreateScale(2));
            if(pacmanState)
                batch.DrawSprite(Player.Get().PacmanAnimation, new Vector2(264, 140 + (23 * menuSelection)), 0, 0, 0);
            else
                batch.DrawSprite(Player.Get().PacmanAnimation, new Vector2(264, 140 + (23 * menuSelection)), 0, 1, 0);

            batch.End();

            DrawHigscore(batch);

            DrawFlash(batch);

        }
        private void DrawFlash(SpriteBatch batch)
        {
            if (flashTimer <= 0)
                return;

            float alpha = 1 - 1 / 100f * (((float)1000 - (float)flashTimer) / (float)1000 * 100f);

            batch.Begin(SpriteSortMode.Texture, BlendState.Additive);
            batch.Draw(blend, new Vector2(-20, -20), new Color(1, 1, 1, alpha));
            batch.End();
        }
        private void DrawHigscore(SpriteBatch batch)
        {
            batch.Begin();
            batch.Draw(highscore, new Vector2(Globals.SCREENSIZE.X - highscore.Width - 22, 20), Color.White);

            HUD.Get().DrawNumberString(batch, Globals.HIGHSCORE.ToString(), 
                new Vector2(Globals.SCREENSIZE.X - (Globals.HIGHSCORE.ToString().Length * 16) - 15, 47));

            batch.End();
            


        }

    }
}
