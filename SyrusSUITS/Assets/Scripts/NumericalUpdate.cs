//using System;
//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.Networking;
//using UnityEngine.UI.ProceduralImage;

//[System.Serializable]
//public class NumericalTelemetry {
//	public int heart_bpm;		//heart beats per minute
//    public float p_suit;    	//Internal Suit Pressure (range:2-4 psid)
//    public float p_sub;       	//Sub pressure      (range: 2-4 psia)
//    public float t_sub;       	//Sub temperature   (degrees Fahrenheit)
//    public int v_fan;       	//Fan tachometer    (range: 10000 - 40000 RPM)
//    public float p_o2;        	//Oxygen pressure   (range: 750 - 950 psia)
//    public float rate_o2;   	//Oxygen rate       (range: 0.5 - 1 psi/min)
//    public float cap_battery; 	//Battery Capacity  (range: 0 - 30 amp-hr)
//    public float p_h2o_g;     	//H2O gas pressure  (range: 14 - 16 psia)
//    public float p_h2o_l;     	//H20 liquid pressure            (range: 14 - 16 psia)
//    public float p_sop;       	//Secondary Oxygen Pack pressure (range: 750 - 950)
//    public float rate_sop;  	//Oxygen rate for Secondary      (range: 0.5 - 1 psi/min)
//	public string t_battery;    //time life battery (range: 0-10 hours) ("hh:mm:ss")
//    public string t_oxygen;    	//time life oxygen (range: 0-10 hours) ("hh:mm:ss")
//    public string t_water;    	//time life water (range: 0-10 hours) ("hh:mm:ss")
//	//public string t_eva;		//EVA time (stopwatch for current EVA)
//	public string create_date;	//date/time the packet of data was created
//}

//public class DataPoints {
//	public string name;
//	public object value;
//	public string unit;
//	public int status;

//	public DataPoints(string name, object value, string unit, float min, float max) {
//		this.name = name;
//		this.value = value;
//		this.unit = unit;

//		if(this.name == "Time Life Battery" || this.name == "Time Life Oxygen" || this.name == "Time Life Water") {
//			//Debug.Log(value);
//			string strSeconds = value.ToString();

//			//Debug.Log(strSeconds);
//			float seconds = (float)TimeSpan.Parse(strSeconds).TotalSeconds;
//			//do math for yellow color
//			if(seconds > max || seconds < min) {
//				this.status = 3;	//red
//			} else {
//				this.status = 1;	//green
//			}
//			//Debug.Log("converted: " + seconds);
//		} else {
//			float yellowPercent = 15f/100f;
//			//Debug.Log("yellow %: " + yellowPercent);
//			float delta = max - min;
//			//Debug.Log("delta: "+ delta);
//			float plusMinus = delta * yellowPercent;
//			//Debug.Log("plusMinus: "+ plusMinus);
//			float yellowMin = min + plusMinus;
//			//Debug.Log("yellowMin: "+ yellowMin);
//			float yellowMax = max - plusMinus;
//			//Debug.Log("yellowMax: "+ yellowMax);

//			float num = Convert.ToSingle(value);

//			if( num > max || num < min) {
//				this.status = 3;	//red
//			} 
//			else if((num > min && num < yellowMin) || (num > yellowMax && num < max)) {
//				this.status = 2;	//yellow
//			}
//			else {
//				this.status = 1;	//green
//			}
//		}
//		//Debug.Log("converted: " + num);
//	}
//}

////public class DataUI {
////	public GameObject panel;
////	public GameObject dataName;
////	public GameObject dataValue;

////	public DataUI(string panelName, DataPoints data) {
////		this.panel = GameObject.Find(panelName);

////		this.dataName = this.panel.transform.Find("name").gameObject;
////		this.dataName.GetComponent<Text>().text = data.name;

////		this.dataValue = this.panel.transform.Find("value").gameObject;
////		string valueString = data.value.ToString();
////		this.dataValue.GetComponent<Text>().text = valueString;

////		this.dataValue = this.panel.transform.Find("unit").gameObject;
////		this.dataValue.GetComponent<Text>().text = data.unit;

////		if(data.status == 1) {
////			this.panel.GetComponent<ProceduralImage>().color = Color.green;
////		}
////		if(data.status == 2) {
////			this.panel.GetComponent<ProceduralImage>().color = Color.yellow;
////		}
////		if(data.status == 3) {
////			this.panel.GetComponent<ProceduralImage>().color = Color.red;
////		}
		
////	}
////}

//public class NumericalUpdate : MonoBehaviour {

//	public NumericalTelemetry numericalData;

