using System.Collections;
using System.Collections.Generic;
using Vuforia;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UI.ProceduralImage;

public class RouteCalibrator : MonoBehaviour, ITrackableEventHandler {

    private TrackableBehaviour mTrackableBehaviour;
    public ProceduralImage pi;

    public static Vector3 originPosition = new Vector3();

    bool charging = false;

    List<Vector3> vSamples;
    List<Quaternion> qSamples;

    // Use this for initialization
    void Start()
    {
        mTrackableBehaviour = GetComponent<TrackableBehaviour>();
        if (mTrackableBehaviour)
        {
            mTrackableBehaviour.RegisterTrackableEventHandler(this);
        }

        vSamples = new List<Vector3>();
        qSamples = new List<Quaternion>();
    }

    // Update is called once per frame
    void Update()
    {
        if (charging)
        {
            pi.fillAmount += Time.deltaTime / 2.0f;

            // Sample the rotations and points
            vSamples.Add(transform.position);
            qSamples.Add(transform.rotation);

            if (pi.fillAmount >= 1.0f)
            {
                charging = false;

                //Reveal Node position
                //setOriginPosition();
                originPosition = transform.position;
                System.Diagnostics.Debug.Write(originPosition);
                NavigationService.Instance.updateNodeMapPositions(NavigationService.nodeMap);
                NavigationService.route = NavigationService.Instance.GetRoute(NavigationService.Instance.GetNodeByID(1), NavigationService.Instance.GetNodeByID(8));
                MiniMap.Instance.DrawRoute(NavigationService.route);
                vSamples.Clear();
                qSamples.Clear();
            }
        }
    }

    void setOriginPosition()
    {
        float x = transform.position.x;
        float y = transform.position.y;
        float z = transform.position.z;

        originPosition = new Vector3(x, y, z);
        System.Diagnostics.Debug.Write(originPosition);
    }

    private Quaternion calcAvg(List<Quaternion> rotationlist)
    {
        float x = 0, y = 0, z = 0, w = 0;
        foreach (Quaternion q in rotationlist)
        {
            x += q.x; y += q.y; z += q.z; w += q.w;
        }
        float k = 1.0f / Mathf.Sqrt(x * x + y * y + z * z + w * w);
        return new Quaternion(x * k, y * k, z * k, w * k);
    }

    private Vector3 calcAvg(List<Vector3> pointList)
    {
        Vector3 sum = Vector3.zero;
        foreach (Vector3 v in pointList)
        {
            sum += v;
        }

        return sum / pointList.Count;
    }

    public void OnTrackableStateChanged(TrackableBehaviour.Status previousStatus,
                                        TrackableBehaviour.Status newStatus)
    {
        if (newStatus == TrackableBehaviour.Status.DETECTED ||
            newStatus == TrackableBehaviour.Status.TRACKED ||
            newStatus == TrackableBehaviour.Status.EXTENDED_TRACKED)
        {

            if (!NavigationService.Instance.calibrated)
            {
                pi.fillAmount = 0.0f;
                charging = true;
            }
        }
    }
}

