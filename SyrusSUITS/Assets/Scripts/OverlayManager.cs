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
    Layout layout = null;           // Layout Class holds the information of the overlay 
    string path;                            // Directory of where the JSON files are located
    List<string> overlayFiles;
    List<string> overlayNames;
    List<GameObject> objs = new List<GameObject>(); // Module game objects

    Step currentStep;

    public delegate void OverlayCreated();
    public static event OverlayCreated OnOverlayCreated;

    Color modColor;

    public Layout getLayout() {
        return layout;
    }

    // Use this for initialization
    void Start() {
        modColor = new Color(1.0f, 1.0f, 1.0f, 1.0f / 4.0f);

        ProcedureManager_2.OnStepChanged += OnStepChanged;
        Instance.path = Application.streamingAssetsPath + "/OverlayLayouts/";

        PreloadOverlays("/OverlayLayouts/");
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

                    if (preload.activator) {
                        //CreateCalibrator(label);
                    }

                    overlayNames.Add(preload.name);
                    overlayFiles.Add(label);
                }
            }
        } catch (System.Exception ex) {
            Debug.Log("Error: JSON input. " + ex.Message);
        }
    }

    void CreateCalibrator(string overlayName) {
        // Doesnt work for some reason
        GameObject obj = (GameObject)Instantiate(Resources.Load("OverlayCalib"));
        obj.name = overlayName + "_Calibrator";
		OverlayCalibrator calib = obj.GetComponent<OverlayCalibrator>();
		calib.overlayName = overlayName;
    }

    private void BoxMode(GameObject obj) {
        LineRenderer lr = obj.GetComponent<LineRenderer>();
        lr.enabled = true;

        MeshRenderer mr = obj.GetComponent<MeshRenderer>();
        mr.enabled = false;
    }

    private void SolidMode(GameObject obj) {
        LineRenderer lr = obj.GetComponent<LineRenderer>();
        lr.enabled = false;

        MeshRenderer mr = obj.GetComponent<MeshRenderer>();
        mr.enabled = true;
    }

    public void LoadOverlay(string name, Vector3 pos, Quaternion rot) {
        if (layout != null) {
            if (layout.fileName == name) {
                return;
            }
        }

        try
        {
            string temp = path + name;
            if (System.IO.File.Exists(temp))
            {
                string contents = System.IO.File.ReadAllText(temp);

                if (layout != null) {
                    foreach (GameObject obj in objs) {
                        Destroy(obj);
                    }
                    objs.Clear();
                }

                Instance.layout = JsonUtility.FromJson<Layout>(contents);
                CreateTaskboard(pos, rot);
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

    //Create the Taskboard
    void CreateTaskboard(Vector3 pos, Quaternion rot) {
        Vector3 corner = new Vector3(-layout.size.x / 2.0f, layout.size.y / 2.0f, layout.size.z / 2.0f);

        transform.position = pos - rot * (corner + new Vector3(layout.activator_pos.x, layout.activator_pos.y, -layout.activator_pos.z)
                             + layout.activator_size.UnityVec() / 2.0f);
        transform.rotation = rot;

        GameObject cube;
        Material mat = Resources.Load("ModuleMat", typeof(Material)) as Material;

        foreach (Modules m in layout.modules) {
            cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
            cube.transform.parent = transform;
            cube.name = m.id;
            cube.transform.localScale = new Vector3(m.size.x, m.size.y, m.size.z);
            Vector3 halfScle = new Vector3(m.size.x / 2, m.size.y / 2, -m.size.z / 2);
            cube.transform.localPosition = corner + new Vector3(m.position.x, m.position.y, -m.position.z) + halfScle;
            cube.transform.localRotation = Quaternion.Euler(m.rotation.UnityVec());

            cube.GetComponent<Renderer>().material = mat;

            // Create the line renderer
            LineRenderer lr = cube.AddComponent<LineRenderer>();
            Vector3[] pts = new Vector3[6];
            
            pts[0] = new Vector3(0.25f, 0.0f, 0.5f);
            pts[1] = new Vector3(0.5f, 0.0f, 0.5f);
            pts[2] = new Vector3(0.5f, 0.0f, -0.5f);
            pts[3] = new Vector3(-0.5f, 0.0f, -0.5f);
            pts[4] = new Vector3(-0.5f, 0.0f, 0.5f);
            pts[5] = new Vector3(0.25f, 0.0f, 0.5f);

            lr.positionCount = 6;
            lr.SetPositions(pts);
            lr.widthMultiplier = 0.005f;
            lr.enabled = false;
            lr.useWorldSpace = false;
            lr.numCornerVertices = 3;

            BoxMode(cube);
            cube.SetActive(false);

            objs.Add(cube);
        }

        ProcedureManager_2.Instance.LoadProcedure(layout.procedure);
    }

    #region old

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
                CreateTaskboard(Vector3.zero, Quaternion.identity);
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

    #endregion

    #region Prompts

    private void OnStepChanged(Step step) {
        // Remove the prompts from the previous step
        if (currentStep != null)
        {
            foreach (Prompt prompt in currentStep.prompts)
            {
                switch (prompt.type)
                {
                    case "arrow":
                        {
                            Transform module1 = transform.Find(prompt.moduleID);
                            Transform module2 = transform.Find(prompt.misc);

                            if (module1 != null && module2 != null)
                            {
                                module1.gameObject.SetActive(false);
                                module2.gameObject.SetActive(false);
                            }
                            Transform arrow = transform.Find("Arrow " + prompt.moduleID + " to " + prompt.misc);
                            if (arrow != null)
                            {
                                Destroy(arrow.gameObject);
                            }
                            else
                            {
                                Debug.LogError("Step #" + currentStep.number + ": Couldn't find arrow for deletion");
                            }
                            break;
                        }
                    case "push":
                        {
                            Transform arrow = transform.Find("Push " + prompt.moduleID);

                            Transform module = transform.Find(prompt.moduleID);
                            if (module != null)
                            {
                                module.gameObject.SetActive(false);
                            }

                            if (arrow != null)
                            {
                                Destroy(arrow.gameObject);
                            }
                            else
                            {
                                Debug.LogError("Step #" + currentStep.number + ": Couldn't find push arrow for deletion");
                            }
                            break;
                        }
                    case "pull":
                        {
                            Transform arrow = transform.Find("Pull " + prompt.moduleID);
                            
                            Transform module = transform.Find(prompt.moduleID);
                            if (module != null)
                            {
                                module.gameObject.SetActive(false);
                            }

                            if (arrow != null)
                            {
                                Destroy(arrow.gameObject);
                            }
                            else
                            {
                                Debug.LogError("Step #" + currentStep.number + ": Couldn't find pull arrow for deletion");
                            }
                            break;
                        }
                    case "highlight":
                        {
                            Transform module = transform.Find(prompt.moduleID);
                            if (module != null)
                            {
                                module.gameObject.GetComponent<Renderer>().material.color = modColor;
                                module.gameObject.SetActive(false);
                            }
                            else
                            {
                                Debug.LogError("Step #" + currentStep.number + ": Couldn't find module by ID for unhighlighting");
                            }
                            break;
                        }
                    case "circle":
                        {
                            Transform circle = transform.Find("Circle " + prompt.moduleID);
                            Transform module = transform.Find(prompt.moduleID);

                            if (module != null)
                            {
                                module.gameObject.SetActive(false);
                            }

                            if (circle != null)
                            {
                                Destroy(circle.gameObject);
                            }
                            else
                            {
                                Debug.LogError("Step #" + step.number + ": Couldn't find circle for deletion");
                            }
                            break;
                        }
                }
            }
        }

        // Create the prompts for the current step
        foreach (Prompt prompt in step.prompts)
        {
            switch (prompt.type)
            {
                case "arrow":
                    {
                        Transform module1 = transform.Find(prompt.moduleID);
                        Transform module2 = transform.Find(prompt.misc);

                        if (module1 != null && module2 != null)
                        {
                            module1.gameObject.SetActive(true);
                            module2.gameObject.SetActive(true);
                            GameObject arrowObj = new GameObject();
                            arrowObj.name = "Arrow " + prompt.moduleID + " to " + prompt.misc;
                            arrowObj.transform.SetParent(transform);
                            Arrow arrow = arrowObj.AddComponent<Arrow>();
                            arrow.beg = module1.position;
                            arrow.end = module2.position;
                            arrow.transform.position = arrow.transform.position + new Vector3(0.0f, 0.01f, 0.0f);
                        }
                        else
                        {
                            Debug.LogError("Step #" + step.number + ": Couldn't find module by ID");
                        }
                        break;
                    }

                case "push":
                    {
                        Transform module = transform.Find(prompt.moduleID);

                        if (module != null)
                        {
                            module.gameObject.SetActive(true);
                            GameObject arrowObj = new GameObject();
                            arrowObj.name = "Push " + prompt.moduleID;
                            arrowObj.transform.SetParent(transform);
                            arrowObj.transform.position = module.position;
                            PushArrow arrow = arrowObj.AddComponent<PushArrow>();
                            arrow.end = 0.01f;
                            arrow.beg = 0.05f;
                            arrow.transform.position = arrow.transform.position + new Vector3(0.0f, 0.01f, 0.0f);
                        }
                        else
                        {
                            Debug.LogError("Step #" + step.number + ": Couldn't find module by ID");
                        }
                        break;
                    }
                case "pull":
                    {
                        Transform module = transform.Find(prompt.moduleID);

                        if (module != null)
                        {
                            module.gameObject.SetActive(true);
                            GameObject arrowObj = new GameObject();
                            arrowObj.name = "Pull " + prompt.moduleID;
                            arrowObj.transform.SetParent(transform);
                            arrowObj.transform.position = module.position;
                            PushArrow arrow = arrowObj.AddComponent<PushArrow>();
                            arrow.end = 0.05f;
                            arrow.beg = 0.01f;
                            arrow.transform.position = arrow.transform.position + new Vector3(0.0f, 0.01f, 0.0f);
                        }
                        else
                        {
                            Debug.LogError("Step #" + step.number + ": Couldn't find module by ID");
                        }
                        break;
                    }
                case "highlight":
                    {
                        Transform module = transform.Find(prompt.moduleID);
                        if (module != null)
                        {
                            module.gameObject.SetActive(true);
                            Color propCol = Color.green;
                            propCol.a = 1.0f / 4.0f;
                            module.gameObject.GetComponent<Renderer>().material.color = propCol;
                        }
                        else
                        {
                            Debug.LogError("Step #" + step.number + ": Couldn't find module by ID");
                        }
                        break;
                    }
                case "circle":
                    {
                        Transform module = transform.Find(prompt.moduleID);

                        if (module != null)
                        {
                            module.gameObject.SetActive(true);
                            float dirMod = 1.0f;
                            if (prompt.misc == "clockwise") dirMod = 1.0f;
                            if (prompt.misc == "counterclockwise") dirMod = -1.0f;

                            GameObject circleObj = new GameObject();
                            circleObj.name = "Circle " + prompt.moduleID;
                            circleObj.transform.SetParent(transform);
                            circleObj.AddComponent<Circle>().speed = dirMod * 45.0f;

                            Mesh mesh = Resources.Load("Circle", typeof(Mesh)) as Mesh;
                            Material mat = Resources.Load("PromptMat", typeof(Material)) as Material;
                            circleObj.AddComponent<MeshRenderer>().material = mat;
                            circleObj.AddComponent<MeshFilter>().mesh = mesh;


                            // Find the smallest dimension of the module
                            float smallestDim = module.localScale.x;
                            if (module.localScale.z < smallestDim) smallestDim = module.localScale.z;


                            circleObj.transform.localScale = new Vector3(dirMod * smallestDim, smallestDim, smallestDim);
                            circleObj.transform.position = module.position + new Vector3(0.0f, 0.01f, 0.0f);
                        }
                        else
                        {
                            Debug.LogError("Step #" + step.number + ": Couldn't find module by ID");
                        }
                        break;
                    }
            }
        }
        currentStep = step;
    }

    #endregion
}

//TASKBOARD LAYOUT
[System.Serializable]
public class Layout
{
    public string name;

    public bool activator;
    public Vec3 activator_pos;
    public Vec3 activator_size;
    public int activator_target;

    public string procedure;
    
    public Vec3 panel_pos;
    public Vec3 panel_rot;
    public string fileName;

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
    public float x;
    public float y;
    public float z;

    public Vector3 UnityVec() {
        return new Vector3(x, y, -z);
    }
}

[System.Serializable]
public class OverlayPreload { // Contains basic information about an overlay for preloading
    public string name;
    public bool activator;
    public Vec3 activator_pos;
    public Vec3 activator_size;
    public int activator_target;

    public string procedure;
}