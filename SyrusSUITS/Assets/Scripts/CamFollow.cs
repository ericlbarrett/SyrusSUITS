using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamFollow : MonoBehaviour {

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		//GetComponent<RectTransform>().position = Camera.main.transform.position + 0.05f * Camera.main.transform.forward;
		//GetComponent<RectTransform>().rotation = Quaternion.Inverse(Camera.main.transform.rotation);
	}
}
