// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.UI;

// public class ProcedureManager : MonoBehaviour {

//     public delegate void StepEvent(Step step);
//     public static event StepEvent OnStepChanged;

//     string path;    // Directory of where the JSON files are located
//     List<string> procedures;                // List of procedure system path
//     List<string> procedureName;             // List of procedure names
//     Procedure procedure = new Procedure();  // Procedure Class holds the information of the steps

//     // Procedure Panel components
//     public GameObject procedurePanel;       // Procedure Panel
//     public Text procedureWindowText;        // Drop down text
//     public Text procedurBarText;            // Current task text
//     public GameObject ProcedureListWindow;  // Drop down object

//     // Load Procedure
//     // OptionsMenu stepMenu;               // Step menu
//     bool loaded = false;                // If the choose procedure options has been loaded.

//     int currentProcedureNum = 0; // The current procedure number
//     int stepNumber = 0;

//     private static ProcedureManager _Instance;
//     public static ProcedureManager Instance {
//         get {
//             if (_Instance == null) {
//                 _Instance = FindObjectOfType<ProcedureManager>();
//             }
//             return _Instance;
//         }
//     }

//     void LoadFileNames(string dir) {
//         procedureName = new List<string>();
//         procedures = new List<string>();
//         string location = Application.streamingAssetsPath;
//         try
//         {
//             string temp = location + dir;
//             foreach (string file in System.IO.Directory.GetFiles(temp))
//             {
//                 string label = file.Replace(temp, ""); // file (e.x. = procedure1.JSON)
//                 if (!label.Contains(".meta"))
//                 {
//                     string contents = System.IO.File.ReadAllText(temp + label);
//                     ProcedureNames procedureNames = JsonUtility.FromJson<ProcedureNames>(contents);
//                     //Debug.Log(procedureNames.title);
//                     procedureName.Add(procedureNames.title);
//                     procedures.Add(label);
//                 }
//             }
//         }
//         catch (System.Exception ex)
//         {
//             Debug.Log("Error: JSON input. " + ex.Message);
//         }
//     }

//     // Use this for initialization
//     void Start() {
//         path = Application.streamingAssetsPath + "/Procedures/";
//         LoadFileNames("/Procedures/");

//         //Old code for pushing to a file

//         //procedure p = new procedure();
//         //p.title = "Shut down Power";
//         //step s  = new step();
//         //s.id = "e_button";
//         //s.action = "push_down";
//         //s.text = "Push EMERGENCY button DOWN!";
//         //s.number = "1";
//         //p.steps.Add(s);
//         //p.steps.Add(s);
//         //string content = JsonUtility.ToJson(p, true);
//         //System.IO.File.WriteAllText(path, content);

//     }

//     // Update is called once per frame
//     void Update () {
//         // if (changed) {
//         //     OnStepChanged(Instance.procedure.steps[Instance.stepNumber]);
//         //     changed = false;
//         // }
//         // if(TaskboardManager.Instance.loaded && !loaded)
//         // {
//         //     // NEED TO DO!!!
//         //     //
//         //     // Make this into an event
//         //     //
//         //     loaded = true;
//         // }
//     }

//     public void ChooseProcedure() {
//         // OptionsMenu opts = OptionsMenu.Instance("Choose A Procedure", true);
//         // opts.OnSelection += LoadProcedure;
//         // if (procedureName.Count > 0) {
//         //     for (int i = 0; i < procedureName.Count; i++) {
//         //         opts.AddItem(procedureName[i], i);
//         //     }
//         // } else {
//         //     Debug.Log("Error: No procedures currently loaded in Procedure Manager Script");
//         // }
//     }

//     //Sets all the steps and subtext to the dropdown list
//     void LoadStepWindow(List<Step> list) {
//         // stepMenu = OptionsMenu.Instance(procedureName[currentProcedureNum], false);
//         // stepMenu.gameObject.SetActive(false);
//         // stepMenu.OnSelection += (int i) => {
//         //     SetStep(i);
//         //     stepMenu.gameObject.SetActive(false);
//         // };
//         // if (list.Count > 0) {
//         //     int i = 0;
//         //     foreach (Step s in list)
//         //     {
//         //         stepMenu.AddItem(s.number + ".) " + s.text, i);
//         //     }
//         // } else {
//         //     Debug.Log("Error: No steps currently loaded in Procedure Manager Script");
//         // }
//         // changeBarTxt();
//     }

    // //Loads in the procedure that should be completed
    // public void LoadProcedure(int procedureNumber) {
    //     currentProcedureNum = procedureNumber;
    //     try
    //     {
    //         //Destroy(options);
    //         string temp = path + procedures[procedureNumber];
    //         if (System.IO.File.Exists(temp))
    //         {
    //             string contents = System.IO.File.ReadAllText(temp);
    //             //Debug.Log(contents);
    //             procedure = JsonUtility.FromJson<Procedure>(contents);
    //             OnStepChanged(procedure.steps[stepNumber]);
    //             //Debug.Log(procedure.steps[1].text);
    //             LoadStepWindow(procedure.steps);
    //             ToggleProcedurePanel();
    //         }
    //         //if the procedures are loaded. Then render the procedure bar
    //         else
    //         {
    //             Debug.Log("Error: Unable to read " + procedures[procedureNumber] + " file, at " + temp);
    //         }
    //     }
    //     catch (System.Exception ex)
    //     {
    //         Debug.Log("Error: " + procedures[procedureNumber] + " JSON input. " + ex.Message);
    //     }
    // }

