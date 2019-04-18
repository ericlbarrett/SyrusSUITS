using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !UNITY_EDITOR
using SyrusLeapClient;
#endif

public class LeapManager : MonoBehaviour {

    GameObject UIleft, UIright;

    private static LeapManager _Instance;
    public static LeapManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType<LeapManager>();
            }
            return _Instance;
        }
    }

    public delegate void GestureTap();
    public static event GestureTap OnGestureTap;

    public delegate void GestureSwipe();
    public static event GestureSwipe OnGestureSwipe;
    
	#if !UNITY_EDITOR
    ///Client manager 
	ClientBTManager cbm;

    Camera mainCamera;
    //Vector for all the finger points
    Vector3[] points;
    //array of ojects
    GameObject[] objs;
    //accounts for the offset of the leap on the hololens
    Vector3 leapOffset = new Vector3(0.0f, 0.08f, 0.04f);

    Quaternion rotation = Quaternion.Euler(110.0f, 0.0f, 0.0f);
    Hand left, right;

	// Use this for initialization
	void Start () {
        //Defaults the UI to be Invisible
        UIright.setActive(false);
        UIleft.setActive(false);
        //Server Manager Object
		cbm = new ClientBTManager();
        //Packets
        cbm.PacketReceived += Recieved;
        //connected
        cbm.Connected += OnConnect;
        cbm.Initialize();
        
        mainCamera = Camera.main;
        //gives indexed for each index
        points = new Vector3[9];
        objs = new GameObject[9];

        //fills each vector array
        for (int i = 0; i < 9; i++) {
            //creates a sphere
            objs[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //size of the object
            objs[i].transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        }
        
	}
	
	// Update is called once per frame
	void Update () {

        for (int i = 0; i < 9; i++) {
            Vector3 p = leapOffset + rotation * points[i];
            objs[i].transform.position = ToUnityCoords(points[i]);//mainCamera.transform.position + mainCamera.transform.rotation * p;
        }

        //finds angle for area that the UI for the left hand will be visible
        if (Vector3.angle(points[1], -mainCamera.transform.forward) < 20.0f) {
            //sets the UI as visible when the parameters are met
            UIleft.setActive(true);
        }else{
            //sets the UI as invisible when the parameters are met
            UIleft.setActive(false);
        }
        
        //finds angle for the area that the UI for the right hand will be visible
        if(Vector3.angle(points[3], -mainCamera.transform.forward) < 20.0f){
            //sets the UI as visible when the parameters are met
            UIright.setActive(true);
        }else{
            //sets the UI as invisible when the parameters are met
            UIright.setActive(false);
        }
		
	}

    public Vector3 ToUnityCoords(Vector3 p) {
        return mainCamera.transform.position + mainCamera.transform.rotation * (leapOffset + rotation * p);
    }

    public Vector3 ToUnityCoordsDir(Vector3 p) {
        return (mainCamera.transform.rotation * rotation * p).normalized;
    }

    private async void OnConnect() {
        Debug.Log("Connected");
    }

    private Vector3 readVector(byte[] arr, int offset) {
        float x = BitConverter.ToSingle(arr, offset);
        float y = BitConverter.ToSingle(arr, offset + 4);
        float z = BitConverter.ToSingle(arr, offset + 8);

        return new Vector3(-x / 1000.0f, y / 1000.0f, z / 1000.0f);
    }

	private async void Recieved(SyrusPacket packet) {
            switch (packet.id) {
                case 20: // Update message
                    {
                        for (int i = 0; i < 9; i++) {
                            points[i] = readVector(packet.data, 12 * i);
                        }
                        
                        break;
                    }
                case 21: // Screen tap gesture
                    {
                        Debug.Log("Gesture: Tap");
                        Vector3 pos = readVector(packet.data, 0);
                        Vector3 dir = readVector(packet.data, 12).normalized;
                        
                        break;
                    }
                case 22: // Swipe gesture
                    {
                        Debug.Log("Gesture: Swipe");
                        Vector3 pos = readVector(packet.data, 0);
                        Vector3 dir = readVector(packet.data, 12).normalized;
                        break;
                    }
            }
        }

	#endif

    class Hand {
        Vector3 palmPos;
        Vector3 palmNorm;
        Vector3 indexTip;
        Vector3 indexDir;
    }

}

