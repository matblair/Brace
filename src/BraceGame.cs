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
        public List<Actor> actors;
        private static BraceGame game;
        private static TrackingLight projectileLamp;

        private Player player;
        private int score;

        public Physics.PhysicsEngine physicsWorld;

        //Our actors
        private GameLogic.Landscape landscape;

        //Rendering stuff
        private Effect unitShader;
        private Effect landscapeEffect;
        private TrackingLight playerLamp;

        //
        public Rectangle bounds = Landscape.getBounds();

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
 
            // Load camera and models
            LoadAssets();

            StartNewGame();
            base.LoadContent();
        }
        private void StartNewGame()
        {
            score = 0;

            // Create PhysicsWorld
            physicsWorld = new PhysicsEngine(); ;

            //InitializeActors
            actors = InitializeActors();
            
            //Initialize Camera
            Camera = new Camera(this, player); // Give this an actor
            Camera.SetViewType(Brace.Camera.ViewType.FirstPerson);
            playerLamp = new TrackingLight(player, new Vector3(0, 2.5f, 4.5f));
            projectileLamp = new TrackingLight(null, new Vector4(0.5f, 0.5f, 1, 1), new Vector3(0, 0, 0));
            input.Camera = Camera;

        }

        private void LoadAssets()
        {
            Assets.spaceship = Content.Load<Model>("Cube");
            Assets.cube = Content.Load<Model>("cube");
            Assets.tree = Content.Load<Model>("tree");
            Assets.player = Content.Load<Model>("player");
            Assets.bullet = Content.Load<Model>("bullet");
            //Load shaders
            unitShader = Content.Load<Effect>("CubeCelShader");
            landscapeEffect = game.Content.Load<Effect>("LandscapeCelShader");

        }

        public void RestartGame(){
            StartNewGame();
        }

        private List<Actor> InitializeActors()
        {
            List<Actor> newActors = new List<Actor>();         
            Random rand = new Random();
            player = new Player(Vector3.Zero, Vector3.Zero);
            newActors.Add(player);
            newActors.Add(new EnemySpawner(player));
            landscape = new GameLogic.Landscape(this);


            for (int t = 0; t < 200; t++)
            {
                float x = rand.Next(-200, 200);
                float y = rand.Next(-200, 200);
                float h = landscape.HeightAt(x, y);
                newActors.Add(new Tree(new Vector3(x, h, y)));
            }


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

            if (player.isDead)
            {
                Utils.HighScoreManager.AddScore(score/100*100);
                MainPage.GetMainPage().ResetGame();
                RestartGame();
            }

            if (!paused)
            {
                score += gameTime.ElapsedGameTime.Milliseconds;

                for (int i = 0; i < actors.Count(); ++i)
                {
                    if (actors[i].doomed)
                    {
                        actors.Remove(actors[i]);
                        --i;
                        continue;
                    }
                    else
                    {
                        actors[i].Update(gameTime);
                    }

                }

                StepPhysicsModel(gameTime);


                //Update tracking lights
                playerLamp.Update(gameTime);
                projectileLamp.Update(gameTime);

                // Update the camera 
                Camera.Update(gameTime);


                // Now update the shaders
                //First the unit shader
                unitShader.Parameters["View"].SetValue(Camera.View);
                unitShader.Parameters["Projection"].SetValue(Camera.Projection);
                unitShader.Parameters["missilePntPos"].SetValue(projectileLamp.lightPntPos);
                unitShader.Parameters["missilePntCol"].SetValue(projectileLamp.GetColour());
                unitShader.Parameters["cameraPos"].SetValue(Camera.position);
                unitShader.Parameters["lightPntPos"].SetValue(playerLamp.lightPntPos);
                unitShader.Parameters["lightPntCol"].SetValue(playerLamp.lightPntCol * player.getIntensityVector());
                //Then the landscape shader
                landscapeEffect.Parameters["lightPntPos"].SetValue(playerLamp.lightPntPos);
                landscapeEffect.Parameters["lightPntCol"].SetValue(playerLamp.lightPntCol * player.getIntensityVector());
                landscapeEffect.Parameters["missilePntPos"].SetValue(projectileLamp.lightPntPos);
                landscapeEffect.Parameters["missilePntCol"].SetValue(projectileLamp.GetColour());
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
            //fpsRenderer.Draw();

            // Handle base.Draw
            base.Draw(gameTime);
        }

        internal void AddActor(Actor actor)
        {
            actors.Add(actor);
        }

        internal void TrackProjectile(Projectile proj)
        {
            projectileLamp.SnapToTarget(proj);
            projectileLamp.TurnOn();
        }

        internal void StopTrackingProjectile(Projectile proj)
        {
            if (projectileLamp.IsTracking(proj))
            {
                projectileLamp.TurnOff();
                projectileLamp.SetTarget(null);
            }
        }

        internal void RemoveActor(Actor actor)
        {
            actors.Remove(actor);
        }
        internal Player getPlayer()
        {
            return player;
        }

       
    }
}
