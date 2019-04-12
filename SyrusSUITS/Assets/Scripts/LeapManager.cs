using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !UNITY_EDITOR
using SyrusLeapClient;
#endif

public class LeapManager : MonoBehaviour {

	#if !UNITY_EDITOR

	ClientBTManager cbm;

    Camera mainCamera;
    Vector3[] points;
    GameObject[] objs;

	// Use this for initialization
	void Start () {
		cbm = new ClientBTManager();
        cbm.PacketReceived += Recieved;
        cbm.Connected += OnConnect;
        cbm.Initialize();
        
        mainCamera = Camera.main;

        points = new Vector3[9];
        objs = new GameObject[9];

        for (int i = 0; i < 9; i++) {
            objs[i] = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            objs[i].transform.localScale = new Vector3(0.02f, 0.02f, 0.02f);
        }
        
	}
	
	// Update is called once per frame
	void Update () {

        for (int i = 0; i < 9; i++) {
            objs[i].transform.position = mainCamera.transform.position + mainCamera.transform.forward + mainCamera.transform.rotation * points[i] - 0.3f * mainCamera.transform.up;
        }
		
	}

    private async void OnConnect() {
        Debug.Log("Connected");
    }

	private async void Recieved(SyrusPacket packet) {
            switch (packet.id) {
                case 24: {

                        break;
                    }

                case 20:
                    {
                        for (int i = 0; i < 9; i++) {
                            float x = BitConverter.ToSingle(packet.data, 12 * i);
                            float y = BitConverter.ToSingle(packet.data, 12 * i + 4);
                            float z = BitConverter.ToSingle(packet.data, 12 * i + 8);

                            points[i] = new Vector3(x / 1000.0f, y / 1000.0f, z / 1000.0f);
                        }
                        
                        
                        break;
                    }
            }
        }

	#endif
}
