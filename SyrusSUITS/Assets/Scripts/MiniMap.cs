using System;
using Assets.Scripts;
using System.Collections;
using System.IO;
using System.Collections.Generic;
using UnityEngine;

public class MiniMap : MonoBehaviour {

    private static MiniMap _Instance;

    public static MiniMap Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType<MiniMap>();
            }
            return _Instance;
        }
    }

    public LineRenderer line;
    float scaleFactor = 1 / 1.0f; // The scale of the minimap relative to the world scale.
                                  
    // Use this for initialization
    void Start () {

        line = GetComponent<LineRenderer>();

        //DrawNodes(NavigationService.nodeMap);
        //DrawRoute(NavigationService.route);
        //DrawNodes(NavigationService.nodeMap);
    }

    void Awake() {
        _Instance = this;
        NavigationService.Instance.MapLoaded += OnMapLoad;
    }
	
	// Update is called once per frame
	void Update () {
        
    }

    void OnMapLoad() {
        //string modelName = NavigationService.Instance.modelName;
        //Debug.Log("Loading map: " + modelName);

        //GameObject map = Instantiate((GameObject)Resources.Load(modelName), Vector3.zero, Quaternion.Euler(0, 0, 90), transform);
        //map.transform.localScale = scaleFactor * map.transform.localScale;

    }

    public void DrawRoute(List<Node> route)
    {   
        line.material = new Material(Shader.Find("Sprites/Default"));
        line.positionCount = route.Count;

        Vector3[] positions = new Vector3[route.Count];
        
        for(int i = 0; i < route.Count; i++)
        {
            line.SetPosition(i, scaleFactor * route[i].position);
        }
    }

    public void DrawNodes(List<Node> nodeMap)
    {
        for(int i = 0; i < nodeMap.Count; i++)
        {
            //GameObject node = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //node.name = "Node " + nodeMap[i].id;
            //node.transform.position = nodeMap[i].position;
            //node.transform.localScale = new Vector3(0.08f, 0.08f, 0.08f);
        }
    }
}
