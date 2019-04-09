using Assets.Scripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationService : MonoBehaviour {

	private static NavigationService _Instance;
    public static NavigationService Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType<NavigationService>();
            }
            return _Instance;
        }
    }
	Vector3 navOrigin;
	Quaternion navRotation;

	public bool calibrated = false;

	// Use this for initialization
	void Start () {
		_Instance = this;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	public void SetOrigin(Vector3 origin, Quaternion rotation) {
		navOrigin = origin;
		navRotation = rotation;
		calibrated = true;
		Debug.Log("Nav Origin Set");
	}

}
