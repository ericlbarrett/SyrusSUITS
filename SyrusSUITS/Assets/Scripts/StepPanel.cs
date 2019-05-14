using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StepPanel : MonoBehaviour {

	public Text previousStep; // Procedure Panel Text
    public Text currentStep; // Procedure Panel Text
    public Text nextStep; // Procedure Panel Text

	// Use this for initialization
	void Start () {
		ProcedureManager.OnStepChanged += StepChanged;
		updateText();
	}
	
	void StepChanged(Step step) {
		updateText();
	}

	public void readStepTTS() {
        string t = ProcedureManager.Instance.getCurrentStep().text;
        if (t != null) HoloToolkit.Unity.TextToSpeech.AetherSpeech(t);
    }

	//Change the current task information
    void updateText() {
        // Current Step
		Step current = ProcedureManager.Instance.getCurrentStep();

        currentStep.alignment = TextAnchor.UpperLeft;
        currentStep.text = current.number + ".) " + current.text + "\n";
        if (current.subtext.Length > 0) {
            currentStep.text += CreateSubText(current) + "\n";
        }

        // Next Step
		Step next = ProcedureManager.Instance.getNextStep();

        if (next != null) {
            nextStep.alignment = TextAnchor.UpperLeft;
            nextStep.text = next.number + ".) " + next.text + "\n";
            if (next.subtext.Length > 0) {
                nextStep.text += CreateSubText(next) + "\n";
            }
        } else {
            nextStep.text = "";
        }

        // Previous Step
		Step previous = ProcedureManager.Instance.getPreviousStep();

        if (previous != null) {
			previousStep.alignment = TextAnchor.UpperLeft;
            previousStep.text = previous.number + ".) " + previous.text + "\n";
            if (previous.subtext.Length > 0) {
                previousStep.text += CreateSubText(previous) + "\n";
            }
        } else {
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
