using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIV_Exam_SimoneSantagati.Scenes
{
    class RulesScene : TitleScene
    {
        public RulesScene(string texturePath, Controller controller, int sceneID = 0) : base(texturePath, controller, sceneID)
        {
            
        }

        public override Scene OnExit()
        {
            sprite = null;
            texture = null;
            IsPlaying = false;
            return NextScene;
        }
    }
}
