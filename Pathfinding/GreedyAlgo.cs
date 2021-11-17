using System;
using System.Collections.Generic;

namespace AIV_Exam_SimoneSantagati
{
    public class GreedyAlgo
    {
        public static NodePath AStar_ShortestPath(Node root, Node dest)
        {
            Dictionary<Node, int> heur = new Dictionary<Node, int>();
            Dictionary<Node, int> dist = new Dictionary<Node, int>();
            Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
            List<Node> potentials = new List<Node>(); 

            heur[root] = 0;
            dist[root] = 0;
            prev[root] = root;
            potentials.Add(root);
            while (potentials.Count > 0) 
            {
                Node Selected = LowestPotential(heur, potentials); 
                potentials.Remove(Selected);

                foreach (KeyValuePair<Node, int> Each in Selected.EdgesWeighted) 
                {
                    Node child = Each.Key;
                    int cost = Each.Value;
                    int potCost = dist[Selected] + cost;
                    int heurCost = potCost + heuristic(child, dest);
                    if (!dist.ContainsKey(child))
                    {
                        potentials.Add(child);
                        dist[child] = potCost;
                        prev[child] = Selected;
                        heur[child] = heurCost;
                    }
                    else if (potCost < dist[child])
                    {
                        dist[child] = potCost;
                        prev[child] = Selected;
                        heur[child] = heurCost;
                    }
                }
            }
            return NodePath.ToPath(prev, dest);
        }

        private static int heuristic(Node from, Node to)
        {
            return Math.Abs(from.Position.row - to.Position.row) + Math.Abs(from.Position.col - to.Position.col);
        }

        public static Dictionary<Node, Node> Dijkstra_Visit(Node root)
        {
            Dictionary<Node, int> dist = new Dictionary<Node, int>();
            Dictionary<Node, Node> prev = new Dictionary<Node, Node>();
            List<Node> potentials = new List<Node>();  

            dist[root] = 0;
            prev[root] = root;
            potentials.Add(root);
            while(potentials.Count > 0) 
            {
                Node Selected = LowestPotential(dist, potentials); 
                potentials.Remove(Selected);

                foreach( KeyValuePair<Node,int> Each in Selected.EdgesWeighted) 
                {
                    Node child = Each.Key;
                    int cost = Each.Value;
                    int potCost = dist[Selected] + cost;
                    if (!dist.ContainsKey(child))
                    {
                        potentials.Add(child);
                        dist[child] = potCost;
                        prev[child] = Selected;
                    } else if (potCost < dist[child])
                    {
                        dist[child] = potCost;
                        prev[child] = Selected;
                    }
                }
            }
            return prev;
        }

        private static Node LowestPotential(Dictionary<Node, int> dist, List<Node> potentials)
        {
            Node Selected = null;
            foreach(Node Each in potentials)
            {
                if (Selected == null || dist[Each] < dist[Selected])
                {
                    Selected = Each;
                }
            }
            return Selected;
        }
    }
}