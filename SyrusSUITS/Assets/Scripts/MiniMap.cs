using System;
using Assets.Scripts;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour {

    public List<Node> nodeMap = new List<Node>();
    public List<Node> route = new List<Node>();

    // Use this for initialization
    void Start () {
        LoadFromJson("/NodeMaps/sampleMap_1.json");

        List<Node> route = GetRoute(GetNodeByID(1), GetNodeByID(6));

        LogRoute(route);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public List<Node> GetRoute(Node source, Node destination)
    {
        Pathfinder pathFinder = new Pathfinder(nodeMap, GetNodeByID(1), GetNodeByID(6));
        pathFinder.Execute();

        return pathFinder.GetShortestPath(); 
    }

    public void LogRoute(List<Node> route)
    {
        string output = "Route: ";

        for(int i = 0; i < route.Count; i++)
        {
            output += route[i].id;

            if (i < route.Count - 1) output += " -> ";
        }

        Debug.Log(output);
    }

    public Node GetNodeByID(int id)
    {
        return nodeMap.Find(x => x.id.Equals(id));
    }

    void LogNodes()
    {
        foreach (Node node in nodeMap)
        {
            Debug.Log(node.id + " (" + node.position.x + ", " + node.position.y + ", " + node.position.z + ")");
        }
    }

    void LoadFromJson(string directory)
    {
        string filePath = Application.streamingAssetsPath + directory;

        Debug.Log(filePath);

        if (File.Exists(filePath))
        {
            string jsonContent = File.ReadAllText(filePath);
            NodeMap jsonNodeMap = JsonUtility.FromJson<NodeMap>(jsonContent);

            nodeMap = ConvertedJsonNodeMap(jsonNodeMap);
        }
        else
        {
            Debug.LogError("Cannot load data from " + filePath);
        }
    }

    public List<Node> ConvertedJsonNodeMap(NodeMap jsonNodeMap)
    {
        List<Node> nodeMap = new List<Node>();

        foreach(JsonNode jsonNode in jsonNodeMap.nodes)
        {
            Node node = new Node();

            node.id = jsonNode.id;
            node.position = new Vector3(jsonNode.position.x, jsonNode.position.y, jsonNode.position.z);
            node.adjacentNodeIDs = jsonNode.adjacentNodeIDs;

            nodeMap.Add(node);
        }

        return nodeMap;
    }

    [Serializable]
    public class NodeMap
    {
        public List<JsonNode> nodes;
    }

    [Serializable]
    public class JsonNode
    {
        public int id;
        public Positition position;
        public List<int> adjacentNodeIDs;

    }

    [Serializable]
    public class Positition
    {
        public float x;
        public float y;
        public float z;
    }
}
