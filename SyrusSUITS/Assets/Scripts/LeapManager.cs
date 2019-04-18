using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !UNITY_EDITOR
using SyrusLeapClient;
#endif

public class LeapManager : MonoBehaviour {

    public GameObject UIleft, UIright = null;

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

    public delegate void GestureSwipe(Vector3 pos, Vector3 dir);
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
    bool connected = false;

	// Use this for initialization
	void Start () {
        //Defaults the UI to be Invisible
        UIright.SetActive(false);
        UIleft.SetActive(false);
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

        left = new Hand();
        right = new Hand();

        //fills each vector array
        for (int i = 0; i < 9; i++) {
            //creates a sphere
            objs[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            //size of the object
            objs[i].transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
            objs[i].SetActive(false);
        }
        
	}
	
	// Update is called once per frame
	void Update () {
        if (connected) {

            for (int i = 0; i < 9; i++) {
                Vector3 p = leapOffset + rotation * points[i];
                objs[i].transform.position = ToUnityCoords(points[i]);//mainCamera.transform.position + mainCamera.transform.rotation * p;
            }

            left.palmPos = ToUnityCoords(points[0]);
            left.palmNorm = ToUnityCoordsDir(points[1]);

            right.palmPos = ToUnityCoords(points[2]);
            right.palmNorm = ToUnityCoordsDir(points[3]);

            //finds angle for area that the UI for the left hand will be visible
            //Debug.Log(Vector3.Angle(left.palmNorm, -mainCamera.transform.forward));
            if (Vector3.Angle(left.palmNorm, -mainCamera.transform.forward) < 30.0f) {
                //sets the UI as visible when the parameters are met
                UIleft.transform.position = left.palmPos;
                Vector3 dir = (left.palmPos - mainCamera.transform.position).normalized;
                UIleft.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
                UIleft.SetActive(true);
            }else{
                //sets Sthe UI as invisible when the parameters are met
                UIleft.SetActive(false);
            }
            
            //finds angle for the area that the UI for the right hand will be visible
            if(Vector3.Angle(right.palmNorm, -mainCamera.transform.forward) < 30.0f){
                //sets the UI as visible when the parameters are met
                UIright.transform.position = right.palmPos;
                Vector3 dir = (right.palmPos - mainCamera.transform.position).normalized;
                UIright.transform.rotation = Quaternion.LookRotation(dir, Vector3.up);
                UIright.SetActive(true);
            }else{
                //sets the UI as invisible when the parameters are met
                UIright.SetActive(false);
            }
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
        connected = true;
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
                        if (OnGestureSwipe != null) OnGestureSwipe(pos, dir);
                        break;
                    }
            }
        }

	#endif

    class Hand {
        public Vector3 palmPos;
        public Vector3 palmNorm;
        public Vector3 indexTip;
        public Vector3 indexDir;
    }

}

