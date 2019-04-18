using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacingProcedure : MonoBehaviour {

    public GameObject thing;
    public GameObject placingPanel;
    public delegate void TestDelegate(); // This defines what type of method you're going to call.
    public TestDelegate m_methodToCall; // This is the variable holding the method you're going to call.


    float moveSpeed = 0.025f;
    // Use this for initialization
    void Start () {
        Debug.Log("Move Procedure Panel with arrow keys and R and T");
    }

	// Update is called once per frame
	void Update () {
		if(placingPanel.gameObject.activeInHierarchy)
        {
            // pos Y direction
            if (Input.GetKey("up"))
            {
                Vector3 pos = placingPanel.gameObject.transform.position;
                placingPanel.gameObject.transform.position = new Vector3(pos.x, pos.y + moveSpeed,pos.z);

                pos = transform.position;
                transform.position =  new Vector3(pos.x, pos.y + moveSpeed, pos.z);
            }
            // neg Y direction
            if (Input.GetKey("down"))
            {
                Vector3 pos = placingPanel.gameObject.transform.position;
                placingPanel.gameObject.transform.position = new Vector3(pos.x , pos.y - moveSpeed, pos.z);

                pos = transform.position;
                transform.position = new Vector3(pos.x, pos.y - moveSpeed, pos.z);
            }
            // pos Z direction
            if (Input.GetKey("left"))
            {
                Vector3 pos = placingPanel.gameObject.transform.position;
                placingPanel.gameObject.transform.position = new Vector3(pos.x - moveSpeed, pos.y, pos.z );

                pos = transform.position;
                transform.position = new Vector3(pos.x - moveSpeed, pos.y, pos.z);
            }
            // neg Z direction
            if (Input.GetKey("right"))
            {
                Vector3 pos = placingPanel.gameObject.transform.position;
                placingPanel.gameObject.transform.position = new Vector3(pos.x + moveSpeed, pos.y, pos.z);

                pos = transform.position;
                transform.position = new Vector3(pos.x + moveSpeed, pos.y, pos.z);
            }
            // pos Z direction
            if (Input.GetKey("r"))
            {
                Vector3 pos = placingPanel.gameObject.transform.position;
                placingPanel.gameObject.transform.position = new Vector3(pos.x, pos.y, pos.z + moveSpeed);

                pos = transform.position;
                transform.position = new Vector3(pos.x, pos.y, pos.z + moveSpeed);
            }
            // neg Z direction
            if (Input.GetKey("t"))
            {
                Vector3 pos = placingPanel.gameObject.transform.position;
                placingPanel.gameObject.transform.position = new Vector3(pos.x, pos.y, pos.z - moveSpeed);

                pos = transform.position;
                transform.position = new Vector3(pos.x, pos.y, pos.z - moveSpeed);
            }
        }
	}
    public void rotateX()
    {
        float val = thing.transform.eulerAngles.x;
        val += 90;
        if(val > 360)
        {
            val = val - 360; ;
        }
        thing.transform.eulerAngles = new Vector3(val, thing.transform.eulerAngles.y, thing.transform.eulerAngles.z);
    }
    public void rotateY()
    {
        float val = thing.transform.eulerAngles.y;
        val += 90;
        if (val > 360)
        {
            val = val - 360; ;
        }
        thing.transform.eulerAngles = new Vector3(thing.transform.eulerAngles.x, val, thing.transform.eulerAngles.z);
    }
    public void rotateZ()
    {
        float val = thing.transform.eulerAngles.z;
        val += 90;
        if (val > 360)
        {
            val = val - 360; ;
        }
        thing.transform.eulerAngles = new Vector3(thing.transform.eulerAngles.x, thing.transform.eulerAngles.y, val);
    }
    public void loadPlacingProcedure()
    {
        Debug.Log(placingPanel.activeInHierarchy);
        placingPanel.SetActive(!placingPanel.activeInHierarchy);
        CenterOnThing();
    }
    public void PlacingProcedureOn()
    {
        placingPanel.SetActive(true);
        CenterOnThing();
    }
    public void PlacingProcedureOff()
    {
        placingPanel.SetActive(false);

        //need to tell it what to call when done placing
        m_methodToCall();
    }
    public void CenterOnThing()
    {
        //thing position (world)
        Vector3 thingPos = thing.gameObject.transform.position;
        //thing dimensions
        GameObject all = GameObject.Find("all");
        Vector3 pos = all.transform.position;
        placingPanel.transform.position = new Vector3(pos.x, pos.y + 0.25f, pos.z );
    }





}