//     //Toggles the procedure panel active state
//     public void ToggleProcedurePanel() {
//         procedurePanel.SetActive(!procedurePanel.activeInHierarchy);
//     }

//     //Choose which state to be active or !active
//     public void ToggleProcedurePanel(bool x) {
//         procedurePanel.SetActive(x);
//     }

//     // Activates / Deactivates the procedure dropdown list
//     public void ToggleProcedureList() {
//         // stepMenu.gameObject.SetActive(!stepMenu.gameObject.activeInHierarchy);
//     }

//     //Change the current task information
//     void changeBarTxt() {
//         procedurBarText.alignment = TextAnchor.UpperLeft;
//         procedurBarText.text = procedure.steps[stepNumber].number + ".) " + procedure.steps[stepNumber].text + "\n";
//         if (procedure.steps[stepNumber].subtext.Length > 0)
//         {
//             procedurBarText.text += CreateSubText(procedure.steps[stepNumber]) + "\n";
//         }

//     }

//     // Manually  override the step and jump to a new one
//     public static void SetStep(int newStepNumber) {
//         if (newStepNumber >= 0 && newStepNumber <= Instance.procedure.steps.Count - 1)
//         {
//             Instance.stepNumber = newStepNumber;
//             OnStepChanged(Instance.procedure.steps[Instance.stepNumber]);
//             Instance.changeBarTxt();
//         }
//     }

//     public static Step GetStep() {
//         return Instance.procedure.steps[Instance.stepNumber];
//     }
    
//     // Goes forward to the next step
//     public void NextStep() {
//         Debug.Log("next Step");
//         Instance.stepNumber++;
//         if (Instance.stepNumber > Instance.procedure.steps.Count -1)
//         {
//             //Do not let them go into the negatives
//             Instance.stepNumber = Instance.procedure.steps.Count - 1;
//         } else OnStepChanged(Instance.procedure.steps[Instance.stepNumber]);
//         Instance.changeBarTxt();
//     }

//     //Goes back to the previous step
//     public void PreviousStep() {
//         Debug.Log("Previous Step");
//         Instance.stepNumber--;
//         if(Instance.stepNumber  < 0)
//         {
//             //Do not let them go into the negatives
//             Instance.stepNumber = 0;
//         } else OnStepChanged(Instance.procedure.steps[Instance.stepNumber]);
//         Instance.changeBarTxt();

//     }
    
//     //Returns a string with the color warning/caution and sub text of current step
//     string CreateSubText(Step s) {
//         string temp = "";
//         string firstWord = s.subtext.Split(' ')[0];
//         if (firstWord == "CAUTION:")
//         {
//             temp += "<color=red>CAUTION: </color>";
//         }
//         else if (firstWord == "WARNING:")
//         {
//             temp += "<color=red>WARNING: </color>";
//         }
//         int i = s.subtext.IndexOf(":");
//         if (i + 2 < s.subtext.Length)
//         {
//             i += 2;
//         }
//         temp += s.subtext.Substring(i, s.subtext.Length - i);
//         return temp;
//     }
// }

// //PROCEDURES
// [System.Serializable]
// public class Procedure
// {
//     public string title; //Name of procedure
//     public List<Step> steps = new List<Step>();
// }

// [System.Serializable]
// public class Step
// {
//     public string text;     // Text to display
//     public string subtext;  // Sub Text with CAUTION || DANGER
//     public string number;   // Number of which step this is
//     public List<Prompt> prompts = new List<Prompt>();
// }

// [System.Serializable]
// public class Prompt {
//     public string type;
//     public string moduleID;
//     public string misc;
//     public float rotation;
// }

// // Query the Procedure names out from the JSON
// [System.Serializable]
// public class ProcedureNames
// {
//     public string title;
// }
