#region File Description
//-----------------------------------------------------------------------------
// Menu.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input.Touch;

namespace InputToyWP7
{
    class Menu
    {
        private const int BUTTONMARGIN = 4;

        private Texture2D pauseBtnImg;
        private Texture2D playBtnImg;
        private Vector2 pauseBtnPos;

        private Texture2D helpBtnImg;
        private Vector2 helpBtnPos;

        private bool paused;
        
        public Menu()
        {
            paused = false;
        }

        public void loadContent(ContentManager cm, GraphicsDevice gd)
        {
            pauseBtnImg = cm.Load<Texture2D>("btnPause");
            playBtnImg = cm.Load<Texture2D>("btnPlay");
            pauseBtnPos = new Vector2(
                    0, gd.Viewport.Height - pauseBtnImg.Height);

            helpBtnImg = cm.Load<Texture2D>("btnHelp");
            helpBtnPos = new Vector2(
                    gd.Viewport.Width - helpBtnImg.Width,
                    gd.Viewport.Height - helpBtnImg.Height);
        }

        public bool isPaused()
        {
            return paused;
        }

        public bool handleInput(TouchLocation tl, Instructions instructions)
        {
            if (tl.Position.Y > (pauseBtnPos.Y - BUTTONMARGIN))
            {
                if (tl.Position.X < (pauseBtnImg.Width + BUTTONMARGIN))
                {
                    // only change on the first press.
                    if (tl.State == TouchLocationState.Pressed)
                    {
                        // toggle the pause state.
                        paused = !paused;
                    }
                }
                else if (tl.Position.X > (helpBtnPos.X - BUTTONMARGIN))
                {
                    // only change on the first press.
                    if (tl.State == TouchLocationState.Pressed)
                    {
                        // show the game instructions.
                        instructions.show();
                    }
                }
            
                return true;
            }

            return false;
        }

        public void draw(SpriteBatch sb)
        {
            // draw the buttons.
            sb.Draw((paused ? playBtnImg : pauseBtnImg), pauseBtnPos, Color.White);
            sb.Draw(helpBtnImg, helpBtnPos, Color.White);
        }
    }
}
