using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

using System.IO.Ports;
 

public class ArduinoCommunication : MonoBehaviour {
SerialPort stream;
public string msg  = "1";
public Image img;
public Sprite a,b;
bool on = false;
	// Use this for initialization
	void Start () {
		stream = new SerialPort("COM9", 9600);
		stream.ReadTimeout = 50;
		stream.Open();

		StartRead();

		WriteToArduino("Hallo");
	}
	


	// Update is called once per frame
	void Update () {
		//delay this
		
	}
	void OnApplicationQuit(){
		if(stream!=null)
		stream.Close();
	}
public void OnClick(){
	if(on){
		on = false;
		msg = "0";
	}else{
		on = true;
		msg = "1";
	}
	WriteToArduino(msg);
}
	// IEnumerator StartWrite(){
	// 	while(true){
	// 		Debug.Log("Send out "+ msg);
	// 		WriteToArduino(msg);
	// 		if(on){
	// 			on = false;
	// 			msg = "0";
	// 		}else{
	// 			on = true;
	// 			msg = "1";
	// 		}
	// 		yield return new WaitForSeconds(1f);
	// 	}

	// }
	public void WriteToArduino(string _msg){
		stream.WriteLine(_msg);
		stream.BaseStream.Flush();
	}
	public string ReadFromArduino(int timeout = 0){
		stream.ReadTimeout = timeout;
		try{
			return stream.ReadLine();
		}
		catch(TimeoutException e){
			return null;
		}
	}
void StartRead(){
	StartCoroutine(AsynchronousReadFromArduino(
				(string s) => HandleData(s),     // Callback
		        () => Debug.LogError("Error!"), // Error callback
		        10000f                          // Timeout (milliseconds)
		    )
		);
}
void HandleData(string data){
if(data.Equals("0")){
		img.color= Color.white;
	}else if(data.Equals("1")){
		img.color= Color.green;


	}
}
public IEnumerator AsynchronousReadFromArduino(Action<string> callback, Action fail = null, float timeout = float.PositiveInfinity) {
    DateTime initialTime = DateTime.Now;
    DateTime nowTime;
    TimeSpan diff = default(TimeSpan);
 
    string dataString = null;
 
    do {
        try {
            dataString = stream.ReadLine();
        }
        catch (TimeoutException) {
            dataString = null;
        }
 
        if (dataString != null)
        {
            callback(dataString);
            StartRead();
            yield break; // Terminates the Coroutine
        } else
            yield return null; // Wait for next frame
 
        nowTime = DateTime.Now;
        diff = nowTime - initialTime;
 
    } while (diff.Milliseconds < timeout);
 
    if (fail != null)
        fail();
    yield return null;
}

}
