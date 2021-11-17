using AIV_Exam_SimoneSantagati;
using OpenTK;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIV_Exam_SimoneSantagati
{
    class World
    {

        private int[,] grid;
        private int rows;
        private int cols;
        private Dictionary<TileObj, Tuple<int, int>> tileObjToGrid;

        private GridGraph graph;


        public void Init(int r, int c)
        {
            rows = r;
            cols = c;

            //Links a node to its respective TileObj, by grid coords stored in the Tuple
            tileObjToGrid = new Dictionary<TileObj, Tuple<int, int>>();

            //Nodes grid
            grid = new int[rows, cols];

            //Sort each Node. The position of each node is at the center of the matched tile object
            float posY = Game.TILE_UNIT_SIZE*0.5f;

            for (int row = 0; row < rows; row++)
            {
                float posX = Game.TILE_UNIT_SIZE*0.5f;

                for (int col = 0; col < cols; col++)
                {

                    foreach (TileObj each in ((PlayScene)Game.CurrentScene).TileObjs)
                    {
                        if(each.Position.X>=posX-0.1f && each.Position.X <= posX + 0.1f
                            && each.Position.Y >= posY-0.1f && each.Position.Y<=posY+0.1f)
                        {
                            //sets node's weight
                            grid[row, col] = each.Weight;

                            //store node's coord in the dictionary
                            tileObjToGrid[each] = new Tuple<int, int>(row, col);
                            break;
                        }
                    }

                    posX += Game.TILE_UNIT_SIZE;
                    
                }

                posY += Game.TILE_UNIT_SIZE;
            }

            //Instanciate graph
            graph = new GridGraph(grid);

        }

        public Vector2 GetNodePosition(Node n)
        {
            return GetTileObj(n).Position;
        }

        public Node GetNodeAtPosition(Vector2 position)
        {
            //Find tile at given position
            TileObj t = GetTileAtPosition(position);

            if (t != null)
            {
                //get tile's node coords 
                Tuple<int, int> coords = tileObjToGrid[t];

                //get node at coords
                return graph.NodeAt(coords.Item1, coords.Item2);

            }

            return null;
        }


        public TileObj GetTileObj(int row, int col)
        {
            //Ask the dictionary the tile object at given coord
            foreach (var each in tileObjToGrid)
            {
                if (each.Value.Item1 == row && each.Value.Item2 == col)
                {
                    return each.Key;
                }
            }

            return null;

        }


        public TileObj GetTileObj(Node n)
        {
            return GetTileObj(n.Position.row, n.Position.col);

        }

        public TileObj GetTileAtPosition(Vector2 position)
        {
            //Find the tile object at given position
            foreach (TileObj each in tileObjToGrid.Keys)
            {
                if (each.Contains(position))
                {
                    return each;
                }
            }

            return null;
        }


        public List<TileObj> GetTilesAround(TileObj tile)
        {
            //Get a list which contains all tile object around the given tile object
            List<TileObj> result = new List<TileObj>();

            Node central= GetNodeAtPosition(tile.Position);

            Node upLeft = graph.NodeAt(central.Position.row - 1, central.Position.col - 1);
            if (upLeft != null)
            {
            result.Add(GetTileObj(upLeft));

            }

            Node up = graph.NodeAt(central.Position.row - 1, central.Position.col);
            if (up != null)
            {
                result.Add(GetTileObj(up));

            }

            Node upRight = graph.NodeAt(central.Position.row - 1, central.Position.col + 1);
            if (upRight != null)
            {
                result.Add(GetTileObj(upRight));

            }

            Node left = graph.NodeAt(central.Position.row, central.Position.col - 1);
            if (left != null)
            {
                result.Add(GetTileObj(left));

            }

            Node right = graph.NodeAt(central.Position.row, central.Position.col + 1);
            if (right != null)
            {
                result.Add(GetTileObj(right));

            }

            Node downLeft = graph.NodeAt(central.Position.row + 1, central.Position.col - 1);
            if (downLeft != null)
            {
                result.Add(GetTileObj(downLeft));

            }

            Node down = graph.NodeAt(central.Position.row + 1, central.Position.col);
            if (down != null)
            {
                result.Add(GetTileObj(down));

            }

            Node downRight = graph.NodeAt(central.Position.row + 1, central.Position.col + 1);
            if (downRight != null)
            {
                result.Add(GetTileObj(downRight));

            }

            return result;

        }
    }
}
