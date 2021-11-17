using Aiv.Fast2D;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIV_Exam_SimoneSantagati
{
    class TitleScene :Scene
    {
        protected Sprite sprite;
        protected Texture texture;
        protected Controller controller;
        protected string texturePath;
        protected int sceneID;
        protected static bool isSkipPressed;

        public TitleScene(string texturePath, Controller controller, int sceneID=0)
        {

            this.texturePath = texturePath;
            this.controller = controller;
            this.sceneID = sceneID; 



        }

        public override void Start()
        {
            texture = new Texture(texturePath);
            sprite = new Sprite(Game.Win.OrthoWidth, Game.Win.OrthoHeight);
            sprite.pivot= new Vector2(sprite.Width * 0.5f, sprite.Height * 0.5f);
            base.Start();
        }
        public override void Input()
        {
            if (controller.Skip())
            {
                if (isSkipPressed == false)
                {
                    IsPlaying = false;
                    isSkipPressed = true;
                }
            }
            else if(isSkipPressed==true)
            {
                isSkipPressed = false;
            }
        }
        public override void Draw()
        {
            sprite.DrawTexture(texture);
        }

        public override Scene OnExit()
        {
            //Plays Background when game begins
            Game.BackgroundSource.Play(Game.BackgroundClip, true);

            sprite = null;
            texture = null;
            return base.OnExit();
        }
    }
}
