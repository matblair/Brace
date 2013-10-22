using SharpDX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Brace.Physics
{
    class Quadtree
    {
        private int level;
        private Rectangle bounds;
        private List<PhysicsModel> objects;
        private Quadtree[] nodes;
        private readonly int MAX_OBJECTS = 50;  
        public Quadtree(int level,Rectangle bounds)
        {
            this.level = level;
            this.bounds = bounds;
            objects = new List<PhysicsModel>();
            nodes = new Quadtree[4];
        }
        public void Clear()
        {
            objects.Clear();
            for (int i = 0; i < 4; ++i)
            {
                if (nodes[i] != null)
                {
                    nodes[i].Clear();
                    nodes[i] = null;
                }
            }
        }
        private void Split()
        {
            int subWidth = (bounds.Width/2);
            int subHeight = (bounds.Height/2);
            int x = bounds.X;
            int y = bounds.Y;
            nodes[0] = new Quadtree(level + 1, new Rectangle(x + subWidth, y, subWidth, subHeight));
            nodes[1] = new Quadtree(level + 1, new Rectangle(x, y, subWidth, subHeight));
            nodes[2] = new Quadtree(level + 1, new Rectangle(x, y + subHeight, subWidth, subHeight));
            nodes[3] = new Quadtree(level + 1, new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight));
        }
        /*
         * Determine which node the object belongs to. -1 means
         * object cannot completely fit within a child node and is part
         * of the parent node
         */
        private int GetIndex(PhysicsModel pRect)
        {
            int index = -1;
            double verticalMidpoint = bounds.Y;
            double horizontalMidpoint = bounds.X;

            // Object can completely fit within the top quadrants
            bool topQuadrant = (pRect.position.Z - pRect.Height/2 < verticalMidpoint && pRect.position.Z + pRect.Height/2 < verticalMidpoint);
            // Object can completely fit within the bottom quadrants
            bool bottomQuadrant = (pRect.position.Z + pRect.Height / 2 > verticalMidpoint && pRect.position.Z - pRect.Height / 2 > verticalMidpoint);
            // Object can completely fit within the left quadrants
            bool leftQuadrant = (pRect.position.X - pRect.Width / 2 < horizontalMidpoint && pRect.position.X + pRect.Width < horizontalMidpoint);
            // Object can completely fit within the right quadrants
            bool rightQuadrant = (pRect.position.X - pRect.Width / 2 > horizontalMidpoint && pRect.position.X + pRect.Width > horizontalMidpoint);
            if (topQuadrant)
            {
                if (leftQuadrant)
                {
                    index = 0;
                }
                else if (rightQuadrant)
                {
                    index = 1;
                }
            }
            else if (bottomQuadrant)
            {
                if (leftQuadrant)
                {
                    index = 2;
                }
                else if (rightQuadrant)
                {
                    index = 3;
                }
            }

            return index;
        }
        /*
         * Insert the object into the quadtree. If the node
         * exceeds the capacity, it will split and add all
         * objects to their corresponding nodes.
         */
        public void Insert(PhysicsModel pRect)
        {
            if (nodes[0] != null)
            {
                int index = GetIndex(pRect);

                if (index != -1)
                {
                    nodes[index].Insert(pRect);

                    return;
                }
            }

            objects.Add(pRect);

            if (objects.Count > MAX_OBJECTS)
            {
                if (nodes[0] == null)
                {
                    Split();
                }

                int i = 0;
                while (i < objects.Count)
                {
                    int index = GetIndex(objects[i]);
                    if (index != -1)
                    {
                        nodes[index].Insert(objects[i]);
                        objects.Remove(objects[i]);
                    }
                    else
                    {
                        i++;
                    }
                }
            }
        }
        /*
         * Return all objects that could collide with the given object
         */
        public List<PhysicsModel> Retrieve(List<PhysicsModel> returnObjects,PhysicsModel pRect)
        {
            int index = GetIndex(pRect);            
            if (index != -1 && nodes[0] != null)
            {
                nodes[index].Retrieve(returnObjects, pRect);
            }
            foreach(PhysicsModel obj in objects) {
                returnObjects.Add(obj);
            }
            
            return returnObjects;
        }
    }

}
