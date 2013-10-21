﻿using Brace.Physics;
using Brace.Utils;
using SharpDX;
using SharpDX.Toolkit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace.GameLogic
{
    class Player : Unit
    {
        private int chargeTime;
        
        private readonly int MINIMUMCHARGETIME = 300;
        private readonly int MAXIMUMCHARGETIME = 1600;
        public readonly int MAXARROWDAMAGE = 60;
        public readonly int MINARROWDAMAGE = 40;

        private readonly int MAXSPEED = 1;

        private int health;
        private readonly int MAXHEALTH = 100;

        PlayerController controller;

        public Player(Vector3 position, Vector3 rotation)
            : base(position, rotation, Assets.player, null)
        {
            controller = new PlayerController(this);
            chargeTime = 0;
            health = MAXHEALTH;
        }

        protected override void InitializePhysicsObject()
        {
            pObject = new PhysicsModel();
            SpheresBody bodyDef = new SpheresBody(pObject, false);
            pObject.Initialize(30, 0.7f, 0.1f, 0.7f, position, Vector3.Zero, bodyDef);
            pObject.bodyDefinition.bodyType = BodyType.dynamic;
            BraceGame.get().physicsWorld.AddBody(pObject);
        }

        public override void Update(GameTime gameTime)
        {
            position = pObject.position;
            controller.Update(gameTime);
        }

        public void ShootArrow(Vector2 direction)
        {
            direction.Normalize();

            if (chargeTime > MAXIMUMCHARGETIME)
            {
                Vector3 arrowIPosition = position + new Vector3(direction.X, 0, direction.Y) * 3;
                Projectile proj = new Projectile(arrowIPosition, new Vector3(direction.X, 0, direction.Y), MAXARROWDAMAGE);
                BraceGame.get().AddActor(proj);
                BraceGame.get().TrackProjectile(proj);

            }
            else if (chargeTime > MINIMUMCHARGETIME)
            {
                int actualDamage = (MAXARROWDAMAGE - MINARROWDAMAGE) * (chargeTime - MINIMUMCHARGETIME) / (MAXIMUMCHARGETIME - MINIMUMCHARGETIME) + MINARROWDAMAGE;
                Vector3 arrowIPosition = position + new Vector3(direction.X, 0, direction.Y)*3;
                Projectile proj = new Projectile(arrowIPosition, new Vector3(direction.X, 0, direction.Y), actualDamage);
                BraceGame.get().AddActor(proj);
                BraceGame.get().TrackProjectile(proj);
            }

            chargeTime = 0;
        }

        public void ChargeArrow(GameTime gameTime)
        {
            chargeTime += gameTime.ElapsedGameTime.Milliseconds;
        }

        public override void Move(Vector2 destination)
        {
            base.Move(destination);

            Vector2 direction = destination - new Vector2(position.X,position.Z);
            direction.Normalize();
            Vector2 targetSpeed = new Vector2(direction.X * MAXSPEED, direction.Y * MAXSPEED);
            Vector3 force = new Vector3(targetSpeed.X, 0, targetSpeed.Y);
            pObject.ApplyImpulse(force);
        }

        internal void lowerHealth(int damage)
        {
            health -= damage;
            if (health < 0)
            {
                die();
            }
        }

        private void die()
        {
            throw new NotImplementedException();
        }


    }
}
