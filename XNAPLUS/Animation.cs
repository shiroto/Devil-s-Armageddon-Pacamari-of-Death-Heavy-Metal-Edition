using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;

namespace XNAPLUS
{
    /// <summary>
    /// This Class extends the SpriteSheet and draw each tile one after another, 
    /// set by the duration. 
    /// There is also a ping pong function for forward/backward animations.
    /// Furthermore you can set if the animation should stop when reaching it's last frame
    /// (useful for explosions e.g., does not work if pingpong is true)
    /// Use reset to start the animation again.
    /// 
    /// Notice that Update(int delta) [milliseconds] musst be called on every game loop.
    /// 
    /// </summary>
    public class Animation : SpriteSheet
    {
        public int CurrentFrame { get; set; }
        public int Duration { get; set; }
        public int TotalFrames { get; set; }
        public bool PingPong { get; set; }
        public bool StopOnLastFrame { get; set; }

        private int lastUpdate;
        private bool runsBackwards;
        private bool isStopped;

        public Animation(Texture2D texture, int tileWidth, int tileHeight, int duration): base(texture, tileWidth, tileWidth)
        {
            Duration = duration;
            lastUpdate = duration;
            CurrentFrame = 0;
            TotalFrames = TilesAcross * TilesDown;
        }

        public void Update(int delta)
        {
            if(isStopped && !PingPong)
                return;

            if (lastUpdate <= 0)
            {
                if (PingPong)
                {
                    updatePingPongAnimation();
                }
                else
                {
                    updateForwardAnimation();
                }
                lastUpdate = Duration;
            }
            else
                lastUpdate -= delta;
        }
        private void updateForwardAnimation()
        {
            if (CurrentFrame >= TotalFrames - 1)
            {
                if(StopOnLastFrame)
                    isStopped = true;
                 CurrentFrame = 0;
            }
               
            else
                CurrentFrame++;
        }
        private void updatePingPongAnimation()
        {
            if (runsBackwards)
            {
                if (CurrentFrame == 0)
                {
                    CurrentFrame = 0;
                    runsBackwards = false;
                }
                else
                    CurrentFrame--;
            }
            else
            {
                if (CurrentFrame >= TotalFrames - 1)
                {
                    CurrentFrame = TotalFrames - 1;
                    runsBackwards = true;
                }
                else
                    CurrentFrame++;
            }
        }

        public void Reset()
        {
            Reset(Duration);
        }
        public void Reset(int firstWaitTime)
        {
            lastUpdate = firstWaitTime;
            CurrentFrame = 0;
            isStopped = false;
        }
        public void ResetTo(int frame)
        {
            ResetTo(frame, Duration);
        }
        public void ResetTo(int frame, int firstWaitTime)
        {
            if (frame < 0)
                throw new IndexOutOfRangeException("frame number is below 0: " + frame);
            if (frame >= TotalFrames - 1)
                throw new IndexOutOfRangeException("frame number is greater then total frame number: " + frame);

            lastUpdate = firstWaitTime;
            CurrentFrame = frame;
            isStopped = false;
        }
        public override String ToString()
        {
            StringBuilder sb = new StringBuilder();
            sb.Append("[TotalFrames: " + TotalFrames);
            sb.Append(", Duration: " + Duration);
            sb.Append(", CurrentFrame: " + CurrentFrame);
            sb.Append(", PingPong: " + PingPong);
            sb.Append(", StopOnLastFrame: " + StopOnLastFrame);
            sb.Append(", Sheet: " + base.ToString());

            return sb.ToString();
        }
    }
}
