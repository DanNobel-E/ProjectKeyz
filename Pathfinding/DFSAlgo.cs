using System;
using System.Collections.Generic;
using System.Text;

namespace AIV_Exam_SimoneSantagati
{
    class DFSAlgo
    {
        static public NodePath IterativeLast(Node root)
        {
            NodePath visited = new NodePath();
            Stack<Node> toVisit = new Stack<Node>();
            toVisit.Push(root);
            while (toVisit.Count > 0)
            {
                Node parent = toVisit.Pop();
                if (!visited.Has(parent))
                {
                    visited.Add(parent);
                    foreach (Node eachChild in parent.Edges)
                    {
                        toVisit.Push(eachChild);
                    }
                }
            }
            return visited;
        }

        static public NodePath IterativeFirst(Node root)
        {
            NodePath visited = new NodePath();
            Stack<Node> toVisit = new Stack<Node>();
            toVisit.Push(root);
            while (toVisit.Count > 0)
            {
                Node parent = toVisit.Pop();
                if (!visited.Has(parent))
                {
                    visited.Add(parent);
                    for(int i=parent.Edges.Count-1; i>=0; i--)
                    {
                        toVisit.Push(parent.Edges[i]);
                    }
                }
            }
            return visited;
        }
    }
}
