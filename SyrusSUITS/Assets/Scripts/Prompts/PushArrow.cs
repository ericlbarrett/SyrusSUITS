using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class PushArrow : MonoBehaviour {

    public GameObject arrow;
    public float beg;
    public float end;

    public float duration = 1.5f;
    public float hold = 0.5f;
    float timer = 0.0f;



    // Use this for initialization
    void Start() {
        Material mat = Resources.Load("PromptMat", typeof(Material)) as Material;
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.endWidth = 0.01f;
        lr.startWidth = 0.01f;
        lr.useWorldSpace = false;
        lr.material = mat;

        Mesh mesh = Resources.Load("ArrowMesh", typeof(Mesh)) as Mesh;
        arrow = new GameObject();
        arrow.transform.SetParent(transform);
        arrow.name = "ArrowHead";
        arrow.AddComponent<MeshRenderer>().material = mat;
        arrow.AddComponent<MeshFilter>().mesh = mesh;
        arrow.transform.localScale = new Vector3(0.02f, 0.09f, 0.02f);

    }

    // Update is called once per frame
    void Update() {
        timer += Time.deltaTime;
        if (timer > duration + hold) timer = 0.0f;

        float t = timer / duration;
        if (t > 1.0f) t = 1.0f;

        t = t * t;



        LineRenderer lr = GetComponent<LineRenderer>();

        float dist = end - beg;
        Vector3 p0 = new Vector3(0.0f, beg, 0.0f);
        Vector3 p1 = new Vector3(0.0f, beg + dist * t, 0.0f);

        Vector3[] pts = new Vector3[2];
        pts[0] = p0;
        pts[1] = p1;

        lr.positionCount = pts.Length;
        lr.SetPositions(pts);

        // Place the arrow at the end
        if (dist < 0) {
            arrow.transform.localRotation = Quaternion.Euler(270.0f, 0.0f, 0.0f);
        } else {
            arrow.transform.localRotation = Quaternion.Euler(90.0f, 0.0f, 0.0f);
        }
        
        arrow.transform.localPosition = p1 + new Vector3(0.0f, -0.003f, 0.0f);

    }
}
