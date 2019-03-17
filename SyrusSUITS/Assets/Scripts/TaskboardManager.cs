using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskboardManager : MonoBehaviour
{

    private static TaskboardManager _Instance;
    public static TaskboardManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType<TaskboardManager>();
            }
            return _Instance;
        }
    }
    Layout layout = new Layout();           // Layout Class holds the information of the taskboard 
    Step currentStep;
    string path;                            // Directory of where the JSON files are located
    string fileName = "taskboard.json";     // File name of the taskboard layout [ need to add option to load more ]
    List<string> files;
    List<string> taskboardNames;
    List<GameObject> objs = new List<GameObject>();

    // Use this for initialization
    void Start()
    {
        ProcedureManager_2.OnStepChanged += OnStepChanged;
        Instance.path = Application.streamingAssetsPath + "/TaskboardLayouts/";
        files = new List<string>();
        taskboardNames = new List<string>();

        LoadFileNames("/TaskboardLayouts/");
        PrintFileNames();
        ChooseTaskboard();
    }

    // Update is called once per frame
    void Update()
    {

    }
    void PrintFileNames()
    {
        for(int i = 0; i < files.Count; i++)
        {
            Debug.Log(files[i]);
        }
    }
    void LoadFileNames(string dir)
    {
        string location = Application.streamingAssetsPath;
        try
        {
            string temp = location + dir;
            foreach (string file in System.IO.Directory.GetFiles(temp))
            {
                string label = file.Replace(temp, "");
                if (label.EndsWith(".json"))
                {
                    string contents = System.IO.File.ReadAllText(temp + label);
                    TaskNames taskname = JsonUtility.FromJson<TaskNames>(contents);
                    //Debug.Log(procedureNames.title);
                    taskboardNames.Add(taskname.name);
                    files.Add(label);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error: JSON input. " + ex.Message);
        }
    }

    private void OnStepChanged(Step step)
    {
        if (step != null)
        {
            //Debug.Log(step);
            currentStep = step;
            Debug.Log(currentStep);
        }
    }
    // Load the taskboard
    void LoadTaskboard(int x)
    {
        try
        {
            //Debug.Log(path + files[x]);
            string temp = path + files[x];
            if (System.IO.File.Exists(temp))
            {
                string contents = System.IO.File.ReadAllText(temp);
                Instance.layout = JsonUtility.FromJson<Layout>(contents);
                //Debug.Log(layout.name);
                //foreach(Modules m in layout.modules)
                //Debug.Log(m.id + " " + m.position.x + " " + m.position.y + " " + m.rotation + " " + m.size.x + " " + m.type);
                CreateTaskboard();
            }
            else
            {
                Debug.Log("Error: Unable to read " + fileName + " file, at " + temp);
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
        if (taskboardNames.Count > 0)
        {
            for (int i = 0; i < taskboardNames.Count; i++)
            {
                opts.AddItem(taskboardNames[i], i);
            }
            opts.ResizeOptions();
        }
        else
        {
            Debug.Log("Error: No taskboard layouts loaded");
        }
    }
    //Create the Taskboard
    void CreateTaskboard()
    {
        Vector3 topLeft = new Vector3(-layout.width / 2.0f, 0, layout.length / 2.0f);
        GameObject cube;
        //Material mat = Resources.Load("ModuleMat", typeof(Material)) as Material;

        foreach (Modules m in layout.modules)
        {
            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.parent = transform;
            cube.name = m.id;
            cube.transform.localScale = new Vector3((float)m.size.x, 0.001F, (float)m.size.y);
            Vector3 halfScle = new Vector3((float)m.size.x / 2, 0, -(float)m.size.y / 2);
            cube.transform.localPosition = topLeft + new Vector3((float)m.position.x, .001f, -(float)m.position.y) + halfScle;


            //cube.GetComponent<Renderer>().material = mat;

            //LineRenderer lr = cube.AddComponent<LineRenderer>();
            //Vector3[] pts = new Vector3[6];

            //pts[0] = new Vector3(0.25f, 0.0f, 0.5f);
            //pts[1] = new Vector3(0.5f, 0.0f, 0.5f);
            //pts[2] = new Vector3(0.5f, 0.0f, -0.5f);
            //pts[3] = new Vector3(-0.5f, 0.0f, -0.5f);
            //pts[4] = new Vector3(-0.5f, 0.0f, 0.5f);
            //pts[5] = new Vector3(0.25f, 0.0f, 0.5f);

            //lr.positionCount = 6;
            //lr.SetPositions(pts);
            //lr.widthMultiplier = 0.005f;
            //lr.enabled = false;
            //lr.useWorldSpace = false;
            //lr.numCornerVertices = 3;

            objs.Add(cube);
        }

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
    public float length;
    public float width;
    public List<Modules> modules = new List<Modules>();
}

[System.Serializable]
public class Modules
{
    public string type;
    public string id;
    public Vec2 size;
    public Vec2 position;
    public int rotation;
}

[System.Serializable]
public class Vec2
{
    public double x;
    public double y;
}

[System.Serializable]
public class TaskNames
{
    public string name;
}
