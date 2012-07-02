using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DAPOD_HME.Core
{
    class Camera
    {
        private static Camera INSTANCE = new Camera();

        public float CamSpeed { get; set; }
        public float FadeSpeed { get; set; }
        public float ScaleSpeed { get; set; }
        public Point MapSize { get; set; }

        private Vector2 currentPosition;
        private Vector2 targetPosition;
    
        private float currentScale;
        private float targetScale;

        private Random seed;
        private Vector2 rumblePos;
        private float rumbleTime;
        private float currentRumbleTime;
        private float rumblePower;
        private float currentRumblePower;

        // true = normal, false = fade moving
        private bool moveType;

        public static Camera Get()
        {
            return INSTANCE;
        }
        public void Init()
        {
            currentPosition = Vector2.Zero;
            targetPosition = Vector2.Zero;

            currentScale = 2f;
            targetScale = 2f;

            CamSpeed = 0.8f;
            FadeSpeed = 0.005f;
            ScaleSpeed = 0.0001f;

            seed = new Random();
            rumblePos = Vector2.Zero;
            rumbleTime = 0;
            currentRumbleTime = 1;
            rumblePower = 0;
            currentRumblePower = 0;

            moveType = true;

        }
        private Camera() { }

        // delta must be milliseconds
        public void Update(int delta)
        {
            if (moveType)
            {
                InterpolateCam(ref currentPosition.X, ref targetPosition.X, delta);
                InterpolateCam(ref currentPosition.Y, ref targetPosition.Y, delta);
            }
            else
            {
                FadeInterpolate(ref currentPosition.X, ref targetPosition.X, delta);
                FadeInterpolate(ref currentPosition.Y, ref targetPosition.Y, delta);
            }

            InterpolateScale(ref currentScale, ref targetScale, delta);
            InterpolateShake(delta);
        }
        // interpolates to the current value to the target value without motion control
        private void InterpolateCam(ref float current, ref float target, int delta) 
        {
            if (current != target)
            {
                if (current > target)
                {
                    current -= CamSpeed * delta;
                    if (current < target)
                        current = target;
                }
                else
                {
                    current += CamSpeed * delta;
                    if (current > target)
                        current = target;
                }
            }
        }
        private void InterpolateScale(ref float current, ref float target, int delta)
        {
            if (current != target)
            {
                if (current > target)
                {
                    current -= ScaleSpeed * delta;
                    if (current < target)
                        current = target;
                }
                else
                {
                    current += ScaleSpeed * delta;
                    if (current > target)
                        current = target;
                }
            }
        }
        // interpolates the current value to the target value getting slower the nearer 
        // the current value gets to the target value.
        private void FadeInterpolate(ref float current, ref float target, int delta)
        {
            if (current != target)
            {
                current += (target - current) * FadeSpeed * delta;
                if (IsCamCloseToTarget(ref current, ref target))
                    current = target;
            }
        }
        // interpolate the rumble values by random, plus getting smaller
        // rumble the closer the current time equals the rumble time;
        private void InterpolateShake(int delta) 
        {
            if (currentRumbleTime <= rumbleTime)
            {
                currentRumblePower = rumblePower * ((rumbleTime - currentRumbleTime) / rumbleTime);

                rumblePos.X = ((float)seed.NextDouble() - 0.5f) * 2 * currentRumblePower;
                rumblePos.Y = ((float)seed.NextDouble() - 0.5f) * 2 * currentRumblePower;

                rumblePos.X = Globals.Round(rumblePos.X);
                rumblePos.Y = Globals.Round(rumblePos.Y);

                currentRumbleTime += delta;
            }
        }

        //implement into SpriteBatch.Begin().
        public Matrix TranslateShake()
        {
            return Matrix.CreateTranslation(-rumblePos.X, -rumblePos.Y, 0);
        }
        public Matrix TranslateMapAndEntities()
        {
            return Matrix.CreateTranslation(-rumblePos.X, -rumblePos.Y, 0) * Matrix.CreateScale(Scale);
        }
        // for pacman only so he shakes and gets his basic scale
        public Matrix TranslatePacman()
        {
            return Matrix.CreateTranslation(-rumblePos.X / (Player.Get().RelativeSize * 2), -rumblePos.Y / (Player.Get().RelativeSize * 2), 0) * Matrix.CreateScale(2);
        }
        public void ShakeScreen(int time, float power)
        {
            currentRumbleTime = 0;
            rumbleTime = time;
            rumblePower = power;
        }
        public bool IsCamCloseToTarget(ref float current, ref float target)
        {
            float temp;
            temp = current - target;
            if (temp < 0)
                temp *= -1;
            return temp <= 0.1;
        }

        public float Scale
        {
            get { return currentScale; }
            set 
            {
                if (value < 0)
                    targetScale = 0.000001f;
                else
                    targetScale = value;
            }
        }
        public void SetFixScale(float scale)
        {
            currentScale = scale;
            targetScale = scale;
        }
        public void SetFixVector(Vector2 vector)
        {
            currentPosition = vector;
            targetPosition = vector;
        }
        public Vector2 Position
        {
            get { return currentPosition; }
            set { targetPosition = value; }
        }
        public float TargetX
        {
            get { return targetPosition.X; }
            set { targetPosition.X = value; }
        }
        public float TargetY
        {
            get { return targetPosition.Y; }
            set { targetPosition.Y = value; }
        }
        public Point getViewPositionTile()
        {
            return new Point((int)(this.currentPosition.X / 16), (int)(this.currentPosition.Y / 16));
        }
    }
}
