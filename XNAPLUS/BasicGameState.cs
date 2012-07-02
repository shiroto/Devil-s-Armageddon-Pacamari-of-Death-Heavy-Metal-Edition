using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;

namespace XNAPLUS
{
    /// <summary>
    /// Abstract base class for a state based game.
    /// </summary>
    public abstract class BasicGameState
    {
        protected int StateID;

        public BasicGameState(int stateID)
        {
            StateID = stateID;
        }

        /// <summary>
        /// Called on adding into the list of states
        /// </summary>
        /// <param name="container"> the container for the states</param>
        abstract public void Init(StateBasedGame container);
        /// <summary>
        /// Called when entering this state
        /// </summary>
        /// <param name="container"> the container for the states</param>
        abstract public void Enter(StateBasedGame container);
        /// <summary>
        /// Called when chaning to another state.
        /// </summary>
        /// <param name="container"> the container for the states</param>
        abstract public void Exit(StateBasedGame container);
        /// <summary>
        /// Called for logic updates
        /// </summary>
        /// <param name="gameTime"> the game time of the current context</param>
        /// <param name="container"> the container for the states</param>
        abstract public void Update(GameTime gameTime, StateBasedGame container);
        /// <summary>
        /// Called for draw updates
        /// </summary>
        /// <param name="batch"> the batch to use</param>
        /// <param name="gameTime"> the game time of the current context</param>
        /// <param name="container"> the container for the states</param>
        abstract public void Draw(SpriteBatch batch, GameTime gameTime, StateBasedGame container);
        /// <summary>
        /// State getter
        /// </summary>
        /// <returns> the id of this state</returns>
        public int GetStateID()
        {
            return StateID;
        }
    }
}
