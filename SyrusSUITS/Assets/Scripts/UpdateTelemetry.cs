using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class NumericalTelemetry {
    public int heart_bpm;       //Heart beats per minute
    public float p_sub;         //Sub pressure
    public float p_suit;        //Suit pressure
    public float t_sub;         //Sub temperature
    public int v_fan;           //Fan tachometer
    public float p_o2;          //Oxygen pressure
    public float rate_o2;       //Oxygen rate
    public float cap_battery;   //Batter capacity
    public float p_h2o_g;       //H2O gas pressure
    public float p_h2o_l;       //H2O liquid pressure
    public float p_sop;         //Secondary oxygen pack pressure
    public float rate_sop;      //Oxygen rate for secondary pack
    public string t_battery;    //Battery time remaining
    public string t_oxygen;     //Oxygen time remaining
    public string t_water;      //Water time remaining
}

[System.Serializable]
public class SwitchTelemetry {
    public bool sop_on;         //Secondary oxygen pack is active
    public bool sspe;           //Spacesuit pressure emergency
    public bool fan_error;      //Cooling fan failure
    public bool vent_error;     //No vent flow
    public bool vehicle_power;  //Receiving power through spacecraft
    public bool h2o_off;        //H2O system is offline
    public bool o2_off;         //O2 system is offline

    //Based off deviation
    public bool battery_amp_high;       //>4 amp
	public bool battery_vdc_low;        //<15 V
	public bool suit_pressure_low;      //<2
	public bool spacesuit_pressure_high;//>5 psid
	public bool o2_high_use;            //1 >psi/min
	public bool sop_pressure_low;       //<700 psia
	public bool co2_high;               //>500 ppm

    public SwitchTelemetry(){
        this.battery_amp_high = false;
        this.battery_vdc_low = false;
        this.suit_pressure_low = false;
        this.spacesuit_pressure_high = false;
        this.o2_high_use = false;
        this.sop_pressure_low = false;
        this.co2_high = false;
    }
}

//Numerical Data Points
public class SuitDataPoints {
    public string name;
    public object value;
    public string unit;
    public int status;

    public SuitDataPoints(string name, object value, string unit, float min, float max) {
        this.name = name;
        this.value = value;
        this.unit = unit;

        if(this.name == "Time Life Battery" || this.name == "Time Life Oxygen" || this.name == "Time Life Water")
        {
            string strSec = value.ToString();
            float seconds = (float)TimeSpan.Parse(strSec).TotalSeconds;

            if(seconds > max || seconds < min)
            {
                this.status = 3;
            }
            else
            {
                this.status = 1;
            }
        }

        float yellowPercent = 15f / 100f;
        float delta = max - min;
        float plusMinus = delta * yellowPercent;
        float yellowMin = min + yellowPercent;
        float yellowMax = max - plusMinus;

        float num = Convert.ToSingle(value);

        if (num > max || num < min)
        {
            this.status = 3;    //Red
        }
        else if ((num > min && num < yellowMin) || (num > yellowMax && num < max))
        {
            this.status = 2;    //Yellow
        }
        else
        {
            this.status = 1;    //Green
        }
    }
}

//Switch Data Points
public class SwitchDataPoints {
    public string name;
    public object status;

    public SwitchDataPoints(string name, object status) {
        this.name = name;
        this.status = status;
    }
}