//	// Use this for initialization
//	void Start () {
//		StartCoroutine(GetNumerical());
//        //initialize
//        //tDataS = new List<NumericalTelemetry>();
//        //deviats = new float[dataPC];
//        //stnNorm = new List<List<float>>();
//        //for (int i = 0; i < dataPC; i++) deviats[i] = 0;
//    }
	
//	// Update is called once per frame
//	void Update () {
//	}

//	IEnumerator GetNumerical() {
//		while(true) {
//			//UnityWebRequest www = UnityWebRequest.Get("https://nasa-bhitt.c9users.io/api/suit/recent");
//			UnityWebRequest www = UnityWebRequest.Get("https://gemini-program.herokuapp.com/api/suit/recent");
//			yield return www.SendWebRequest();

//			if(www.isNetworkError || www.isHttpError) {
//				Debug.Log(www.error);
//			} else {
//				//Debug.Log(www.downloadHandler.text);
//				string jsonString = www.downloadHandler.text;
//				jsonString = jsonString.Replace('[',' ').Replace(']',' ');
//				//NumericalTelemetry numericalData = JsonUtility.FromJson<NumericalTelemetry>(jsonString);
//				numericalData = JsonUtility.FromJson<NumericalTelemetry>(jsonString);

//				//if time life battery is erroneous when received from server
//				if(numericalData.t_battery[0] == '-') {
//					numericalData.t_battery = "00:00:00";
//				}

//                //check for negative time value
//                if (numericalData.t_battery[0] == '-') numericalData.t_battery = "00:00:00";


//				int numComponents = 15;

//				DataPoints [] dataPoints = new DataPoints[numComponents];

//				setDataPoints(numericalData, dataPoints);
//				for(int i = 0; i<dataPoints.Length; i++) {
//                    Debug.Log(dataPoints[1].name  + " : " + dataPoints[1].value + " " + dataPoints[1].unit);
//					if(dataPoints[i].status == 3) {
//						//NotificationService.Issue(dataPoints[i].name, "has failed", 0);
//					}
//				}
//				//makeNumericalDataUI(dataPoints);
//			}
//			//make a request every 10 seconds
//			yield return new WaitForSeconds(10);
//		}
//	}

//	public void setDataPoints(NumericalTelemetry data, DataPoints [] dataPoints) {
//		dataPoints[0] = new DataPoints("Heart Rate", data.heart_bpm, "bpm", 85f, 90f);
//		dataPoints[1] = new DataPoints("Internal Suit Pressure", data.p_suit, "psid", 2.0f, 4.0f);
//		dataPoints[2] = new DataPoints("Sub Pressure", data.p_sub, "psia", 2.0f, 4.0f);
//		dataPoints[3] = new DataPoints("Sub Temperature", data.t_sub, "\u00B0F", 4f, 6f);
//		dataPoints[4] = new DataPoints("Fan Tachometer", data.v_fan, "RPM", 10000f, 40000f);
//		dataPoints[5] = new DataPoints("Oxygen Pressure", data.p_o2, "psia", 750f, 950f);
//		dataPoints[6] = new DataPoints("Oxygen Rate", data.rate_o2, "psi/min", 0.5f, 1.0f);
//		dataPoints[7] = new DataPoints("Battery Capacity", data.cap_battery, "amp-hr", 0f, 30f);
//		dataPoints[8] = new DataPoints("H2O Gas Pressure", data.p_h2o_g, "psia", 14f, 16f);
//		dataPoints[9] = new DataPoints("H2O Liquid Pressure", data.p_h2o_l, "psia", 14f, 16f);
//		dataPoints[10] = new DataPoints("Secondary Oxygen Pack Pressure", data.p_sop, "psia", 750f, 950f);
//		dataPoints[11] = new DataPoints("Secondary Oxygen Pack Flow Rate", data.rate_sop, "psi/min", 0.5f, 1.0f);
//		dataPoints[12] = new DataPoints("Time Life Battery", data.t_battery, "hh:mm:ss", 0, 36000);
//		dataPoints[13] = new DataPoints("Time Life Oxygen", data.t_oxygen, "hh:mm:ss", 0, 36000);
//		dataPoints[14] = new DataPoints("Time Life Water", data.t_water, "hh:mm:ss", 0, 36000);
//		//dataPoints[15] = new DataPoints("Last Update", data.create_date, "find out", 1, 1);
//	}

//	//public void makeNumericalDataUI(DataPoints [] dataPoints) {
//	//	DataUI [] dataUIArray = new DataUI[dataPoints.Length];
//	//	for(int i=0; i<dataUIArray.Length; i++) {
//	//		dataUIArray[i] = new DataUI(("data-" + (i+1)), dataPoints[i]);
//	//	}
//	//}

