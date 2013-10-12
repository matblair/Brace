using SharpDX;
using SharpDX.Toolkit;
using SharpDX.Toolkit.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace
{
    public class BraceGame : Game
    {
        public GraphicsDeviceManager graphicsDeviceManager;
        public SpriteFont DefaultFont { get; private set; }

        public InputManager input { get; private set; }
        private FPSRenderer fpsRenderer;
        public Camera Camera { get; private set; }
        private bool cameraToggling=false;
        private Actor[] actors;

        public BraceGame()
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
            Camera = new Camera(this, (Unit)actors[1]); // Give this an actor

            base.LoadContent();
        }

        private Actor[] InitializeActors()
        {
            var newActors = new Actor[] {
                new GameLogic.Landscape(this),
                new Unit(Vector3.UnitY*10, Vector3.Zero, Content.Load<Model>("Shaceship"), null)
            };

            return newActors;
        }

        protected override void Update(GameTime gameTime)
        {
            // Handle base.Update
            base.Update(gameTime);

            input.Update();

            // Blerrurhhjgsjkdh. Camera toggling with Tab.
            if (input.KeyboardState.IsKeyDown(SharpDX.Toolkit.Input.Keys.Tab))
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
