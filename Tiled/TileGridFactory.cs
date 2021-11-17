using System;

namespace AIV_Exam_SimoneSantagati
{
     class TileGridFactory
    {

        public static TileGrid Create(int rows, int cols, string csvString, TileSet ts)
        {
            TileGrid result = new TileGrid(rows, cols);

            //Set play scene's width and height by using grid dimensions
            ((PlayScene)Game.CurrentScene).WorldHeight = rows;
            ((PlayScene)Game.CurrentScene).WorldWidth = cols;

            string[] tileIds = csvString.Split(',');

            //Read current scene init position to set map position
            float xPos=((PlayScene)Game.CurrentScene).MapInitPos.X;
            float yPos = ((PlayScene)Game.CurrentScene).MapInitPos.Y;

            for (int row = 0; row < rows; row++)
            {
                for (int col = 0; col < cols; col++)
                {
                    uint id = uint.Parse(tileIds[row * cols + col]);
                    if (id != 0)
                    {
                    TileType type = ts.At(id - 1);
                    TileInstance instance = new TileInstance(type, xPos, yPos);
                    result.Set(row, col, instance);

                    }

                    xPos += Game.TILE_UNIT_SIZE;

                }

                xPos = ((PlayScene)Game.CurrentScene).MapInitPos.X;
                yPos += Game.TILE_UNIT_SIZE; 
            }

            return result;
        }
    }
}