namespace AIV_Exam_SimoneSantagati
{
     class TileInstance
    {
        public TileType Type { get; }
        public float PosX { get; }

        public float PosY { get; }

        public TileInstance(TileType type, float xPos, float yPos)
        {
            this.Type = type;
            this.PosX = xPos;
            this.PosY = yPos;
        }

        public override string ToString()
        {
            return Type + " {" + PosX + ", " + PosY + "} ";
        }
    }
}