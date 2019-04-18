using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace Assets.Scripts
{   
    public class Node
    {
        // For general use
        public int id;
        public Vector3 position;
        public List<int> adjacentNodeIDs;

        // Used by Pathfinder (Dijkstra's Algorithm)
        public bool visited = false;
        public float shortestDistanceFromSource = float.MaxValue;
        public Node previousNode;
    }
}
