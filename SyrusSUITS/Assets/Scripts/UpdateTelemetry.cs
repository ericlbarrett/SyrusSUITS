using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

[System.Serializable]
public class NumericalTelemetry {
    public int heart_bpm;       //Heart beats per minute
    public float p_sub;         //Sub pressure
    public float t_sub;         //Sub temperature
    public int v_fan;           //Fan tachometer
    public string t_eva;        //EVA time
    public float p_o2;          //Oxygen pressure
    public float cap_battery;   //Batter capacity
    public float p_h2o_g;       //H2O gas pressure
    public float p_h2o_l;       //H2O liquid pressure
    public float p_sop;         //Secondary oxygen pack pressure
    public float rate_sop;      //Oxygen rate for secondary pack
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
            //old server: https://gemini-program.herokuapp.com/api/suit/recent
            UnityWebRequest www = UnityWebRequest.Get("http://192.70.120.211:3000/api/simulation/state");  //New server from Test Week 2019
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

                int numComponents = 10; //Number of components

                SuitDataPoints[] suitDataPoints = new SuitDataPoints[numComponents];

                setSuitData(numericalData, suitDataPoints);
            }
            yield return new WaitForSeconds(10);
        }
    }

    //Sets Suit Telemetry
    public void setSuitData(NumericalTelemetry data, SuitDataPoints[] dataPoints) {
        dataPoints[0] = new SuitDataPoints("Sub Pressure", data.p_sub, "psia", 2.0f, 4.0f);
        dataPoints[1] = new SuitDataPoints("Sub Temperature", data.t_sub, "\u00B0F", 4f, 6f);
        dataPoints[2] = new SuitDataPoints("Fan Tachometer", data.v_fan, "RPM", 10000f, 40000f);
        dataPoints[3] = new SuitDataPoints("EVA Time", data.t_eva, "hh:mm:ss", 0, 36000);
        dataPoints[4] = new SuitDataPoints("Oxygen Pressure", data.p_o2, "psia", 750f, 950f);
        dataPoints[5] = new SuitDataPoints("Battery Capacity", data.cap_battery, "amp-hr", 0f, 30f);
        dataPoints[6] = new SuitDataPoints("H2O Gas Pressure", data.p_h2o_g, "psia", 14f, 16f);
        dataPoints[7] = new SuitDataPoints("H2O Liquid Pressure", data.p_h2o_l, "psia", 14f, 16f);
        dataPoints[8] = new SuitDataPoints("Secondary Oxygen Pack Pressure", data.p_sop, "psia", 750f, 950f);
        dataPoints[9] = new SuitDataPoints("Secondary Oxygen Pack Flow Rate", data.rate_sop, "psi/min", 0.5f, 1.0f);
    }

    IEnumerator GetSwitch() {
        while (true) {
            UnityWebRequest www = UnityWebRequest.Get("https://gemini-program.herokuapp.com/api/suitswitch/recent");    //Placeholder
            yield return www.SendWebRequest();
            if (www.isNetworkError || www.isHttpError) {
                Debug.Log(www.error);
            } else {
                string jsonString = www.downloadHandler.text;
                jsonString = jsonString.Replace('[', ' ').Replace(']', ' ');

                SwitchTelemetry switchData = JsonUtility.FromJson<SwitchTelemetry>(jsonString);
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
}