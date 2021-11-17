using System;

namespace AIV_Exam_SimoneSantagati
{
     class TileSetFactory
    {
        public static TileSet Create(int tileW, int tileH, string tImgPath, int tImageW, int tImageH)
        {
            int rows = tImageH / tileH;
            int cols = tImageW / tileW;

            TileSet result = new TileSet(rows, cols, tileW,tileH, tImgPath);

            int offX = 0;
            int offY = 0;
            uint id = 1;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col <cols; col++)
                {

                    TileType t = new TileType(id, tImgPath,tileW, tileH, offX,offY);
                    result.Set(row, col, t);
                    offX += tileW;
                    id++;


                }

                offX = 0;
                offY += tileH;
            }

            return result;
        }
    }
}