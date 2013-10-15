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
        public GraphicsDeviceManager graphicsDeviceManager;
        public SpriteFont DefaultFont { get; private set; }

        public InputManager input { get; private set; }
        private FPSRenderer fpsRenderer;
        public Camera Camera { get; private set; }
        private bool cameraToggling=false;
        private Actor[] actors;
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
            playerLamp = new TrackingLight((Unit)actors[0]);
            //Load shaders
            unitShader = Content.Load<Effect>("CelShader");
            landscapeEffect = game.Content.Load<Effect>("LandscapeCelShader");
            base.LoadContent();
        }

        private void LoadAssets()
        {
            Assets.spaceship = Content.Load<Model>("Cube");
            Assets.cube = Content.Load<Model>("Cube");
        }

        private Actor[] InitializeActors()
        {
            var newActors = new Actor[] {
                //new Unit(Vector3.UnitY*10, Vector3.Zero, Assets.spaceship),
                new Cube(Vector3.Zero,false),
                new Cube( Vector3.UnitY*20+Vector3.UnitZ,false),
                new Cube( Vector3.UnitY*20-Vector3.UnitZ,false),
                new Cube( Vector3.UnitY*20+Vector3.UnitX,false),
                new Cube( Vector3.UnitY*20-Vector3.UnitX,false),
            };

            landscape = new GameLogic.Landscape(this);

            return newActors;
        }

        protected override void Update(GameTime gameTime)
        {
            // Handle base.Update
            base.Update(gameTime);

            input.Update();

            // Blerrurhhjgsjkdh. Camera toggling with Tab.
            if (input.keyboardState.IsKeyDown(Keys.Tab))
            {
                if (!cameraToggling)
                {
                    Camera.SetViewType((Camera.ViewType)((int)(Camera.CurrentViewType + 1) % Enum.GetNames(typeof(Camera.ViewType)).Length));
                    cameraToggling = true;
                }
            }
            else
            {
                cameraToggling = false;
            }

            foreach (Actor actor in actors)
            {
                actor.Update(gameTime);
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
           
            //Then the landscape shader
            landscapeEffect.Parameters["lightPntPos"].SetValue(playerLamp.lightPntPos);
            Debug.WriteLine("PLAYER LAMP");
            Debug.WriteLine(playerLamp.lightPntPos);
            Debug.WriteLine("OBJ TRACK POS");
            Unit tracking = (Unit)actors[0];
            Debug.WriteLine(tracking.EyeLocation());

            landscapeEffect.Parameters["View"].SetValue(Camera.View);
            landscapeEffect.Parameters["Projection"].SetValue(Camera.Projection);
            landscapeEffect.Parameters["cameraPos"].SetValue(Camera.position);

        }

        private void StepPhysicsModel(GameTime gameTime)
        {
            float timeInSeconds = gameTime.ElapsedGameTime.Milliseconds;
            timeInSeconds /= 1000;
            physicsWorld.step(timeInSeconds);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.DarkRed);
            //First draw the landscape
            landscape.Draw(graphicsDeviceManager.GraphicsDevice, Camera.View, Camera.Projection, landscapeEffect);

            //Then teh actors
            foreach (Actor actor in actors)
            {
                actor.Draw(graphicsDeviceManager.GraphicsDevice, Camera.View, Camera.Projection,null);
            }

            // Show FPS
            fpsRenderer.Draw();

            // Handle base.Draw
            base.Draw(gameTime);
        }
    }
}
