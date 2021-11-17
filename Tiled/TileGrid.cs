using System;

namespace AIV_Exam_SimoneSantagati
{
     class TileGrid
    {
        private int rows;
        private int cols;
        private TileInstance[] tiles;

        public TileGrid(int rows, int cols)
        {
            this.rows = rows;
            this.cols = cols;
            tiles = new TileInstance[rows * cols];
        }

        public TileInstance At(uint v)
        {
            return tiles[v];
        }

        public void Set(int row, int col, TileInstance instance)
        {
            tiles[row * cols + col] = instance;
        }

        public int Size()
        {
            return tiles.Length;
        }
    }
}