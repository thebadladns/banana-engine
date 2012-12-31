using System;
using System.Collections.Generic;
using System.Linq;

using System.IO;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.GamerServices;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace BananaEngine
{
    /// <summary>
    /// This is the main type for your game
    /// </summary>
    public class BananaGame : Microsoft.Xna.Framework.Game
    {
        // Rendering
        protected GraphicsDeviceManager graphics;
        protected SpriteBatch spriteBatch;
        public GraphicsDevice graphicsDevice { get { return GraphicsDevice; } }
        protected uint horizontalZoom, verticalZoom;
        public SpriteFont gameFont;

        // Input
        public static GameInput input = new GameInput();
        
        // Gameplay
        protected GameState world;

        // Time flow
        protected double millisecondsPerFrame = 17;
        protected double timeSinceLastUpdate = 0;

        public BananaGame()
        {
            horizontalZoom = 3;
            verticalZoom = 3;

            graphics = new GraphicsDeviceManager(this);

            Resolution.Init(ref graphics, 256, 240);
            Resolution.SetVirtualResolution(256, 240);
            Resolution.SetResolution(256*3, 240*3, false);

            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        protected override void Initialize()
        {
            // TODO: Create Initial Level
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

            // Load game-wide available sprite fonts
            // gameFont = Content.Load<SpriteFont>("font0");

            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// all content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            // Control time flow (30fps)
            timeSinceLastUpdate += gameTime.ElapsedGameTime.TotalMilliseconds;
            if (timeSinceLastUpdate < millisecondsPerFrame)
                return;

            timeSinceLastUpdate = 0;

            // Update inputstate
            input.update();

            // Allows the game to exit
            if (/*input.pressed(Buttons.Back) ||*/ input.pressed(Keys.Escape))
                this.Exit();
            else if (input.pressed(Keys.F4))
            {
                int rw, rh;
                if (graphics.IsFullScreen)
                {
                    rw = 256 * (int) horizontalZoom;
                    rh = 240 * (int) verticalZoom;
                }
                else
                {
                    rw = GraphicsDevice.DisplayMode.Width;
                    rh = GraphicsDevice.DisplayMode.Height;
                }

                Resolution.SetResolution(rw, rh, !graphics.IsFullScreen);
            }
            else if (input.pressed(Keys.Add))
            {
                millisecondsPerFrame += 5.0;
            }
            else if (input.pressed(Keys.Subtract))
            {
                millisecondsPerFrame -= 5.0;
            }

            if (input.pressed(Buttons.Y))
                BananaConfig.DEBUG = !BananaConfig.DEBUG;
            
            if (world != null)
                world.update(gameTime);

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            Resolution.BeginDraw();
            // Generate resolution render matrix 
            Matrix matrix = Resolution.getTransformationMatrix();
            
            // Render world
            world.render(gameTime, spriteBatch, matrix);

            // Finish drawing
            spriteBatch.End();

            base.Draw(gameTime);
        }

        public void changeWorld(GameState newWorld)
        {
            if (world != null)
                world.end();

            world = newWorld;
            world.game = this;

            newWorld.init();   
        }

        public int count = 0;
        public void screenshot()
        {
            count += 1;
            string counter = count.ToString();

            int w = GraphicsDevice.PresentationParameters.BackBufferWidth;
            int h = GraphicsDevice.PresentationParameters.BackBufferHeight;

            //force a frame to be drawn (otherwise back buffer is empty) 
            Draw(new GameTime());

            //pull the picture from the buffer 
            int[] backBuffer = new int[w * h];
            GraphicsDevice.GetBackBufferData(backBuffer);

            //copy into a texture 
            Texture2D texture = new Texture2D(GraphicsDevice, w, h, false, GraphicsDevice.PresentationParameters.BackBufferFormat);
            texture.SetData(backBuffer);

            //save to disk 
            Stream stream = File.OpenWrite(counter + ".png");

            texture.SaveAsPng(stream, w, h);
            stream.Dispose();

            texture.Dispose();
        }
    }
}
