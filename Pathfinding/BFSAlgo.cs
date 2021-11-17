using System;
using System.Collections.Generic;
using System.Text;

namespace AIV_Exam_SimoneSantagati
{
    class BFSAlgo
    {
        public static NodePath Iterative(Node root)
        {
            NodePath visited = new NodePath();            
            Queue<Node> toVisit = new Queue<Node>();      
            visited.Add(root);                            
            toVisit.Enqueue(root);                        
            while (toVisit.Count > 0)                     
            {
                Node parent = toVisit.Dequeue();              
                foreach(Node eachChild in parent.Edges)       
                {
                    if (!visited.Has(eachChild))              
                    {
                        visited.Add(eachChild);               
                        toVisit.Enqueue(eachChild);           
                    }
                }
            }
            return visited; 
        }
    }
}
