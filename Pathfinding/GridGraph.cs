using System;
using System.Collections.Generic;
using System.Text;

namespace AIV_Exam_SimoneSantagati
{
    public class GridGraph
    {
        private int[,] grid;
        private Node[] nodes;

        public GridGraph(int[,] grid)
        {
            this.grid = grid;
            nodes = new Node[grid.GetLength(0) * grid.GetLength(1)];


            int rows = grid.GetLength(0);
            int cols = grid.GetLength(1);

            for (int r = 0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    string label = r + "x" + c;
                    Node n = new Node(label, r, c);
                    SetNode(n);
                }
            }


            for (int r=0; r < rows; r++)
            {
                for (int c = 0; c < cols; c++)
                {
                    Node curr = NodeAt(r, c);

                    //UP
                    if (r - 1 >= 0)
                    {
                        Node n = NodeAt(r - 1, c);
                        curr.LinkTo(n, grid[r - 1, c]);
                    }
                    //DOWN
                    if (r + 1 < rows)
                    {
                        Node n = NodeAt(r + 1, c);
                        curr.LinkTo(n, grid[r + 1, c]);
                    }

                    //LEFT
                    if (c - 1 >= 0)
                    {
                        Node n = NodeAt(r, c-1);
                        curr.LinkTo(n, grid[r, c-1]);
                    }

                    //RIGHT
                    if (c + 1 < cols)
                    {
                        Node n = NodeAt(r, c + 1);
                        curr.LinkTo(n, grid[r, c + 1]);
                    }
                }
            }
        }

        public Node NodeAt(int row, int col)
        {
            Node result = null;

            int pos = row * grid.GetLength(1) + col;

            if (pos>=0 && pos<nodes.Length)
            {
                result= nodes[pos];

            }

            return result;
        }

        public Node SetNode(Node node)
        {
            int pos = node.Position.row * grid.GetLength(1) + node.Position.col;
            return nodes[pos] = node;
        }
    }
}
