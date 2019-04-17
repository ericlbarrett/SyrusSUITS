using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(LineRenderer))]
public class Arrow : MonoBehaviour {

    public GameObject arrow;
    public Vector3 beg;
    public Vector3 end;

    public int nExtra = 10;

    public float duration = 1.5f;
    public float hold = 0.5f;
    float timer = 0.0f;

    

    // Use this for initialization
    void Start () {
        Material mat = Resources.Load("PromptMat", typeof(Material)) as Material;
        LineRenderer lr = GetComponent<LineRenderer>();
        lr.endWidth = 0.02f;
        lr.startWidth = 0.02f;
        lr.useWorldSpace = false;
        lr.material = mat;

        Mesh mesh = Resources.Load("ArrowMesh", typeof(Mesh)) as Mesh;
        arrow = new GameObject();
        arrow.transform.SetParent(transform);
        arrow.name = "ArrowHead";
        arrow.AddComponent<MeshRenderer>().material = mat;
        arrow.AddComponent<MeshFilter>().mesh = mesh;
        arrow.transform.localScale = new Vector3(0.04f, 0.18f, 0.04f);

    }
	
	// Update is called once per frame
	void Update () {
        timer += Time.deltaTime;
        if (timer > duration + hold) timer = 0.0f;

        float t = timer / duration;
        if (t > 1.0f) t = 1.0f;

        t = t * t;

        // Calculate the midpoint
        Vector3 mid = (beg + end) / 2.0f;
        mid.y += Vector3.Distance(beg, end) * 0.25f;

        Vector3 tEnd = bezier(beg, mid, end, t);

        LineRenderer lr = GetComponent<LineRenderer>();

        

        Vector3[] pts = new Vector3[nExtra + 2];
        pts[0] = beg;
        pts[pts.Length - 1] = tEnd;

        for (int i = 1; i <= nExtra; i++) {
            pts[i] = bezier(beg, mid, end, i * t / pts.Length);
        }
        lr.positionCount = pts.Length;
        lr.SetPositions(pts);

        // Place the arrow at the end
        Vector3 dir = transform.rotation * (tEnd - pts[pts.Length - 2]).normalized;
        arrow.transform.rotation = Quaternion.LookRotation(dir, transform.up) * Quaternion.Euler(0.0f, 180.0f, 0.0f);
        arrow.transform.position = transform.position + (transform.rotation * tEnd + (0.01f * dir));
    }

    // Bezier curve interpolation
    private Vector3 bezier(Vector3 p0, Vector3 p1, Vector3 p2, float t) {
        // Clamp t
        if (t > 1.0f) t = 1.0f;
        if (t < 0.0f) t = 0.0f;

        float rev = 1.0f - t;
        return rev * rev * p0 + 2 * rev * t * p1 + t * t * p2;
    }
}
