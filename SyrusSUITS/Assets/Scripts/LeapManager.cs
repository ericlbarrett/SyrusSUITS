using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if !UNITY_EDITOR
using SyrusLeapClient;
#endif

public class LeapManager : MonoBehaviour {

    public Transform test;

    // void Update () {
    //     Camera mainCamera = Camera.main;
    //     Vector3 leapOffset = new Vector3(0.0f, 0.08f, 0.04f);
    //     float angle = 100.0f; // Degrees
    //         Vector3 p = leapOffset + Quaternion.Euler(angle, 0.0f, 0.0f) * new Vector3(0, 0.1f, 0);
    //     test.position = mainCamera.transform.position + mainCamera.transform.rotation * p;
    //     Debug.DrawRay(mainCamera.transform.position + mainCamera.transform.rotation * leapOffset, mainCamera.transform.rotation * Quaternion.Euler(angle, 0.0f, 0.0f) * Vector3.up);
	// }

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

                            points[i] = new Vector3(-x / 1000.0f, y / 1000.0f, z / 1000.0f);
                        }
                        
                        break;
                    }
            }
        }

	#endif
}
