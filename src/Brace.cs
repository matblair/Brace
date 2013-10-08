using Project1;
using Project1.src.Physics;
using SharpDX;
using SharpDX.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Project1.src
{
    public class Brace : Game
    {
        public static Brace get() {
            if(brace == null) {
                brace = new Brace();
            }
            return brace;
        }

        private static Brace brace;
        public GraphicsDeviceManager graphicsDeviceManager;
        
        public InputManager input;
        public Camera view;
        private GameObject[] actors;

        

        public Brace()
        {
            input = new InputManager();
            
            view = new Camera(0,0,0);
            actors = InitializeActors();

            graphicsDeviceManager = new GraphicsDeviceManager(this);
        }

        private GameObject[] InitializeActors()
        {
            throw new NotImplementedException();
        }


        protected override void Update(GameTime gameTime)
        {
            for (int i = 0; i < actors.Length; ++i)
            {
                actors[i].Update(gameTime);
            }

            stepPhysicsModel(gameTime);

            // Handle base.Update
            base.Update(gameTime);
        }

        private void stepPhysicsModel(GameTime gameTime)
        {
            Resolver.step(gameTime, actors);
        }

        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.Black);
            foreach (GameObject actor in actors)
            {
                actor.Draw(graphicsDeviceManager.GraphicsDevice, Camera;
            }

            // Handle base.Update
            base.Update(gameTime);
        }



    }
}
