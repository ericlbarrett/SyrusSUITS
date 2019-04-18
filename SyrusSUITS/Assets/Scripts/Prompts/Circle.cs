using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Circle : MonoBehaviour {

    public float speed = 45.0f; // Rot speed in degrees / sec

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        transform.rotation *= Quaternion.Euler(0.0f, speed * Time.deltaTime, 0.0f);
	}
}
