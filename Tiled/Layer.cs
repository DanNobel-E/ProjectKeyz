namespace AIV_Exam_SimoneSantagati
{
     class Layer
    {
        public TileGrid Grid;
        public string Name;
        public TileProperties Props { get; internal set; }

        public Layer(string name, TileGrid tg)
        {
           Name = name;
           Grid = tg;
           Props = new TileProperties();
        }

    }
}