using System;
using System.Collections.Generic;
using System.Linq;
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
using DAPOD_HME.States;

namespace DAPOD_HME
{
    public class DAPOD_STARTER : StateBasedGame
    {

       

        public DAPOD_STARTER() : base()
        {
            Content.RootDirectory = "Content";
        }

        // init game properties 
        protected override void Initialize()
        {
            this.IsMouseVisible = true;
            Graphics.IsFullScreen = false;
            Graphics.PreferredBackBufferWidth = 800;
            Graphics.PreferredBackBufferHeight = 600;
            Graphics.ApplyChanges();

            Window.Title = "Devil's Aramageddon Pacamari of Death - Heavy Metal Edition | Ver. 1.0.0.4";


            

            base.Initialize();



        }

        // load images, sounds etc.
        protected override void LoadContent()
        {
            Globals.loadTextures(Content);
            SpriteBatch = new SpriteBatch(GraphicsDevice);
            MusicManager.Get().Init(Content);
            Camera.Get().Init();

            Globals.ReadHighscore();

            // load all states
            AddState(new GamePlayState(Globals.GAMEPLAYSTATE));
            AddState(new MenuState(Globals.MENUSTATE));
            SetState(Globals.MENUSTATE);

        }
        // unload images, sound etc.
        protected override void UnloadContent()
        {

        }
        protected override void Update(GameTime gameTime)
        {
            base.Update(gameTime);



            if (Keyboard.GetState().IsKeyDown(Keys.F12))
            {
                Graphics.IsFullScreen = Graphics.IsFullScreen;
                Graphics.ToggleFullScreen();
            }
        }
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            base.Draw(gameTime);

            if (SwitchingStates)
            {
                if (SwitchType)
                    Globals.DrawBlendIn(SpriteBatch, SwitchTimer, 2000);
                else
                    Globals.DrawBlendOut(SpriteBatch, SwitchTimer, 2000);
            }
        }
    }
}
