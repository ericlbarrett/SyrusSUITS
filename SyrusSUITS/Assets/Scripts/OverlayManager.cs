using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OverlayManager : MonoBehaviour
{

    private static OverlayManager _Instance;
    public static OverlayManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType<OverlayManager>();
            }
            return _Instance;
        }
    }
    Layout layout = new Layout();           // Layout Class holds the information of the overlay 
    string path;                            // Directory of where the JSON files are located
    List<string> overlayFiles;
    List<string> overlayNames;
    List<GameObject> objs = new List<GameObject>(); // Module game objects

    Step currentStep;

    // Use this for initialization
    void Start() {
        ProcedureManager_2.OnStepChanged += OnStepChanged;
        Instance.path = Application.streamingAssetsPath + "/OverlayLayouts/";

        PreloadOverlays("/OverlayLayouts/");
        PrintFileNames();
        ChooseTaskboard();
        
    }

    void PreloadOverlays(string dir) {
        overlayFiles = new List<string>();
        overlayNames = new List<string>();

        try {
            string location = Application.streamingAssetsPath + dir;
            foreach (string file in System.IO.Directory.GetFiles(location)) {
                string label = file.Replace(location, "");
                if (label.EndsWith(".json")) {
                    string contents = System.IO.File.ReadAllText(location + label);
                    OverlayPreload preload = JsonUtility.FromJson<OverlayPreload>(contents);

                    // TODO: Image activator
                    Debug.Log(preload.name + " Activator:" + preload.activator);

                    overlayNames.Add(preload.name);
                    overlayFiles.Add(label);
                }
            }
        } catch (System.Exception ex) {
            Debug.Log("Error: JSON input. " + ex.Message);
        }
    }

    void PrintFileNames() {
        if (overlayFiles != null) {
            for (int i = 0; i < overlayFiles.Count; i++) {
                Debug.Log(overlayFiles[i]);
            }
        }
    }

    private void OnStepChanged(Step step) {
        if (step != null)
        {
            //Debug.Log(step);
            currentStep = step;
            //Debug.Log(currentStep);
        }
    }

    public void LoadOverlay(string name, Vector3 pos, Quaternion rot) {
        try
        {
            string temp = path + name;
            if (System.IO.File.Exists(temp))
            {
                string contents = System.IO.File.ReadAllText(temp);
                Instance.layout = JsonUtility.FromJson<Layout>(contents);
                CreateTaskboard();
                this.GetComponent<PlacingProcedure>().PlacingProcedureOn();
                this.GetComponent<PlacingProcedure>().m_methodToCall = GameObject.Find("ProcedureManager").gameObject.GetComponent<ProcedureManager_2>().ChooseProcedure;
            }
            else
            {
                Debug.Log("Error: Unable to read " + name + " file, at " + temp);
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error: Taskboard JSON input. " + ex.Message);
        }
    }

    // Load the taskboard
    void LoadTaskboard(int x)
    {
        try
        {
            string temp = path + overlayFiles[x];
            if (System.IO.File.Exists(temp))
            {
                string contents = System.IO.File.ReadAllText(temp);
                Instance.layout = JsonUtility.FromJson<Layout>(contents);
                //Debug.Log(layout.name);
                //foreach(Modules m in layout.modules)
                //Debug.Log(m.id + " " + m.position.x + " " + m.position.y + " " + m.rotation + " " + m.size.x + " " + m.type);
                CreateTaskboard();
                this.GetComponent<PlacingProcedure>().PlacingProcedureOn();
                this.GetComponent<PlacingProcedure>().m_methodToCall = GameObject.Find("ProcedureManager").gameObject.GetComponent<ProcedureManager_2>().ChooseProcedure;
            }
            else
            {
                Debug.Log("Error: Unable to read " + overlayFiles[x] + " file, at " + temp);
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error: Taskboard JSON input. " + ex.Message);
        }
    }

    //Choose a Taskboard
    public void ChooseTaskboard()
    {
        //foreach (Modules m in layout.modules)
        //{
        //    Transform trans = transform.Find(m.id);
            //if (trans)
            //{
            //    solidmode(trans.gameobject);
            //}
        //}

        OptionsMenu opts = OptionsMenu.Instance("Choose A Taskboard", true);
        opts.OnSelection += LoadTaskboard;
        if (overlayNames.Count > 0)
        {
            for (int i = 0; i < overlayNames.Count; i++)
            {
                opts.AddItem(overlayNames[i], i);
            }
            opts.ResizeOptions();
        }
        else
        {
            Debug.Log("Error: No Overlay layouts loaded");
        }
    }
    
    //Create the Taskboard
    void CreateTaskboard()
    {
        Vector3 corner = new Vector3(-(float)layout.size.x / 2.0f, (float)layout.size.y / 2.0f, (float)layout.size.z / 2.0f);
        GameObject cube;
        //Material mat = Resources.Load("ModuleMat", typeof(Material)) as Material;

        // whole taskboard
        cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.parent = transform;
        cube.name = "all";
        cube.transform.localScale = new Vector3((float)layout.size.x, (float)layout.size.y, (float)layout.size.z);
        cube.transform.localPosition = Vector3.zero;

        foreach (Modules m in layout.modules)
        {
            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.parent = transform;
            cube.name = m.id;
            cube.transform.localScale = new Vector3((float)m.size.x, (float)m.size.y, (float)m.size.z);
            Vector3 halfScle = new Vector3((float)m.size.x / 2, (float)m.size.y / 2, -(float)m.size.z / 2);
            cube.transform.localPosition = corner + new Vector3((float)m.position.x, (float)m.position.y, -(float)m.position.z) + halfScle;

            cube.transform.localRotation = Quaternion.Euler(m.rotation.UnityVec());

  
            objs.Add(cube);
        }
        transform.localPosition = new Vector3(0, 0, 0.55f);

        //Transform panel = transform.Find("ProcedurePanel");
        //panel.localPosition = new Vector3(0.0f, 0.11f, layout.length / 2.0f + 0.00f);
        //moveTaskboard();
    }
}

//TASKBOARD LAYOUT
[System.Serializable]
public class Layout
{
    public string name;
    public Vec3 size;
    public List<Modules> modules = new List<Modules>();
}

[System.Serializable]
public class Modules
{
    public string type;
    public string id;
    public Vec3 size;
    public Vec3 position;
    public Vec3 rotation;
}

[System.Serializable]
public class Vec2
{
    public double x;
    public double y;
}

[System.Serializable]
public class Vec3
{
    public double x;
    public double y;
    public double z;

    public Vector3 UnityVec() {
        return new Vector3((float)x, (float)y, (float)z);
    }
}

[System.Serializable]
public class OverlayPreload { // Contains basic information about an overlay for preloading
    public string name;
    public bool activator;
    public Vec3 activator_pos;
    public int activator_target;
}