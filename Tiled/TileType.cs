using System.Collections.Generic;

namespace AIV_Exam_SimoneSantagati
{
     class TileType
    {
        public TileProperties Props { get; set; }

        public uint Id { get; }
        public string ImagePath { get; }
        public int Width { get; }
        public int Height { get; }
        public int OffX { get; }
        public int OffY { get; }

        public TileType(uint id, string tImgPath, int tileW, int tileH, int offX, int offY)
        {
            this.Id = id;
            this.ImagePath = tImgPath;
            this.Width = tileW;
            this.Height = tileH;
            this.OffX = offX;
            this.OffY = offY;
            Props = new TileProperties();
        }

        public override string ToString()
        {
            return "id: " + Id + "  {" + OffX + ", " + OffY + "} ";
        }
    }
}