//    //find the standard deviation for a set of data
//    /*void stdDev(List<NumericalTelemetry> tDataS)
//    {
//        //Debug.Log("t_battery to seconds: " + (float)TimeSpan.Parse("00:30:00").TotalSeconds);
//        //find the mean, starting with adding all values
//        float[] means = new float[dataPC];
//        //Debug.Log("means[0] : " + means[0]);

//        for (var dSet = 0; dSet < tDataS.Count; dSet++) // loop through different data sets
//        {
//            //for(var setEl = 0; setEl < 15; setEl) // loop through different data points in a set
//            //{
//            //    if (setEl == 1 || setEl == 2 || setEl == 3 || setEl == 7) means[setEl] += 1;
//            //    else means[setEl] += tDataS[dSet].
//            //}
//            //means[0] += tDataS[dSet].i_suit_p;
//            means[0] += tDataS[dSet].heart_bpm;
//            means[1] += tDataS[dSet].p_suit;
//            means[2] += tDataS[dSet].p_sub;
//            means[3] += tDataS[dSet].t_sub;
//            means[4] += tDataS[dSet].v_fan;
//            //means[5] += (float)TimeSpan.Parse(tDataS[dSet].t_battery).TotalSeconds; //string value t-eva
//            means[5] += tDataS[dSet].p_o2;
//            means[6] += tDataS[dSet].rate_o2;
//            means[7] += tDataS[dSet].cap_battery;
//            means[8] += tDataS[dSet].p_h2o_g;
//            means[9] += tDataS[dSet].p_h2o_l;
//            means[10] += tDataS[dSet].p_sop;
//            means[11] += tDataS[dSet].rate_sop;
//            means[12] += (float)TimeSpan.Parse(tDataS[dSet].t_battery).TotalSeconds; //string value
//            means[13] += (float)TimeSpan.Parse(tDataS[dSet].t_oxygen).TotalSeconds;  //string value
//            means[14] += (float)TimeSpan.Parse(tDataS[dSet].t_water).TotalSeconds;   //string value
//        }

//        //finalize mean values
//        for (int i = 0; i < dataPC; i++)
//        {
//            means[i] /= tDataS.Count;
//        }

//        //Debug.Log("Mean: " + means[0]);

//        //the sum of each difference with the mean
//        float[] variances = new float[dataPC];

//        //find the variances, first by summing up the differences squared
//        for (var dSet = 0; dSet < tDataS.Count; dSet++) // loop through different data sets
//        {
//            variances[0] += ((tDataS[dSet].heart_bpm - means[0]) * (tDataS[dSet].heart_bpm - means[0]));
//            variances[1] += ((tDataS[dSet].p_suit - means[1]) * (tDataS[dSet].p_suit - means[1]));                 //string value
//            variances[2] += ((tDataS[dSet].p_sub - means[2]) * (tDataS[dSet].p_sub - means[2]));
//            variances[3] += ((tDataS[dSet].t_sub - means[3]) * (tDataS[dSet].t_sub - means[3]));
//            variances[4] += ((tDataS[dSet].v_fan - means[4]) * (tDataS[dSet].v_fan - means[4]));
//            //variances[5] += (((float)TimeSpan.Parse(tDataS[dSet].t_eva).TotalSeconds - means[6]) * ((float)TimeSpan.Parse(tDataS[dSet].t_eva).TotalSeconds - means[6])); //string value                  //string value
//            variances[5] += ((tDataS[dSet].p_o2 - means[5]) * (tDataS[dSet].p_o2 - means[5]));
//            variances[6] += ((tDataS[dSet].rate_o2 - means[6]) * (tDataS[dSet].rate_o2 - means[6]));
//            variances[7] += ((tDataS[dSet].cap_battery - means[7]) * (tDataS[dSet].cap_battery - means[7]));
//            variances[8] += ((tDataS[dSet].p_h2o_g - means[8]) * (tDataS[dSet].p_h2o_g - means[8]));
//            variances[9] += ((tDataS[dSet].p_h2o_l - means[9]) * (tDataS[dSet].p_h2o_l - means[9]));
//            variances[10] += ((tDataS[dSet].p_sop - means[10]) * (tDataS[dSet].p_sop - means[10]));
//            variances[11] += ((tDataS[dSet].rate_sop - means[11]) * (tDataS[dSet].rate_sop - means[11]));
//            variances[12] += (((float)TimeSpan.Parse(tDataS[dSet].t_battery).TotalSeconds - means[12]) * ((float)TimeSpan.Parse(tDataS[dSet].t_battery).TotalSeconds - means[12])); //string value
//            variances[13] += (((float)TimeSpan.Parse(tDataS[dSet].t_oxygen).TotalSeconds - means[13]) * ((float)TimeSpan.Parse(tDataS[dSet].t_oxygen).TotalSeconds - means[13]));   //string value
//            variances[14] += (((float)TimeSpan.Parse(tDataS[dSet].t_water).TotalSeconds - means[14]) * ((float)TimeSpan.Parse(tDataS[dSet].t_water).TotalSeconds - means[14]));     //string value
//        }
//        //finalize variances by dividing by N (where N is number of data sets)
//        for (int i = 0; i < dataPC; i++)
//        {
//            variances[i] /= tDataS.Count;
//        }

