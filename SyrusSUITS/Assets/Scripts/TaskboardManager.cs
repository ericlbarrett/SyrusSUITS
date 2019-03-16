using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TaskboardManager : MonoBehaviour
{

    Step currentStep;

    // Use this for initialization
    void Start()
    {
        ProcedureManager_2.OnStepChanged += OnStepChanged;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void OnStepChanged(Step step)
    {
        if (step != null)
        {
            Debug.Log(step);
            currentStep = step;
        }
    }

}
