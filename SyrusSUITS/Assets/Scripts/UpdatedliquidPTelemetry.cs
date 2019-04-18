using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class UpdatedliquidPTelemetry : MonoBehaviour {

    public Text pres;                      // Reference to the text component.
    public UpdateTelemetry ut;
    
    // Use this for initialization
    void Start()
    {
        pres = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        pres.text = GameObject.Find("TelemetryManager").GetComponent<UpdateTelemetry>().numericalData.p_h2o_l.ToString() + " psia";
    }
}
