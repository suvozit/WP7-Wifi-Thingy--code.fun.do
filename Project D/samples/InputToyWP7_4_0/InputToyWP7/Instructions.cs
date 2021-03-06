#region File Description
//-----------------------------------------------------------------------------
// Instructions.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;

namespace InputToyWP7
{
    class Instructions
    {
        private const float TIMETOSHOW = 6000.0f;
        private const float FADETIME = 2000.0f;

        public enum State { DISPLAY, FADEOUT, HIDE };
        private State state;
        private float timeRemaining;
        private Texture2D image;
        private Color color;
        private Vector2 pos;

        public Instructions()
        {
            state = State.HIDE;
            color = new Color(255,255,255,255);
        }

        public void loadContent(ContentManager cm, GraphicsDevice gd)
        {
            image = cm.Load<Texture2D>("Instructions");
            pos = new Vector2(
                (gd.Viewport.Width - image.Width) * 0.5f,
                (gd.Viewport.Height - image.Height) * 0.5f);
        }

        public void show()
        {
            state = State.DISPLAY;
            color.A = 255;
            timeRemaining = TIMETOSHOW;
        }

        public bool isVisible()
        {
            return (state != State.HIDE);
        }

        public void update(float etms)
        {
            if (state == State.HIDE)
            {
                return;
            }

            timeRemaining -= etms;
            if(timeRemaining < 0)
            {
                state = State.HIDE;
                return;
            }
            else if (timeRemaining < FADETIME)
            {
                // fading out
                state = State.FADEOUT;
                color.A = (byte)(255.0f * timeRemaining / FADETIME);
            }
        }

        public void draw(SpriteBatch sb)
        {
            if (state == State.HIDE)
            {
                return;
            }

            sb.Draw(image, pos, color);
        }
    }
}
