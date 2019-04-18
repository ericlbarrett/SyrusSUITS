using System;
using System.Collections;
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

    public GameObject sop_on;         //Secondary oxygen pack is active
    public GameObject sspe;           //Spacesuit pressure emergency
    public GameObject fan_error;      //Cooling fan failure
    public GameObject vent_error;     //No vent flow
    public GameObject vehicle_power;  //Receiving power through spacecraft
    public GameObject h2o_off;        //H2O system is offline
    public GameObject o2_off;         //O2 system is offline

    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(GetNumerical());
        StartCoroutine(GetSwitch());
    }

    // Update is called once per frame
    void Update()
    {
      //to add switch statement
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

                int numAr = 15; //Number of components

                SuitDataPoints[] suitDataPoints = new SuitDataPoints[numAr];

                //Check later
                setSuitData(numericalData, suitDataPoints);
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
        //dataPoints[12] = new SuitDataPoints("Time Life Battery", data.t_battery, "hh:mm:ss", 0f, 36000f);
        //dataPoints[13] = new SuitDataPoints("Time Life Oxygen", data.t_oxygen, "hh:mm:ss", 0, 36000);
        //dataPoints[14] = new SuitDataPoints("Time Life Water", data.t_water, "hh:mm:ss", 0, 36000);
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
}