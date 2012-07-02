using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace XNAPLUS
{
    /// <summary>
    /// Container class for state based gaming sytems
    /// </summary>
    public class StateBasedGame : Game
    {
        private Dictionary<int, BasicGameState> states;
        private BasicGameState currentState;
        private BasicGameState nextState;

        /// <summary>
        /// Gets the device manager.
        /// </summary>
        public GraphicsDeviceManager Graphics { set; get; }
        /// <summary>
        /// Gets the current sprite batch.
        /// </summary>
        public SpriteBatch SpriteBatch { set; get; }
        /// <summary>
        /// true if states are switching.
        /// </summary>
        public bool SwitchingStates { set; get; }
        /// <summary>
        /// true if we fade in, false if we fade out.
        /// </summary>
        public bool SwitchType { set; get; }
        /// <summary>
        /// time for fade in and out. 
        /// </summary>
        public int SwitchTimer { set; get; }

        /// <summary>
        /// Creates a new StateBasedGame.
        /// </summary>
        public StateBasedGame()
        {
            Graphics = new GraphicsDeviceManager(this);

            SwitchingStates = true;
            SwitchType = true;
            SwitchTimer = 1000;
            states = new Dictionary<int, BasicGameState>();
            currentState = null;
            nextState = null;
        }

        /// <summary>
        /// Add a new state to the container.
        /// if the satte is the first, which will be added to the 
        /// list then this state becomes the current state.
        /// </summary>
        /// <param name="state">the state to add</param>
        public void  AddState(BasicGameState state)
        {
            state.Init(this);
            if (currentState == null)
            {
                currentState = state;
                currentState.Enter(this);
            }
            states.Add(state.GetStateID(), state);
        }
        /// <summary>
        /// Enters a state.
        /// </summary>
        /// <param name="id"> the id of the state</param>
        public void EnterState(int id)
        {
            currentState.Exit(this);
            nextState = states[id];
            SwitchingStates = true;
            SwitchType = false;
            SwitchTimer = 1000;
        }
        /// <summary>
        /// Sets a State. Note that this is an instant change, no transitioning.
        /// </summary>
        /// <param name="id"></param>
        public void SetState(int id)
        {
            currentState = states[id];
        }

        /// <summary>
        /// Called for updates.
        /// </summary>
        /// <param name="gameTime"> the game time of the current context</param>
        protected override void Update(GameTime gameTime)
        {
            //
            if (SwitchingStates)
            {
                if (SwitchTimer <= 0)
                {
                    if (!SwitchType)
                    {
                        
                        currentState = nextState;
                        currentState.Enter(this);
                        SwitchType = true;
                        SwitchTimer = 1000;
                    }
                    else
                        SwitchingStates = false;
                }
                else
                    SwitchTimer -= gameTime.ElapsedGameTime.Milliseconds;
            }
            else
            {
                currentState.Update(gameTime, this);
            }
            base.Update(gameTime);
        }
        /// <summary>
        /// Called for render
        /// </summary>
        /// <param name="gameTime"> the game time of the current context</param>
        protected override void Draw(GameTime gameTime)
        {
            currentState.Draw(SpriteBatch, gameTime, this);

            base.Draw(gameTime);
        }
        public BasicGameState GetState(int id)
        {
            return states[id];
        }
        

    }
}
