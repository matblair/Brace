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
        private int MAX_LIGHTS = 8;
        public GraphicsDeviceManager graphicsDeviceManager;
        public SpriteFont DefaultFont { get; private set; }

        public InputManager input { get; private set; }
        private FPSRenderer fpsRenderer;
        public Camera Camera { get; private set; }
        public List<Actor> actors;
        public Queue<Light> lights;
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
        private Effect drawShader;
        private TrackingLight playerLamp;
        private Light sun;

        //Comment this???
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

            playerLamp = new TrackingLight((Unit)actors[0], new Vector4(1.0f, 0.8f, 0.1f, 1.0f), new Vector3(0, 2.5f, 4.5f), 0.03f, 1f, 1f, 1f, 6f);
            sun = new Light(new Vector3(-1000, 20, 100), new Vector4(0.55f, 0.1f, 0.9f, 1), 0.05f, 0.5f, 0.90f, 10.0f, 500f);
            projectileLamp = new TrackingLight(null, new Vector4(0.5f, 0.5f, 1, 1), new Vector3(0, 0, 0), 0.05f, 1f, 2f, 2f, 2f);
            projectileLamp.TurnOff();

            lights = new Queue<Light>();
            lights.Enqueue(projectileLamp);

            input.Camera = Camera;

        }

        private void LoadAssets()
        {
            Assets.spaceship = Content.Load<Model>("Cube");
            Assets.cube = Content.Load<Model>("cube");
            Assets.tree = Content.Load<Model>("tree");
            Assets.player = Content.Load<Model>("player");
            Assets.bullet = Content.Load<Model>("bullet");
            Assets.healthPickup = Content.Load<Model>("health");
            //Load shaders
            unitShader = Content.Load<Effect>("MPUnitCelShader");
            landscapeEffect = game.Content.Load<Effect>("MPLandscapeCelShader");
            drawShader = game.Content.Load<Effect>("DrawShader");

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
                playerLamp.intensityVector = player.getIntensityVector();
                playerLamp.Update(gameTime);
                projectileLamp.Update(gameTime);

                foreach (Light light in lights)
                {
                    light.Update(gameTime);
                }

                // Update the camera 
                Camera.Update(gameTime);

                // Make the extra light array to assign things from 
                PointLight[] extraLights = new PointLight[8];
                for (int i = 0; i < MAX_LIGHTS; i++)
                {
                    Light elem = lights.ElementAt(i);
                    if (elem != null)
                    {
                        extraLights[i] = elem.shadingLight;
                    }
                    else
                    {
                        extraLights[i] = Light.getNullLight();
                    }
                }


                // Now update the shaders
                //First the unit shader
                unitShader.Parameters["View"].SetValue(Camera.View);
                unitShader.Parameters["Projection"].SetValue(Camera.Projection);
                unitShader.Parameters["cameraPos"].SetValue(Camera.position);
                unitShader.Parameters["player"].SetValue(playerLamp.shadingLight);
                unitShader.Parameters["sun"].SetValue(sun.shadingLight);
              
                landscapeEffect.Parameters["View"].SetValue(Camera.View);
                landscapeEffect.Parameters["Projection"].SetValue(Camera.Projection);
                landscapeEffect.Parameters["cameraPos"].SetValue(Camera.position);
                landscapeEffect.Parameters["player"].SetValue(playerLamp.shadingLight);
                landscapeEffect.Parameters["sun"].SetValue(sun.shadingLight);

                //Now put the extra lights in unit shader
                unitShader.Parameters["extra1"].SetValue(extraLights[0]);
                unitShader.Parameters["extra2"].SetValue(extraLights[1]);
                unitShader.Parameters["extra3"].SetValue(extraLights[2]);
                unitShader.Parameters["extra4"].SetValue(extraLights[3]);
                unitShader.Parameters["extra5"].SetValue(extraLights[4]);
                unitShader.Parameters["extra6"].SetValue(extraLights[5]);
                unitShader.Parameters["extra7"].SetValue(extraLights[6]);
                unitShader.Parameters["extra8"].SetValue(extraLights[7]);

                //Now put the extra lights in landscape shader
                landscapeEffect.Parameters["extra1"].SetValue(extraLights[0]);
                landscapeEffect.Parameters["extra2"].SetValue(extraLights[1]);
                landscapeEffect.Parameters["extra3"].SetValue(extraLights[2]);
                landscapeEffect.Parameters["extra4"].SetValue(extraLights[3]);
                landscapeEffect.Parameters["extra5"].SetValue(extraLights[4]);
                landscapeEffect.Parameters["extra6"].SetValue(extraLights[5]);
                landscapeEffect.Parameters["extra7"].SetValue(extraLights[6]);
                landscapeEffect.Parameters["extra8"].SetValue(extraLights[7]);
            
            

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

        public void AddLight(Light light){
            if(lights.Count>7){
                lights.Dequeue();
                lights.Enqueue(light);
            } else {
                lights.Enqueue(light);
            }
        }
       
    }
}
