using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProcedureManager : MonoBehaviour {

    public GameObject placingPanel;

    public delegate void StepEvent(Step step);
    public static event StepEvent OnStepChanged;

    private static ProcedureManager _Instance;
    public static ProcedureManager Instance
    {
        get
        {
            if (_Instance == null)
            {
                _Instance = FindObjectOfType<ProcedureManager>();
            }
            return _Instance;
        }
    }

	string path;                    // Directory where the procedure JSON files are located

    List<string> procedureFiles;    // List of procedure file names
    List<string> procedureNames;    // List of procedure names

    Procedure procedure = null;     // The currently loaded procedure
    //Step currentStep = null;      // The current step

    int stepIndex;                  // Index of the current step

    public GameObject optionsPrefab;
	public GameObject textPrefab;
	public GameObject buttonPrefab;

    //This is for the Procedure Panel V2
    public GameObject procedurePanel;       // Procedure Panel
    public Text previousStep; // Procedure Panel Text
    public Text currentStep; // Procedure Panel Text
    public Text nextStep; // Procedure Panel Text

    float timer;

    void Start () {

        LeapManager.OnGestureSwipe += SwipeGesture;
        timer = 0;

        path = Application.streamingAssetsPath + "/Procedures/";
		PreloadProcedures();
	}
	
    // Loads the all the JSON files from the Streaming Assets
    void PreloadProcedures() {
        procedureFiles = new List<string>();
        procedureNames = new List<string>();

        try {
            foreach (string file in System.IO.Directory.GetFiles(path)) {
                string label = file.Replace(path, "");
				if(label.EndsWith(".json")) {
                    string contents = System.IO.File.ReadAllText(path + label);
                    ProcedurePreload preload = JsonUtility.FromJson<ProcedurePreload>(contents);

                    procedureNames.Add(preload.title);
                    procedureFiles.Add(label);
                }
            }
        }
        catch (System.Exception ex) {
            Debug.Log("Error: JSON input. " + ex.Message);
        }
    }

    public void LoadProcedure(string fileName) {
        try {
            string dir = path + fileName;
            if (System.IO.File.Exists(dir))
            {
                string contents = System.IO.File.ReadAllText(dir);
                procedure = JsonUtility.FromJson<Procedure>(contents);

                // Set the index for each step
                for (int i = 0; i < procedure.steps.Count; i++) {
                    procedure.steps[i].index = i;
                }

                // Trigger step changed event
                stepIndex = 0;
                OnStepChanged(procedure.steps[stepIndex]);

                // Enable the procedure panel
                ToggleProcedurePanel(true);


                Transform tr = OverlayManager.Instance.transform;
                Layout ly = OverlayManager.Instance.getLayout();
                procedurePanel.transform.rotation = Quaternion.LookRotation(tr.forward, Vector3.up) * Quaternion.Euler(ly.panel_rot.x, ly.panel_rot.y, ly.panel_rot.z);
                procedurePanel.transform.position = tr.position + tr.rotation * new Vector3(ly.panel_pos.x, ly.panel_pos.y, ly.panel_pos.z);
                changeBarTxt(); // Needs to be here to update to the first Item
            } else {
                Debug.Log("Error: Unable to read " + fileName + " file, at " + dir);
            }
        } catch (System.Exception ex) {
            Debug.Log("Error: " + fileName + " JSON input. " + ex.Message);
        }
    }

    public void LoadProcedure(int procedureNumber) {
        try {
            string dir = path + procedureFiles[procedureNumber];
            if (System.IO.File.Exists(dir)) {
                string contents = System.IO.File.ReadAllText(dir);
                procedure = JsonUtility.FromJson<Procedure>(contents);

                // Set the index for each step
                for (int i = 0; i < procedure.steps.Count; i++) {
                    procedure.steps[i].index = i;
                }

                // Trigger step changed event
                stepIndex = 0;
                OnStepChanged(procedure.steps[stepIndex]);

                // Enable the procedure panel
                ToggleProcedurePanel(true);

                changeBarTxt(); // Needs to be here to update to the first Item
            } else {
                Debug.Log("Error: Unable to read " + procedureFiles[procedureNumber] + " file, at " + dir);
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error: " + procedureFiles[procedureNumber] + " JSON input. " + ex.Message);
        }
    }

    // Goes forward to the next step
    public void NextStep() {
        if (procedure != null) {
            if (stepIndex < procedure.steps.Count - 1) {
                OnStepChanged(procedure.steps[stepIndex]);
            }
        }
    }

    // Goes back to the previous step
    public void PreviousStep() {
        if (procedure != null) {
            if (stepIndex < procedure.steps.Count - 1) {
                OnStepChanged(procedure.steps[stepIndex]);
            }
        }
    }

    // Manually override the step and jump to a new one
    public void SetStep(int newStepIndex) {
        if (newStepIndex >= 0 && newStepIndex <= procedure.steps.Count - 1) {
            stepIndex = newStepIndex;
            OnStepChanged(procedure.steps[stepIndex]);
        }
    }

    // Opens a menu to select the procedure from
	public void ChooseProcedure() {
        OptionsMenu opts = OptionsMenu.Instance("Choose A Procedure", true);
        opts.OnSelection += LoadProcedure;
        if (procedureNames.Count > 0) {
            for (int i = 0; i < procedureNames.Count; i++) {
                opts.AddItem(procedureNames[i], i);
            }
            opts.ResizeOptions();
        } else {
            Debug.Log("Error: No procedures currently loaded in Procedure Manager Script");
        }
    }


// ---------------------------------------


    void SwipeGesture(Vector3 pos, Vector3 dir) {
        if (Vector3.Angle(Vector3.left, dir) < 40.0f) {
            if (timer <= 0) {
                timer = 1.0f;
                NextStep();
            }
        } else if (Vector3.Angle(Vector3.right, dir) < 40.0f) {
            if (timer <= 0) {
                timer = 1.0f;
                PreviousStep();
            }
        }

    }

	// Update is called once per frame
	void Update () {
		if (timer > 0) timer -= Time.deltaTime;
	}
    public void stepSpeak()
    {
        string t = procedure.steps[stepIndex].text;
        if (t != null) HoloToolkit.Unity.TextToSpeech.AetherSpeech(t);
    }

    //Toggles the procedure panel active state
    public void ToggleProcedurePanel(bool val)
    {
        procedurePanel.SetActive(val);
    }

    //Change the current task information
    void changeBarTxt()
    {
        //procedurBarText.alignment = TextAnchor.UpperLeft;
        //procedurBarText.text = procedure.steps[stepIndex].number + ".) " + procedure.steps[stepIndex].text + "\n";
        //if (procedure.steps[stepIndex].subtext.Length > 0)
        //{
        //    procedurBarText.text += CreateSubText(procedure.steps[stepIndex]) + "\n";
        //}

        //current
        currentStep.alignment = TextAnchor.UpperLeft;
        currentStep.text = procedure.steps[stepIndex].number + ".) " + procedure.steps[stepIndex].text + "\n";
        if (procedure.steps[stepIndex].subtext.Length > 0)
        {
            currentStep.text += CreateSubText(procedure.steps[stepIndex]) + "\n";
        }

        //next
        if (Instance.stepIndex + 1 <= Instance.procedure.steps.Count - 1)
        {
            nextStep.text = procedure.steps[stepIndex + 1].number + ".) " + procedure.steps[stepIndex + 1].text + "\n";
            nextStep.alignment = TextAnchor.UpperLeft;
            nextStep.text = procedure.steps[stepIndex + 1].number + ".) " + procedure.steps[stepIndex + 1].text + "\n";
            if (procedure.steps[stepIndex + 1].subtext.Length > 0)
            {
                nextStep.text += CreateSubText(procedure.steps[stepIndex + 1]) + "\n";
            }
        }
        else
        {
            nextStep.text = "";
        }
        //previous
        if (Instance.stepIndex - 1 >= 0)
        {
            //previousStep.text = procedure.steps[stepIndex - 1].number + ".) " + procedure.steps[stepIndex - 1].text + "\n";
            previousStep.alignment = TextAnchor.UpperLeft;
            previousStep.text = procedure.steps[stepIndex - 1].number + ".) " + procedure.steps[stepIndex - 1].text + "\n";
            if (procedure.steps[stepIndex - 1].subtext.Length > 0)
            {
                previousStep.text += CreateSubText(procedure.steps[stepIndex - 1]) + "\n";
            }
        }
        else
        {
            previousStep.text = "";
        }



    }

    //Returns a string with the color warning/caution and sub text of current step
    string CreateSubText(Step s)
    {
        string temp = "";
        string firstWord = s.subtext.Split(' ')[0];
        if (firstWord == "CAUTION:")
        {
            temp += "<color=red>CAUTION: </color>";
        }
        else if (firstWord == "WARNING:")
        {
            temp += "<color=red>WARNING: </color>";
        }
        int i = s.subtext.IndexOf(":");
        if (i + 2 < s.subtext.Length)
        {
            i += 2;
        }
        temp += s.subtext.Substring(i, s.subtext.Length - i);
        return temp;
    }
}

//PROCEDURES
[System.Serializable]
public class Procedure {
    public string title; //Name of procedure
    public List<Step> steps = new List<Step>();
}

[System.Serializable]
public class Step {

    public int index; // Index from the list, not read from JSON

    public string text;     // Text to display
    public string subtext;  // Sub Text with CAUTION || DANGER
    public string number;   // Number of which step this is
    public List<Prompt> prompts = new List<Prompt>();
}

[System.Serializable]
public class Prompt {
    public string type;
    public string moduleID;
    public string misc;
    public float rotation;
}

[System.Serializable]
public class ProcedurePreload {
    public string title;
}