//        //take the square root of values held in variances to get the final deviations
//        for (int i = 0; i < dataPC; i++)
//        {
//            deviats[i] = Mathf.Sqrt(variances[i]);
//        }

//        //take final deviations and calculate the standard normals
//        List<float> newStd = new List<float>(); //newest standard norm

//        //find individual standard normals
//        //Debug.Log("Heart bpm : " + latestT.heart_bpm);
//        //Debug.Log("means[0] : " + means[0]);
//        //Debug.Log("Standard Normal heart bpm : " + ((latestT.heart_bpm - means[0]) / (deviats[0])));
//        newStd.Add(((latestT.heart_bpm - means[0]) / (deviats[0])));
//        newStd.Add((latestT.p_suit - means[1]) / deviats[1]);
//        newStd.Add((latestT.p_sub - means[2]) / deviats[2]);
//        newStd.Add((latestT.t_sub - means[3]) / deviats[3]);
//        newStd.Add((latestT.v_fan - means[4]) / deviats[4]);
//        newStd.Add((latestT.p_o2 - means[5]) / deviats[5]);
//        newStd.Add((latestT.rate_o2 - means[6]) / deviats[6]);
//        newStd.Add((latestT.cap_battery - means[7]) / deviats[7]);
//        newStd.Add((latestT.p_h2o_g - means[8]) / deviats[8]);
//        newStd.Add((latestT.p_h2o_l - means[9]) / deviats[9]);
//        newStd.Add((latestT.p_sop - means[10]) / deviats[10]);
//        newStd.Add((latestT.rate_sop - means[11]) / deviats[11]);
//        newStd.Add(((float)TimeSpan.Parse(latestT.t_battery).TotalSeconds - means[12]) / deviats[12]);
//        newStd.Add(((float)TimeSpan.Parse(latestT.t_oxygen).TotalSeconds - means[13]) / deviats[13]);
//        newStd.Add(((float)TimeSpan.Parse(latestT.t_water).TotalSeconds - means[14]) / deviats[14]);
//        //means[5] += (float)TimeSpan.Parse(tDataS[dSet].t_battery).TotalSeconds; //string value t-eva

//        //loop and remove NaNs
//        for (int i = 0; i < dataPC; i++)
//        {
//            if (deviats[i] == 0)
//            {
//                //Debug.Log("value isNaN");
//                newStd.Insert(i, 0);
//                newStd.RemoveAt(i + 1);
//            }
//        }

//        //output current standard normal list (testing)
//        //printCurrentStandard(newStd);
//        //add to the list of all standard normals
//        stnNorm.Add(newStd);

//    }

//    void printD()   //print deviations
//    {
//        Debug.Log("STANDARD DEVIATIONS");
//        Debug.Log("--------------------");
//        Debug.Log("Heart Beats Per Minute:" + deviats[0]);
//        Debug.Log("Internal Suit Pressure: " + deviats[1]);
//        Debug.Log("Sub pressure: " + deviats[2]);
//        Debug.Log("Sub temperature: " + deviats[3]);
//        Debug.Log("Fan tachometer: " + deviats[4]);
//        //Debug.Log("EVA time: " + deviats[5]);
//        Debug.Log("Oxygen pressure: " + deviats[5]);
//        Debug.Log("Oxygen rate: " + deviats[6]);
//        Debug.Log("Battery Capacity: " + deviats[7]);
//        Debug.Log("H2O gas pressure: " + deviats[8]);
//        Debug.Log("H20 liquid pressure: " + deviats[9]);
//        Debug.Log("Secondary Oxygen Pack pressure: " + deviats[10]);
//        Debug.Log("Oxygen rate for Secondary: " + deviats[11]);
//        Debug.Log("Time Life Battery: " + deviats[12]);
//        Debug.Log("Time Life Oxygen: " + deviats[13]);
//        Debug.Log("Time Life Water: " + deviats[14]);
//    }

//    void printCurrentStandard(List<float> currentStd)
//    {
//        Debug.Log("Current Standard Normals");
//        Debug.Log("------------------------");
//        int index = 0;
//        foreach (float std in currentStd)
//        {
//            Debug.Log("Datapoint[" + index + "] : " + std);
//            index++;
//        }
//    }
//    */
//}
