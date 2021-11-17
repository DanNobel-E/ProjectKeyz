using System;
using System.Collections.Generic;
using System.Text;

namespace AIV_Exam_SimoneSantagati
{
    public class NodePath
    {
        public static NodePath ToPath(Dictionary<Node, Node> parents, Node dest)
        {
            NodePath result = new NodePath();
            Node current = dest;
            result.Add(current);
            while (true)
            {
                Node prev = parents[current];
                if (prev.Equals(current)) break;
                result.Add(prev, 0);
                current = prev;
            }
            return result;
        }

        private List<Node> nodes;
        public NodePath()
        {
            nodes = new List<Node>();
        }

        public void Add(Node aNode)
        {
            nodes.Add(aNode);
        }

        public void Add(Node aNode, int pos)
        {
            nodes.Insert(pos, aNode);
        }

        public int Cost()
        {
            int result = 0;
            for(int i=0; i<nodes.Count-1; i++)
            {
                result += nodes[i + 1].EdgesWeighted[nodes[i]];
            }
            return result;
        }

        public string AsString()
        {
            string result = "";
            result += "{" + Cost() + "}";
            result += "[ ";
            for (int i=0; i<nodes.Count; i++)
            {
                result+=nodes[i];
                if (i < nodes.Count-1) result+=" > ";
            }
            result += " ]";
            return result;
        }

        public bool Has(Node other)
        {
            return nodes.Contains(other);
        }

        public int Length()
        {
            return nodes.Count;
        }

        public Node At(int pos)
        {
            return nodes[pos];
        }

        public void Print(string title="")
        {
            Console.WriteLine(title + " " + AsString());
        }
    }
}
