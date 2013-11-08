using Brace.GameLogic;
using Brace.Utils;
using SharpDX;
using SharpDX.Direct3D;
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
        public List<TrackingLight> lights;
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

        //Our bounds for the quadtree.
        public Rectangle bounds = Landscape.getBounds();

        //Our features we will be supporting for this graphics levell
        public static int MAX_LIGHTS;
        public static int LANDSCAPE_GEN;
        public static int LANDSCAPE_HEIGHT;
        private string unitshader;
        private string landscapeshader;

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

            //Set up the feature set
            initialiseFeatureLevel();

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

            playerLamp = new TrackingLight(player, Assets.playerColour, Assets.playerOffset, Assets.playerKa, Assets.playerKd, Assets.playerKs, Assets.playerSpecN, Assets.playerOnTop);
            sun = new Light(new Vector3(-1000, 20, 100), Assets.sunColour, Assets.sunKa, Assets.sunKd, Assets.sunKs, Assets.sunSpecN, Assets.sunOnTop);
            lights = new List<TrackingLight>();
            input.Camera = Camera;

        }

        private void LoadAssets()
        {
            Assets.spaceship = Content.Load<Model>("Cube");
            Assets.cube = Content.Load<Model>("cube");
            Assets.tree = Content.Load<Model>("tree");
            Assets.smalltree = Content.Load<Model>("smalltree");
            Assets.player = Content.Load<Model>("player");
            Assets.bullet = Content.Load<Model>("bullet");
            Assets.healthPickup = Content.Load<Model>("health");
            //Load shaders
            unitShader = Content.Load<Effect>(unitshader);
            landscapeEffect = game.Content.Load<Effect>(landscapeshader);

        }

        public void RestartGame()
        {
            StartNewGame();
        }

        private List<Actor> InitializeActors()
        {
            List<Actor> newActors = new List<Actor>();
            Random rand = new Random();
            player = new Player(Vector3.Zero, Vector3.Zero);
            if (OptionsManager.ChallengeModeEnabled())
            {
                player.decreasePerMs = player.decreasePerMs *2;
            }

            newActors.Add(player);
            newActors.Add(new EnemySpawner(player));
            landscape = new GameLogic.Landscape(this);


            for (int t = 0; t < 200; t++)
            {
                float x = rand.Next(-200, 200);
                float y = rand.Next(-200, 200);
                float h = landscape.HeightAt(x, y);
                float rot = rand.Next(0,360);
                int treeSize = rand.Next(0, 2);
                if(treeSize==0){
                     newActors.Add(new Tree(new Vector3(x, h, y), Assets.tree, rot));
                } else {
                     newActors.Add(new Tree(new Vector3(x, h, y), Assets.smalltree, rot));
                }
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
                Utils.HighScoreManager.AddScore(score / 100 * 100);
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

                foreach (TrackingLight light in lights)
                {
                    if (light.isReal)
                    {
                        light.Update(gameTime);
                    }
                }

                // Update the camera 
                Camera.Update(gameTime);

                // Make the extra light array to assign things from 
                PointLight[] extraLights = new PointLight[MAX_LIGHTS];
                int realCount = lights.Count();
                if(realCount>MAX_LIGHTS){
                    realCount = MAX_LIGHTS;
                  }

                for (int i = 0; i < realCount; i++)
                {
                    Light elem = lights.ElementAt(i);
                    extraLights[i] = elem.shadingLight;
                }

                for (int i = realCount; i < MAX_LIGHTS; i++)
                {
                    extraLights[i] = Light.getNullLight();
                }


                // Now update the shaders
                //First the unit shader
                unitShader.Parameters["View"].SetValue(Camera.View);
                unitShader.Parameters["Projection"].SetValue(Camera.Projection);
                unitShader.Parameters["cameraPos"].SetValue(Camera.position);
                unitShader.Parameters["player"].SetValue<PointLight>(playerLamp.shadingLight);
                unitShader.Parameters["sun"].SetValue<PointLight>(sun.shadingLight);

                landscapeEffect.Parameters["View"].SetValue(Camera.View);
                landscapeEffect.Parameters["Projection"].SetValue(Camera.Projection);
                landscapeEffect.Parameters["cameraPos"].SetValue(Camera.position);
                landscapeEffect.Parameters["player"].SetValue<PointLight>(playerLamp.shadingLight);
                landscapeEffect.Parameters["sun"].SetValue<PointLight>(sun.shadingLight);

                //Now put the extra lights in unit shader
                unitShader.Parameters["extra1"].SetValue<PointLight>(extraLights[0]);
            
                //Now put the extra lights in landscape shader
                landscapeEffect.Parameters["extra1"].SetValue<PointLight>(extraLights[0]);

                //For higherlevel feature sets we can use more lights.
                if (GraphicsDevice.Features.Level >= FeatureLevel.Level_9_3)
                {
                    unitShader.Parameters["extra2"].SetValue<PointLight>(extraLights[1]);
                    unitShader.Parameters["extra3"].SetValue<PointLight>(extraLights[2]);
                    unitShader.Parameters["extra4"].SetValue<PointLight>(extraLights[3]);
                    unitShader.Parameters["extra5"].SetValue<PointLight>(extraLights[4]);
                    unitShader.Parameters["extra6"].SetValue<PointLight>(extraLights[5]);
                    unitShader.Parameters["extra7"].SetValue<PointLight>(extraLights[6]);

                    landscapeEffect.Parameters["extra2"].SetValue<PointLight>(extraLights[1]);
                    landscapeEffect.Parameters["extra3"].SetValue<PointLight>(extraLights[2]);
                    landscapeEffect.Parameters["extra4"].SetValue<PointLight>(extraLights[3]);
                    landscapeEffect.Parameters["extra5"].SetValue<PointLight>(extraLights[4]);
                    landscapeEffect.Parameters["extra6"].SetValue<PointLight>(extraLights[5]);
                    landscapeEffect.Parameters["extra7"].SetValue<PointLight>(extraLights[6]);
                }

                //For even higherlevel feature sets we can use even more lights.
                if (GraphicsDevice.Features.Level >= FeatureLevel.Level_10_0)
                {
                    unitShader.Parameters["extra8"].SetValue<PointLight>(extraLights[7]);
                    unitShader.Parameters["extra9"].SetValue<PointLight>(extraLights[8]);
                    unitShader.Parameters["extra10"].SetValue<PointLight>(extraLights[9]);
                    unitShader.Parameters["extra11"].SetValue<PointLight>(extraLights[10]);
                    unitShader.Parameters["extra12"].SetValue<PointLight>(extraLights[11]);
                    landscapeEffect.Parameters["extra8"].SetValue<PointLight>(extraLights[7]);
                    landscapeEffect.Parameters["extra9"].SetValue<PointLight>(extraLights[8]);
                    landscapeEffect.Parameters["extra10"].SetValue<PointLight>(extraLights[9]);
                    landscapeEffect.Parameters["extra11"].SetValue<PointLight>(extraLights[10]);
                    landscapeEffect.Parameters["extra12"].SetValue<PointLight>(extraLights[11]);
                } 

               
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
                actor.Draw(graphicsDeviceManager.GraphicsDevice, Camera.View, Camera.Projection, unitShader);
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

        internal void TrackProjectile(ITrackable proj, Vector4 colour)
        {
            this.AddLight(new TrackingLight(proj, colour, Assets.missileOffset, Assets.missileKa, Assets.missileKd, Assets.missileKs, Assets.missileSpecN, Assets.missileOnTop));
        }

        internal void StopTrackingProjectile(ITrackable proj)
        {
            foreach (TrackingLight light in lights)
            {
                if(light!=null && proj!=null){
                    if (proj.Equals(light.tracking))
                    {
                        lights.Remove(light);
                         return;
                    }
                }
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

        public void AddLight(TrackingLight light)
        {
            if (lights.Count > (MAX_LIGHTS-1))
            {
                lights.RemoveAt(0);
                lights.Add(light);
            }
            else
            {
                lights.Add(light);
            }
        }

        private void initialiseFeatureLevel()
        {
            //Get the feature level.
            FeatureLevel graphicsSupport = graphicsDeviceManager.GraphicsDevice.Features.Level;
            if (graphicsSupport >= FeatureLevel.Level_11_0)
            {
                LANDSCAPE_GEN = 9;
                MAX_LIGHTS = 12;
                LANDSCAPE_HEIGHT = 10;
                unitshader = "MPUnitCelShader_11";
                landscapeshader = "MPLandscapeCelShader_11";

                //Set up the player information
                Assets.playerColour = new Vector4(1.0f, 0.8f, 0.1f, 1.0f);
                Assets.playerOffset = new Vector3(0, 2.5f, 4.5f);
                Assets.playerKa = 0.03f;
                Assets.playerKd = 1f;
                Assets.playerKs = 1f;
                Assets.playerSpecN = 1f;
                Assets.playerOnTop = 6f;

                //Set up the sun information 
                Assets.sunColour = new Vector4(0.55f, 0.1f, 0.9f, 1);
                Assets.sunKa = 0.05f;
                Assets.sunKd = 0.5f;
                Assets.sunKs = 0.90f;
                Assets.sunSpecN = 10.0f;
                Assets.sunOnTop = 500f;

                //Set up the missile information
                Assets.missileColour = new Vector4(0.5f, 0.5f, 1, 1);
                Assets.missileOffset = new Vector3(0, 0, 0);
                Assets.missileKa = 0.05f;
                Assets.missileKd = 1f;
                Assets.missileKs = 2f;
                Assets.missileSpecN = 2f;
                Assets.missileOnTop = 2f;

                //Set up the health information
                Assets.healthColour = new Vector4(0.4f, 1.0f, 0.8f, 1);
                Assets.healthOffSet = new Vector3(0, 0, 0);
                Assets.healthKa = 0.0f;
                Assets.healthKd = 0f;
                Assets.healthKs = 2f;
                Assets.healthSpecN = 2f;
                Assets.healthOnTop = 2f;
            }
            else if (graphicsSupport >= FeatureLevel.Level_10_0)
            {
                LANDSCAPE_GEN = 9;
                MAX_LIGHTS = 12;
                LANDSCAPE_HEIGHT = 10;
                unitshader = "MPUnitCelShader_10";
                landscapeshader = "MPLandscapeCelShader_10";

                //Set up the player information
                Assets.playerColour = new Vector4(1.0f, 0.8f, 0.1f, 1.0f);
                Assets.playerOffset = new Vector3(0, 2.5f, 4.5f);
                Assets.playerKa = 0.03f;
                Assets.playerKd = 1f;
                Assets.playerKs = 1f;
                Assets.playerSpecN = 1f;
                Assets.playerOnTop = 6f;

                //Set up the sun information 
                Assets.sunColour = new Vector4(0.55f, 0.1f, 0.9f, 1);
                Assets.sunKa = 0.05f;
                Assets.sunKd = 0.5f;
                Assets.sunKs = 0.90f;
                Assets.sunSpecN = 10.0f;
                Assets.sunOnTop = 500f;

                //Set up the missile information
                Assets.missileColour = new Vector4(0.5f, 0.5f, 1, 1);
                Assets.missileOffset = new Vector3(0, 0, 0);
                Assets.missileKa = 0.05f;
                Assets.missileKd = 1f;
                Assets.missileKs = 2f;
                Assets.missileSpecN = 2f;
                Assets.missileOnTop = 2f;

                //Set up the health information
                Assets.healthColour = new Vector4(0.4f, 1.0f, 0.8f, 1);
                Assets.healthOffSet = new Vector3(0, 0, 0);
                Assets.healthKa = 0.0f;
                Assets.healthKd = 0f;
                Assets.healthKs = 2f;
                Assets.healthSpecN = 2f;
                Assets.healthOnTop = 2f;
            }
            else if (graphicsSupport >= FeatureLevel.Level_9_3)
            {
                LANDSCAPE_GEN = 8;
                MAX_LIGHTS = 7;
                LANDSCAPE_HEIGHT = 10;
                unitshader = "MPUnitCelShader_10";
                landscapeshader = "MPLandscapeCelShader_9_3";

                //Set up the player information
                Assets.playerColour = new Vector4(1.0f, 0.8f, 0.1f, 1.0f);
                Assets.playerOffset = new Vector3(0, 2.5f, 4.5f);
                Assets.playerKa = 0.03f;
                Assets.playerKd = 1f;
                Assets.playerKs = 1f;
                Assets.playerSpecN = 1f;
                Assets.playerOnTop = 6f;

                //Set up the sun information 
                Assets.sunColour = new Vector4(0.55f, 0.1f, 0.9f, 1);
                Assets.sunKa = 0.05f;
                Assets.sunKd = 0.5f;
                Assets.sunKs = 0.90f;
                Assets.sunSpecN = 10.0f;
                Assets.sunOnTop = 500f;

                //Set up the missile information
                Assets.missileColour = new Vector4(0.5f, 0.5f, 1, 1);
                Assets.missileOffset = new Vector3(0, 0, 0);
                Assets.missileKa = 0.05f;
                Assets.missileKd = 1f;
                Assets.missileKs = 2f;
                Assets.missileSpecN = 2f;
                Assets.missileOnTop = 2f;

                //Set up the health information
                Assets.healthColour = new Vector4(0.4f, 1.0f, 0.8f, 1);
                Assets.healthOffSet = new Vector3(0, 0, 0);
                Assets.healthKa = 0.0f;
                Assets.healthKd = 0f;
                Assets.healthKs = 2f;
                Assets.healthSpecN = 2f;
                Assets.healthOnTop = 2f;
            }
            else
            {
                //  Load minimal support level
                LANDSCAPE_GEN = 6;
                MAX_LIGHTS = 2;
                LANDSCAPE_HEIGHT = 4;
                unitshader = "MPUnitCelShader_9_1";
                landscapeshader = "MPLandscapeCelShader_9_1";

                //Set up the player information
                Assets.playerColour = new Vector4(1.0f, 0.8f, 0.1f, 1.0f);
                Assets.playerOffset = new Vector3(0, 2.5f, 4.5f);
                Assets.playerKa = 0.5f;
                Assets.playerKd = 1.5f;
                Assets.playerKs = 2f;
                Assets.playerSpecN = 1f;
                Assets.playerOnTop = 6f;

                //Set up the sun information 
                Assets.sunColour = new Vector4(0.55f, 0.1f, 0.9f, 1);
                Assets.sunKa = 0.5f;
                Assets.sunKd = 0.5f;
                Assets.sunKs = 3.0f;
                Assets.sunSpecN = 10.0f;
                Assets.sunOnTop = 500f;

                //Set up the missile information
                Assets.missileColour = new Vector4(0.5f, 0.5f, 1, 1);
                Assets.missileOffset = new Vector3(0, 0, 0);
                Assets.missileKa = 0.4f;
                Assets.missileKd = 1.5f;
                Assets.missileKs = 5f;
                Assets.missileSpecN = 2f;
                Assets.missileOnTop = 2f;

                //Set up the health information
                Assets.healthColour = new Vector4(0.4f, 1.0f, 0.8f, 1);
                Assets.healthOffSet = new Vector3(0, 0, 0);
                Assets.healthKa = 0.5f;
                Assets.healthKd = 3f;
                Assets.healthKs = 2f;
                Assets.healthSpecN = 3f;
                Assets.healthOnTop = 2f;
            }
        }

    }
}
