using Brace.GameLogic;
using Brace.Utils;
using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using SharpDX.Toolkit.Input;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace
{
    using Physics;

    public class BraceGame : Game
    {
        public bool paused = true;
        public GraphicsDeviceManager graphicsDeviceManager;
        public SpriteFont DefaultFont { get; private set; }

        public InputManager input { get; private set; }
        private FPSRenderer fpsRenderer;
        public Camera Camera { get; private set; }
        private List<Actor> actors;
        private static BraceGame game;

        public Physics.PhysicsEngine physicsWorld;

        //Our actors
        private GameLogic.Landscape landscape;
        //Rendering stuff
        private Effect unitShader;
        private Effect landscapeEffect;
        private TrackingLight playerLamp;

        public static BraceGame get() 
        {
            if (game == null)
            {
                game = new BraceGame();
            }
            return game;

        }

        private BraceGame()
        {
            graphicsDeviceManager = new GraphicsDeviceManager(this);
            input = new InputManager(this);

            // Setup the relative directory to the executable directory
            // for loading contents with the ContentManager
            Content.RootDirectory = "Content";
        }

        protected override void LoadContent()
        {
            //Setup Singleton
            game = this;

            // Load the font
            DefaultFont = Content.Load<SpriteFont>("Arial16");

            // Create FPS renderer
            fpsRenderer = new FPSRenderer(this);
            
            // Create PhysicsWorld
            physicsWorld = new PhysicsEngine();


            // Load camera and models
            LoadAssets();
            actors = InitializeActors();

            Camera = new Camera(this, (Unit)actors[0]); // Give this an actor
            Camera.SetViewType(Brace.Camera.ViewType.Follow);
            playerLamp = new TrackingLight((Unit)actors[0]);
            //Load shaders
            unitShader = Content.Load<Effect>("CubeCelShader");
            landscapeEffect = game.Content.Load<Effect>("LandscapeCelShader");
            base.LoadContent();
        }

        private void LoadAssets()
        {
            Assets.spaceship = Content.Load<Model>("Cube");
            Assets.cube = Content.Load<Model>("Cube");
        }

        private List<Actor> InitializeActors()
        {
            List<Actor> newActors = new List<Actor>();
            newActors.Add(new Cube(Vector3.Zero));
            for (int i = -3; i < 3; ++i)
            {
                for (int j = -3; j < 3; ++j)
                {
                    newActors.Add(new Cube(new Vector3(i,10,j)));
                }
            }


            landscape = new GameLogic.Landscape(this);

            return newActors;
        }

        public void Start()
        {
            this.paused = false;
        }

        protected override void Update(GameTime gameTime)
        {
            // Handle base.Update
            base.Update(gameTime);
            input.Update();

            if (!paused)
            {
                foreach (Actor actor in actors)
                {
                    if (actor.doomed)
                    {
                        actors.Remove(actor);
                    }
                    else
                    {
                        actor.Update(gameTime);
                    }

                }

                StepPhysicsModel(gameTime);


                //Update tracking lights
                playerLamp.Update(gameTime);

                // Update the camera 
                Camera.Update(gameTime);
               

                // Now update the shaders
                //First the unit shader
                unitShader.Parameters["View"].SetValue(Camera.View);
                unitShader.Parameters["Projection"].SetValue(Camera.Projection);
                unitShader.Parameters["cameraPos"].SetValue(Camera.position);
                unitShader.Parameters["lightPntPos"].SetValue(playerLamp.lightPntPos);

                //Then the landscape shader
                landscapeEffect.Parameters["lightPntPos"].SetValue(playerLamp.lightPntPos);
                landscapeEffect.Parameters["View"].SetValue(Camera.View);
                landscapeEffect.Parameters["Projection"].SetValue(Camera.Projection);
                landscapeEffect.Parameters["cameraPos"].SetValue(Camera.position);
            }
        }

        private void StepPhysicsModel(GameTime gameTime)
        {
            float timeInSeconds = gameTime.ElapsedGameTime.Milliseconds;
            timeInSeconds /= 1000;
            physicsWorld.step(timeInSeconds);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);

            //First draw the landscape
            landscape.Draw(graphicsDeviceManager.GraphicsDevice, Camera.View, Camera.Projection, landscapeEffect);

            //Then teh actors
            foreach (Actor actor in actors)
            {
                actor.Draw(graphicsDeviceManager.GraphicsDevice, Camera.View, Camera.Projection,unitShader);
            }

            // Show FPS
            fpsRenderer.Draw();

            // Handle base.Draw
            base.Draw(gameTime);
        }
    }
}
