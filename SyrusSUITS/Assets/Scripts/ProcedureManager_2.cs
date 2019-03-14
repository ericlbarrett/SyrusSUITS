using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ProcedureManager_2 : MonoBehaviour {
	public delegate void StepEvent(Step step);
    public static event StepEvent OnStepChanged;
	public GameObject optionsPrefab;
	public GameObject textPrefab;
	public GameObject buttonPrefab;
	
	int currentProcedureNum = 0; // The current procedure number
	int stepNumber = 0;
	string path;    // Directory of where the procedure JSON files are located
    List<string> proceduresPath;                // List of procedure system path (e.x. something.json)
    List<string> procedureName;             // List of procedure names (e.x. title from JSON)
    Procedure procedure = new Procedure();  // Procedure Class holds the information of the steps

	void Start () {
		path = Application.streamingAssetsPath + "/Procedures/";
		LoadFileNames("/Procedures/");
		function();
		ChooseProcedure();
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	void function() {
		for(int i = 0; i < proceduresPath.Count; i++) {
			Debug.Log(proceduresPath[i]);
		}
	}
	
	public void ChooseProcedure() {
        OptionsMenu opts = OptionsMenu.Instance("Choose A Procedure", true);
        opts.OnSelection += LoadProcedure;
        if (procedureName.Count > 0) {
            for (int i = 0; i < procedureName.Count; i++) {
                opts.AddItem(procedureName[i], i);
            }
        } else {
            Debug.Log("Error: No procedures currently loaded in Procedure Manager Script");
        }
    }
	    //Loads in the procedure that should be completed
    public void LoadProcedure(int procedureNumber) {
        currentProcedureNum = procedureNumber;
        try
        {
            //Destroy(options);
            string temp = path + proceduresPath[procedureNumber];
            if (System.IO.File.Exists(temp))
            {
                string contents = System.IO.File.ReadAllText(temp);
                //Debug.Log(contents);
                procedure = JsonUtility.FromJson<Procedure>(contents);
                OnStepChanged(procedure.steps[stepNumber]);
                //Debug.Log(procedure.steps[1].text);
                // LoadStepWindow(procedure.steps);
                // ToggleProcedurePanel();
            }
            //if the procedures are loaded. Then render the procedure bar
            else
            {
                Debug.Log("Error: Unable to read " + proceduresPath[procedureNumber] + " file, at " + temp);
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error: " + proceduresPath[procedureNumber] + " JSON input. " + ex.Message);
        }
    }
	void LoadFileNames(string dir) {
        procedureName = new List<string>();
        proceduresPath = new List<string>();
        string location = Application.streamingAssetsPath;
        try
        {
            string temp = location + dir;
            foreach (string file in System.IO.Directory.GetFiles(temp))
            {
                string label = file.Replace(temp, ""); // file (e.x. = procedure1.JSON)
				if( label.EndsWith(".json"))
                {
                    string contents = System.IO.File.ReadAllText(temp + label);
                    ProcedureNames procedureNames = JsonUtility.FromJson<ProcedureNames>(contents);
                    // Debug.Log(procedureNames.title);
                    procedureName.Add(procedureNames.title);
                    proceduresPath.Add(label);
                }
            }
        }
        catch (System.Exception ex)
        {
            Debug.Log("Error: JSON input. " + ex.Message);
        }
    }
}


//PROCEDURES
[System.Serializable]
public class Procedure
{
    public string title; //Name of procedure
    public List<Step> steps = new List<Step>();
}

[System.Serializable]
public class Step
{
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

// Query the Procedure names out from the JSON
[System.Serializable]
public class ProcedureNames
{
    public string title;
}

