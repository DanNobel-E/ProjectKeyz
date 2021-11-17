using System;
using System.Collections.Generic;

namespace AIV_Exam_SimoneSantagati
{
    public class Position
    {
        public int row { get; private set; }
        public int col { get; private set; }
        public Position(int x, int y)
        {
            this.row = x;
            this.col = y;
        }

    } 
    public class Node
    {
        public string Label { get; }
        public List<Node> Edges { get; }
        public Dictionary<Node, int> EdgesWeighted { get;  }
        public Position Position { get; private set; }

        public Node(string aLabel, int row=-1, int col=-1)
        {
            Label = aLabel;
            Edges = new List<Node>();
            EdgesWeighted = new Dictionary<Node, int>();
            SetPosition(row, col);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Node)) return false;
            Node other = (Node)obj;
            return Label == other.Label;
        }

        public override string ToString()
        {
            return Label;
        }

        public void SetPosition(int row, int col)
        {
            Position = new Position(row, col);
        }

        public void Link(Node other)
        {
            if (other.Equals(this)) return;
            if (Edges.Contains(other)) return;
            Edges.Add(other);
            other.Edges.Add(this);
        }

        public void Link(Node other, int cost)
        {
            if (other.Equals(this)) return;
            if (EdgesWeighted.ContainsKey(other)) return;
            EdgesWeighted[other] = cost;
            other.EdgesWeighted[this] = cost;
        }

        public void LinkTo(Node other, int cost)
        {
            if (other.Equals(this)) return;
            if (EdgesWeighted.ContainsKey(other)) return;
            EdgesWeighted[other] = cost;
        }
    }
}