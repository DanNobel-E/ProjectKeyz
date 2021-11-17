using Aiv.Fast2D;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIV_Exam_SimoneSantagati
{
    enum SheetDirection { Up, Side, Down, Last}
    abstract class Actor : GameObject
    {
        protected float speed;
        public float Speed { get { return speed; } }
        

        public Vector2 Velocity
        {
            get
            {//read
                return RigidBody.Velocity;
            }
            set
            {//write
                RigidBody.Velocity = value;
            }
        }


        public Actor(string textureName, DrawLayer layer=DrawLayer.Playground, float width=0, float height=0) : base(textureName, layer, width, height)
        {

            sprite.pivot = new Vector2(sprite.Width * 0.5f, sprite.Height * 0.5f);
            RigidBody = new RigidBody(this);
            RigidBody.Collider = ColliderFactory.CreateBoxFor(this);

            DrawMgr.AddItem(this);


        }


        public virtual void SetXVelocity(float x)
        {
            RigidBody.Velocity = new Vector2(x, RigidBody.Velocity.Y);
        }

        public virtual void SetYVelocity(float y)
        {
            RigidBody.Velocity =new Vector2( RigidBody.Velocity.X, y);
        }

       


    }
}
