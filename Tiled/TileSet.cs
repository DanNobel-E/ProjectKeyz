using System;

namespace AIV_Exam_SimoneSantagati
{
     class TileSet
    {
        public string ImagePath;
        private int rows;
        private int cols;

        private TileType[] types;

        public TileSet(int rows, int cols, int tileW, int tileH, string tsImgPath)
        {
            this.rows = rows;
            this.cols = cols;
            types = new TileType[rows * cols];
            TileWidth = tileW;
            TileHeight = tileH;
            ImagePath = tsImgPath;
        }

        public int TileWidth { get; internal set; }
        public int TileHeight { get; internal set; }

        public TileType At(uint v)
        {
            return types[v];
        }

        public void Set(int row, int col, TileType t)
        {
            types[row * cols + col] = t;
        }
    }
}