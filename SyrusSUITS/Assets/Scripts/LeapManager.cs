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

    Vector3 leapOffset = new Vector3(0.0f, 0.08f, 0.04f);
    float angle = 110.0f; // Degrees

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
            Vector3 p = leapOffset + Quaternion.Euler(angle, 0.0f, 0.0f) * points[i];
            objs[i].transform.position = mainCamera.transform.position + mainCamera.transform.rotation * p;
        }
		
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
                case 24: {

                        break;
                    }

                case 20:
                    {
                        for (int i = 0; i < 9; i++) {
                            points[i] = readVector(packet.data, 12 * i);
                        }
                        
                        break;
                    }
                case 21:
                    {
                        Vector3 pos = readVector(packet.data, 0);
                        Vector3 dir = readVector(packet.data, 12).normalized;
                        
                        break;
                    }
            }
        }

	#endif
}
