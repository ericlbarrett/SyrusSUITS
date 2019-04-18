using System.Collections.Generic;
using UnityEngine;

namespace Assets.Scripts
{
    public class Pathfinder
    {
        public List<Node> nodes;
        public Node source;
        public Node destination;

        // Temporary variables used during alorithm execution
        private Node currentNode;
        private Node adjacentNode;

        // Constructor
        public Pathfinder(List<Node> nodes, Node source, Node destination)
        {
            this.nodes = nodes;
            this.source = source;
            this.destination = destination;

            currentNode = source;
            currentNode.shortestDistanceFromSource = 0;
            currentNode.previousNode = null;
        }

        // Returns Shortest Path
        // Call Execute() before calling this function
        public List<Node> GetShortestPath()
        {
            List<Node> path = new List<Node>();

            currentNode = destination;
            path.Insert(0, currentNode);

            while(currentNode.previousNode != null)
            {
                // Place node in beginning of list so path will be in order
                path.Insert(0, currentNode.previousNode);
                currentNode = currentNode.previousNode;
            }

            return path;
        }

        // Dijkstra's Algorithm is executed here
        public void Execute()
        {
            while(currentNode != null)
            {
                SetAdjacentNodeProperties();

                currentNode.visited = true;
                currentNode = GetNextNode();
            }
        }

        private void SetAdjacentNodeProperties()
        {
            foreach(int adjacentNodeID in currentNode.adjacentNodeIDs)
            {
                adjacentNode = GetNodeByID(adjacentNodeID);

                if (!adjacentNode.visited) {

                    float distance = GetDistance(currentNode, adjacentNode) + currentNode.shortestDistanceFromSource;

                    if (distance < adjacentNode.shortestDistanceFromSource)
                    {
                        adjacentNode.shortestDistanceFromSource = distance;
                        adjacentNode.previousNode = currentNode;
                    }     
                }
            }
        }

        private Node GetNextNode()
        {
            // First try to get adjacent unvisited node
            Node nextNode = GetAdjacentUnvisitedNode();
            if (nextNode != null) return nextNode;

            // If there's no adjacent unvisted nodes, try to get any unvisted node
            nextNode = GetAnyUnVisitedNode();
            if (nextNode != null) return nextNode;

            // If no other nodes, return null
            return null;
        }

        private Node GetAdjacentUnvisitedNode()
        {
            Node nearestNode = null;
            float? nearestNodeDistance = null;

            foreach (int adjacentNodeID in currentNode.adjacentNodeIDs)
            {
                Node adjacentNode = GetNodeByID(adjacentNodeID);

                if (!adjacentNode.visited)
                {
                    float distance = GetDistance(currentNode, adjacentNode);

                    if (nearestNodeDistance == null || distance < nearestNodeDistance)
                    {
                        nearestNode = adjacentNode;
                        nearestNodeDistance = distance;
                    }
                }
            }

            return nearestNode;
        }

        private Node GetAnyUnVisitedNode()
        {
            foreach(Node node in nodes)
            {
                if (!node.visited) return node;
            }

            return null;
        }

        public float GetDistance(Node source, Node destination)
        {
            return Mathf.Sqrt(Mathf.Pow(destination.position.x - source.position.x, 2) +
                              Mathf.Pow(destination.position.y - source.position.y, 2) +
                              Mathf.Pow(destination.position.z - source.position.z, 2));
        }

        public Node GetNodeByID(int id)
        {
            return nodes.Find(x => x.id.Equals(id));
        }

    }
}
