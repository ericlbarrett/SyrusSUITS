using System;
using Assets.Scripts;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour {

    public LineRenderer line;
    float scaleFactor = 1 / 25.0f; // The scale of the minimap relative to the world scale.
                                  
    // Use this for initialization
    void Start () {

        line = GetComponent<LineRenderer>();

        DrawNodes(NavigationService.nodeMap);
        DrawRoute(NavigationService.route);
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    public void DrawRoute(List<Node> route)
    {   
        line.material = new Material(Shader.Find("Particles/Alpha Blended Premultiply"));
        line.positionCount = route.Count;

        Vector3[] positions = new Vector3[route.Count];
        
        for(int i = 0; i < route.Count; i++)
        {
            line.SetPosition(i, route[i].position);
        }
    }

    public void DrawNodes(List<Node> nodeMap)
    {
        for(int i = 0; i < nodeMap.Count; i++)
        {
            GameObject node = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            node.name = "Node " + nodeMap[i].id;
            node.transform.position = nodeMap[i].position;
            node.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
        }
    }
}
