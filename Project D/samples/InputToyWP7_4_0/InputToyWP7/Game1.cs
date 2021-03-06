#region File Description
//-----------------------------------------------------------------------------
// Game1.cs
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Input.Touch;
using Microsoft.Xna.Framework.Media;
using Microsoft.Devices.Sensors;

namespace InputToyWP7
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class Game1 : Microsoft.Xna.Framework.Game
    {
        static Random random = new Random();

        class Sparkle
        {
            public Vector2 position;
            public Vector2 speed;
            public float rotation;  // in radians
            public Color color;
            public long birthtime;

            public Sparkle(float x, float y, long time)
            {
                position = new Vector2(x, y);
                rotation = (float)(random.NextDouble() * 2.0 * Math.PI);
                color = Color.White;
                birthtime = time;
            }
        }

        const int SPARKLELIFE = 4000; // ticks
        const int MAXSPARKLES = 100; // maximum number of sparkles, for
                                     // performance tuning.
        const float ACCELFACTOR = 0.01f;
        const float FADEFACTOR = 255.0f / SPARKLELIFE;
        Vector2 screenDimensions = new Vector2(272, 480);

        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;
        Texture2D flareImage;
        Vector2 flareOffset;
        List<Sparkle> sparkles;
        Instructions instructions;
        Menu menu;
        bool accelActive = false;
        Accelerometer accelSensor;
        Vector3 accelReading = new Vector3();

        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            sparkles = new List<Sparkle>();

            instructions = new Instructions();
            menu = new Menu();

            Content.RootDirectory = "Content";

            // Frame rate is 30 fps by default for Windows Phone.
            TargetElapsedTime = TimeSpan.FromSeconds(1 / 30.0);

            // This makes use of the built-in hardware scaler to work with our original (Zune HD) resolution
            graphics.PreferredBackBufferWidth = (int)screenDimensions.X;
            graphics.PreferredBackBufferHeight = (int)screenDimensions.Y;

            // use the whole screen
            graphics.IsFullScreen = true;

            accelSensor = new Accelerometer();

            // Add the accelerometer event handler to the accelerometer sensor.
            accelSensor.ReadingChanged +=
                new EventHandler<AccelerometerReadingEventArgs>(AccelerometerReadingChanged);

            // Start the accelerometer
            try
            {
                accelSensor.Start();
                accelActive = true;
            }
            catch (AccelerometerFailedException e)
            {
                // the accelerometer couldn't be started.  No fun!
                accelActive = false;                
            }
            catch (UnauthorizedAccessException e)
            {
                // This exception is thrown in the emulator-which doesn't support an accelerometer.
                //accelActive = false;                
            }
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before
        /// starting to run.  This is where it can query for any required
        /// services and load any non-graphic related content.  Calling
        /// base.Initialize will enumerate through any components and
        /// initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            flareImage = this.Content.Load<Texture2D>("Flare");
            flareOffset = new Vector2(
                    flareImage.Width * 0.5f, flareImage.Height * 0.5f);
            menu.loadContent(this.Content, this.GraphicsDevice);
            instructions.loadContent(this.Content, this.GraphicsDevice);
            instructions.show();
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to
        /// unload all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // Unload any non ContentManager content here
            // Stop the accelerometer if it's active.
            if (accelActive)
            {
                try
                {
                    accelSensor.Stop();
                }
                catch (AccelerometerFailedException e)
                {
                    // the accelerometer couldn't be stopped now.
                }
            }
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Allows the game to exit
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed)
            {
                this.Exit();
            }

            long ttms = (long)gameTime.TotalGameTime.TotalMilliseconds;

            // elapsed time will be used in updating movement
            float etms = gameTime.ElapsedGameTime.Milliseconds;

            // update the instructions state if needed
            if (instructions.isVisible())
            {
                instructions.update(etms);
            }

            // Process touch events
            TouchCollection touchCollection = TouchPanel.GetState();
            foreach (TouchLocation tl in touchCollection)
            {
                if ((tl.State == TouchLocationState.Pressed)
                        || (tl.State == TouchLocationState.Moved))
                {
                    if (menu.handleInput(tl, instructions))
                    {
                        continue;
                    }

                    // add sparkles based on the touch location
                    sparkles.Add(new Sparkle(tl.Position.X,
                             tl.Position.Y, ttms));

                    // if there are too many sparkles, remove from the head.
                    if (sparkles.Count > MAXSPARKLES)
                    {
                        sparkles.RemoveAt(0);
                    }
                }
            }

             // update the sparkles.  We count from the end of the list in case
            // we need to remove a sparkle (without upsetting the order).
            for (int i = sparkles.Count - 1; i >= 0; i--)
            {
                Sparkle s = sparkles[i];

                if (!menu.isPaused() && ((ttms - s.birthtime) > SPARKLELIFE))
                {
                    sparkles.RemoveAt(i);
                    continue;
                }
                else
                {
                    if (menu.isPaused())
                    {
                        // reset the birthtime based on the alpha, so the
                        // sparkles don't immediately dissapear once fading is
                        // un-paused.
                        s.birthtime = ttms - SPARKLELIFE
                            + (long)(s.color.A / FADEFACTOR);
                    }
                    else
                    {
                        // fade the sparkle a bit, depending on its age.
                        s.color.A = (byte)(255.0f - (ttms - s.birthtime)
                                * FADEFACTOR);
                    }

                    // sparkles can still move, even when paused.
                    if (accelActive)
                    {
                        // accelerate the sparkle depending on accelerometer
                        // action.
                        s.speed.X += accelReading.X * ACCELFACTOR;
                        s.speed.Y += -accelReading.Y * ACCELFACTOR;

                        // move the sparkle based on its speed.
                        s.position.X += s.speed.X * etms;
                        s.position.Y += s.speed.Y * etms;
                        s.rotation += s.speed.Length() * ACCELFACTOR * etms;
                    }
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            spriteBatch.Begin();
            // draw stuff in the background
            if (instructions.isVisible())
            {
                instructions.draw(spriteBatch);
            }

            // draw the sparkles
            foreach (Sparkle s in sparkles)
            {
                // when drawing with a specified origin, the origin affects
                // both the rotation and position parameters.
                spriteBatch.Draw(
                     flareImage, s.position, null, s.color, s.rotation,
                     flareOffset, 1.0f, SpriteEffects.None, 0);
            }

            // draw the foreground (AKA the HUD)
            menu.draw(spriteBatch);

            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void AccelerometerReadingChanged(object sender, AccelerometerReadingEventArgs e)
        {
            accelReading.X =  (float)e.X;
            accelReading.Y = (float)e.Y;
            accelReading.Z = (float)e.Z;
        }
        
        public int getTouchPoints()
        {
            TouchPanelCapabilities tc = TouchPanel.GetCapabilities();
            if(tc.IsConnected)
            {
                return tc.MaximumTouchCount;
            }
            else
            {
                return -1;
            }
        }
    }
}
