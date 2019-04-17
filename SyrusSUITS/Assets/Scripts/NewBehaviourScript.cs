using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class NewBehaviourScript : MonoBehaviour {

    public int test = 0;
    public Text pres;
    public static int score;        // The player's score.
    public Text text;                      // Reference to the text component.
    NumericalTelemetry nt;


    // Use this for initialization
    void Start()
    {
        pres = GetComponent<Text>();
        nt = new NumericalTelemetry();
        for (int i = 14; i < 16; i++)
        {
            nt.p_h2o_l = i;
            text.text = nt.p_h2o_l.ToString() + " psia";
        }
        StartCoroutine(Increment());
    }



    IEnumerator Increment()
    {
        for (int i = 14; i < 16; i++)
        {
            test++;
            pres.text = test.ToString() + " psia";
            yield return new WaitForSeconds(10);
        }
    }

    // Update is called once per frame
    void Update() {

    }
    
        
    void Awake() 
        {
            //Set up the reference;
            text = GetComponent <Text>();

            // Reset the score
            score = 0;
        
    }
}