public class UpdateTelemetry : MonoBehaviour
{
    public NumericalTelemetry numericalData;
    public SwitchTelemetry switchData;
    public List<NumericalTelemetry> tDataS;

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetNumerical());
        StartCoroutine(GetSwitch());
    }

    // Update is called once per frame
    void Update()
    {
    }

    IEnumerator GetNumerical()
    {
        while (true)
        {
            UnityWebRequest www = UnityWebRequest.Get("https://skylab-program.herokuapp.com/api/suit/recent");
            yield return www.SendWebRequest();

            if (www.isNetworkError || www.isHttpError)
            {
                Debug.Log(www.error);
            }
            else
            {
                string jsonString = www.downloadHandler.text;
                jsonString = jsonString.Replace('[', ' ').Replace(']', ' ');
                numericalData = JsonUtility.FromJson<NumericalTelemetry>(jsonString);

                if(numericalData.t_battery[0] == '-')
                {
                    numericalData.t_battery = "00:00:00";
                }

                if(numericalData.t_oxygen[0] == '-')
                {
                    numericalData.t_oxygen = "00:00:00";
                }

                if(numericalData.t_water[0] == '-')
                {
                    numericalData.t_water = "00:00:00";
                }

                int numAr = 12; //Number of components

                SuitDataPoints[] suitDataPoints = new SuitDataPoints[numAr];

                setSuitData(numericalData, suitDataPoints);

                tDataS.Add(numericalData);
                stdDev(tDataS, numAr);
            }
            yield return new WaitForSeconds(10);
        }
    }

    //Sets Suit Telemetry
    public void setSuitData(NumericalTelemetry data, SuitDataPoints[] dataPoints) {
        dataPoints[0] = new SuitDataPoints("Heart Beats Per Minute", data.heart_bpm, "bpm", 85f, 90f);
        dataPoints[1] = new SuitDataPoints("Sub Pressure", data.p_sub, "psia", 2.0f, 4.0f);
        dataPoints[2] = new SuitDataPoints("Internal Suit Pressure", data.p_suit, "psid", 2.0f, 4.0f);
        dataPoints[3] = new SuitDataPoints("Sub Temperature", data.t_sub, "\u00B0F", 4f, 6f);
        dataPoints[4] = new SuitDataPoints("Fan Tachometer", data.v_fan, "RPM", 10000f, 40000f);
        dataPoints[5] = new SuitDataPoints("Oxygen Pressure", data.p_o2, "psia", 750f, 950f);
        dataPoints[6] = new SuitDataPoints("Oxygen Rate", data.rate_o2, "psi/min", 0.5f, 1.0f);
        dataPoints[7] = new SuitDataPoints("Battery Capacity", data.cap_battery, "amp-hr", 0f, 30f);
        dataPoints[8] = new SuitDataPoints("H2O Gas Pressure", data.p_h2o_g, "psia", 14f, 16f);
        dataPoints[9] = new SuitDataPoints("H2O Liquid Pressure", data.p_h2o_l, "psia", 14f, 16f);
        dataPoints[10] = new SuitDataPoints("Secondary Oxygen Pack Pressure", data.p_sop, "psia", 750f, 950f);
        dataPoints[11] = new SuitDataPoints("Secondary Oxygen Pack Flow Rate", data.rate_sop, "psi/min", 0.5f, 1.0f);
    }

    IEnumerator GetSwitch() {
        while (true) {
            UnityWebRequest www = UnityWebRequest.Get("http://skylab-program.herokuapp.com/api/suitswitch/recent");    //Placeholder
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            } else {
                string jsonString = www.downloadHandler.text;
                jsonString = jsonString.Replace('[', ' ').Replace(']', ' ');

                switchData = JsonUtility.FromJson<SwitchTelemetry>(jsonString);
                int numSwitch = 14;
                SwitchDataPoints[] dataPoints = new SwitchDataPoints[numSwitch];

                setSwitchData(switchData, dataPoints);
            }
            yield return new WaitForSeconds(10);
        }
        
    }

    public void setSwitchData(SwitchTelemetry data, SwitchDataPoints[] dataPoints)
    {
        dataPoints[0] = new SwitchDataPoints("SOP On", data.sop_on);
        dataPoints[1] = new SwitchDataPoints("Spacesuit Pressure Emergency", data.sspe);
        dataPoints[2] = new SwitchDataPoints("Fan Failure", data.fan_error);
        dataPoints[3] = new SwitchDataPoints("No Vent Flow", data.vent_error);
        dataPoints[4] = new SwitchDataPoints("Vehicle Power Present", data.vehicle_power);
        dataPoints[5] = new SwitchDataPoints("H2O is off", data.h2o_off);
        dataPoints[6] = new SwitchDataPoints("O2 is off", data.o2_off);

        //data not coming from server
        dataPoints[7] = new SwitchDataPoints("Battery AMP High", data.battery_amp_high);
        dataPoints[8] = new SwitchDataPoints("Battery VDC Low", data.battery_vdc_low);
        dataPoints[9] = new SwitchDataPoints("Suit Pressure Low", data.suit_pressure_low);
        dataPoints[10] = new SwitchDataPoints("Spacesuit Pressure High", data.spacesuit_pressure_high);
        dataPoints[11] = new SwitchDataPoints("O2 High Use", data.o2_high_use);
        dataPoints[12] = new SwitchDataPoints("SOP Pressure Low", data.sop_pressure_low);
        dataPoints[13] = new SwitchDataPoints("CO2 High", data.co2_high);
    }

    void stdDev(List<NumericalTelemetry> tDataS, int numAr){ 
        float[] mean = new float[numAr];

        for (int i = 0; i < tDataS.Count; i++){
            mean[0] += tDataS[i].heart_bpm;
            mean[1] += tDataS[i].p_sub;
            mean[2] += tDataS[i].p_suit;
            mean[3] += tDataS[i].t_sub;
            mean[4] += tDataS[i].v_fan;
            mean[5] += tDataS[i].p_o2;
            mean[6] += tDataS[i].rate_o2;
            mean[7] += tDataS[i].cap_battery;
            mean[8] += tDataS[i].p_h2o_g;
            mean[9] += tDataS[i].p_h2o_l;
            mean[10] += tDataS[i].p_sop;
            mean[11] += tDataS[i].rate_sop;
        }
       
        for (int i = 0; i < numAr; i++)
        {
            mean[i] /= tDataS.Count;
        }

        float[] variances = new float[numAr];
        float[] deviats = new float[numAr];

        for (var dSet = 0; dSet < tDataS.Count; dSet++) // loop through different data sets
        {
            variances[0] += ((tDataS[dSet].heart_bpm - mean[0]) * (tDataS[dSet].heart_bpm - mean[0]));
            variances[1] += ((tDataS[dSet].p_sub - mean[1]) * (tDataS[dSet].p_sub - mean[1]));
            variances[2] += ((tDataS[dSet].p_suit - mean[2]) * (tDataS[dSet].p_suit - mean[2]));
            variances[3] += ((tDataS[dSet].t_sub - mean[3]) * (tDataS[dSet].t_sub - mean[3]));
            variances[4] += ((tDataS[dSet].v_fan - mean[4]) * (tDataS[dSet].v_fan - mean[4]));
            variances[5] += ((tDataS[dSet].p_o2 - mean[5]) * (tDataS[dSet].p_o2 - mean[5]));
            variances[6] += ((tDataS[dSet].rate_o2 - mean[6]) * (tDataS[dSet].rate_o2 - mean[6]));
            variances[7] += ((tDataS[dSet].cap_battery - mean[7]) * (tDataS[dSet].cap_battery - mean[7]));
            variances[8] += ((tDataS[dSet].p_h2o_g - mean[8]) * (tDataS[dSet].p_h2o_g - mean[8]));
            variances[9] += ((tDataS[dSet].p_h2o_l - mean[9]) * (tDataS[dSet].p_h2o_l - mean[9]));
            variances[10] += ((tDataS[dSet].p_sop - mean[10]) * (tDataS[dSet].p_sop - mean[10]));
            variances[11] += ((tDataS[dSet].rate_sop - mean[11]) * (tDataS[dSet].rate_sop - mean[11]));
        }
        //finalize variances by dividing by N (where N is number of data sets)
        for (int i = 0; i < tDataS.Count; i++)
        {
            variances[i] /= tDataS.Count;
        }
        //take the square root of values held in variances to get the final deviations
        for (int i = 0; i < numAr; i++)
        {
            deviats[i] = Mathf.Sqrt(variances[i]);
        }

        if(tDataS[tDataS.Count-1].cap_battery > (deviats[7] + mean[7]))
        {
            switchData.battery_amp_high = true;
        }
        else
        {
            switchData.battery_amp_high = false;
        }

        if (tDataS[tDataS.Count-1].cap_battery < (mean[7] - deviats[7]))
        {
            switchData.battery_vdc_low = true;
        }
        else
        {
            switchData.battery_vdc_low = false;
        }

        if (tDataS[tDataS.Count-1].p_suit < (mean[2] - deviats[2]))
        {
            switchData.suit_pressure_low = true;
        }
        else
        {
            switchData.suit_pressure_low = false;
        }

        if (tDataS[tDataS.Count-1].p_suit > (deviats[2] + mean[2]))
        {
            switchData.spacesuit_pressure_high = true;
        }
        else
        {
            switchData.spacesuit_pressure_high = false;
        }

        if (tDataS[tDataS.Count-1].rate_o2 > (deviats[6] + mean[6]))
        {
            switchData.o2_high_use = true;
        }
        else
        {
            switchData.o2_high_use = false;
        }

        if (tDataS[tDataS.Count-1].p_sop < (mean[10] - deviats[10]))
        {
            switchData.sop_pressure_low = true;
        }
        else
        {
            switchData.sop_pressure_low = false;
        }

        if (tDataS[tDataS.Count-1].p_o2 < (mean[5] - deviats[5]))
        {
            switchData.co2_high = true;
        }
        else
        {
            switchData.co2_high = false;
        }
    }
}