﻿using Project1;
using Project1.src.Physics;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.src
{
    public class Brace : Game
    {
        public GraphicsDeviceManager graphicsDeviceManager;
        public SpriteFont DefaultFont { get; private set; }
        
        public InputManager input;
        private FPSRenderer fpsRenderer;
        public Camera Camera { get; private set; }
        private Actor[] actors;

        public Brace()
        {
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            input = new InputManager(this);

            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            // Load the font
            DefaultFont = Content.Load<SpriteFont>("Arial16");

            // Create FPS renderer
            fpsRenderer = new FPSRenderer(this);

            // Load camera and models
            actors = InitializeActors();
            Camera = new Camera(this, new OriginTrackable()); // Give this an actor

            base.LoadContent();
        }

        private Actor[] InitializeActors()
        {
            var newActors = new Actor[] {
                new GameLogic.Landscape(this)
            };

            return newActors;
        }

        protected override void Update(GameTime gameTime)
        {
            // Handle base.Update
            base.Update(gameTime);

            foreach (Actor actor in actors)
            {
                actor.Update(gameTime);
            }

            StepPhysicsModel(gameTime);

            // Update the camera
            Camera.Update(gameTime);
        }

        private void StepPhysicsModel(GameTime gameTime)
        {
            //PhysicsEngine.step(gameTime, actors);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.CadetBlue);

            foreach (Actor actor in actors)
            {
                actor.Draw(graphicsDeviceManager.GraphicsDevice, Camera.View, Camera.Projection);
            }

            // Show FPS
            fpsRenderer.Draw();

            // Handle base.Draw
            base.Draw(gameTime);
        }
    }
}
