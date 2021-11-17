using Aiv.Fast2D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Aiv.Audio;

namespace AIV_Exam_SimoneSantagati
{
    class AnimatedTitleScene : TitleScene
    {
        protected int numFrames;
        protected float frameDuration;
        protected int frameWidth;
        protected int frameHeight;
        protected int textureOffsetX;
        protected int textureOffsetY;

        protected int currentFrame;

        protected float elapsedTime;

        

        public AnimatedTitleScene(string textureName, Controller controller) : base(textureName, controller)
        {
            //Initi animation Settings
            numFrames = 5;
            frameDuration = 1;
            frameWidth = 1280;
            frameHeight = 720;
            textureOffsetX = 0;
            textureOffsetY = 0;
            elapsedTime = 0;

        }
        public override void Start()
        {
            base.Start();
            sprite.Camera = CameraMgr.MainCamera;
            CameraMgr.MainCamera.position=sprite.position;

            //Change background clip to gameover clip
            Game.BackgroundClip = new AudioClip("Assets/Sounds/gameOverMusic.wav");
            Game.BackgroundSource.Play(Game.BackgroundClip);
        }


        public override void Input()
        {
            base.Input();

            if (IsPlaying && controller.Quit())
            {
                NextScene = null;
                IsPlaying = false;
            }

        }
        public override void Update()
        {
            base.Update();

            //animate sprite texture
            elapsedTime += Game.DeltaTime;

            if (elapsedTime >= frameDuration)
            {
                currentFrame++;
                elapsedTime = 0;

                //Loop animation
                if (currentFrame >= numFrames)
                {
                    
                     currentFrame = 0;
                    
                }

                textureOffsetX = frameWidth * currentFrame;

            }
        }
        public override void Draw()
        {
            sprite.DrawTexture(texture, textureOffsetX, textureOffsetY, frameWidth, frameHeight);
        }

    }

}
