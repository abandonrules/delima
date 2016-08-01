using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;

public class ExampleBasicLogic : MonoBehaviour {

	public Renderer profilePicturePlaneRenderer;
	public Text logWindow;
	public Renderer profilePicturePlaneRenderer1;
	public Renderer profilePicturePlaneRenderer2;
	public Renderer profilePicturePlaneRenderer3;
	public Renderer profilePicturePlaneRenderer4;
	public Renderer profilePicturePlaneRenderer5;
	public Renderer profilePicturePlaneRenderer6;

	void Awake () {
		// register events
//		AirConsole.instance.onReady += OnReady;
//		AirConsole.instance.onMessage += OnMessage;
//		AirConsole.instance.onConnect += OnConnect;
//		AirConsole.instance.onDisconnect += OnDisconnect;
//		AirConsole.instance.onDeviceStateChange += OnDeviceStateChange;
//		AirConsole.instance.onCustomDeviceStateChange += OnCustomDeviceStateChange;
//		AirConsole.instance.onDeviceProfileChange += OnDeviceProfileChange;
//		AirConsole.instance.onGameEnd += OnGameEnd;
		logWindow.text = "Connecting... \n \n";
	}

	void OnReady (string code) {
		//Log to on-screen Console
		logWindow.text = "ExampleBasic: AirConsole is ready! \n \n";

		//Mark Buttons as Interactable as soon as AirConsole is ready
		Button[] allButtons = (Button[])GameObject.FindObjectsOfType ((typeof(Button)));
		foreach (Button button in allButtons) {
			button.interactable = true;
		}
	}

	void OnMessage (int from, JToken data) {
		//Log to on-screen Console
		logWindow.text = logWindow.text.Insert (0, "Incoming message from device: " + from + ": " + data.ToString () + " \n \n");

	}

	void OnConnect (int device_id) {
		//Log to on-screen Console
		logWindow.text = logWindow.text.Insert (0, "Device: " + device_id + " connected. \n \n");
		updateprofiles ();
	}

	void OnDisconnect (int device_id) {
		//Log to on-screen Console
		logWindow.text = logWindow.text.Insert (0, "Device: " + device_id + " disconnected. \n \n");
		updateprofiles ();
	}

	void OnDeviceStateChange (int device_id, JToken data) {
		//Log to on-screen Console
		logWindow.text = logWindow.text.Insert (0, "Device State Change on device: " + device_id + ", data: " + data + "\n \n");
	}

	void OnCustomDeviceStateChange (int device_id, JToken custom_data) {
		//Log to on-screen Console
		logWindow.text = logWindow.text.Insert (0, "Custom Device State Change on device: " + device_id + ", data: " + custom_data + "\n \n");
	}

	void OnDeviceProfileChange (int device_id) {
		//Log to on-screen Console
		logWindow.text = logWindow.text.Insert (0, "Device " + device_id + " made changes to its profile. \n \n");
		updateprofiles ();
	}


	void OnGameEnd () {
		Debug.Log ("OnGameEnd is called");
		Camera.main.enabled = false;
		Time.timeScale = 0.0f;
	}

	void Update () {
		//If any controller is pressing a 'Rotate' button, rotate the AirConsole Logo in the scene
	}
	void updateprofiles(){
		DisplayProfilePicture (0,profilePicturePlaneRenderer1);
		DisplayProfilePicture (1,profilePicturePlaneRenderer2);
		DisplayProfilePicture (2,profilePicturePlaneRenderer3);
		DisplayProfilePicture (3,profilePicturePlaneRenderer4);
		DisplayProfilePicture (4,profilePicturePlaneRenderer5);
		DisplayProfilePicture (5,profilePicturePlaneRenderer6);
	}
	public void DisplayProfilePicture (int device_id,Renderer profilePicturePlaneRenderer) {
		//We cannot assume that the first controller's device ID is '1', because device 1 
		//might have left and now the first controller in the list has a different ID.
		//Never hardcode device IDs!		
		int idOfController = AirConsole.instance.GetControllerDeviceIds () [device_id];

		string urlOfProfilePic = AirConsole.instance.GetProfilePicture (idOfController, 512);
		//Log url to on-screen Console
		logWindow.text = logWindow.text.Insert (0, "URL of Profile Picture of " + device_id +" Controller: " + urlOfProfilePic + "\n \n");
		StartCoroutine (DisplayUrlPicture (urlOfProfilePic, profilePicturePlaneRenderer));
	}

	public void SendMessageToController1 () {
		//Say Hi to the first controller in the GetControllerDeviceIds List.

		//We cannot assume that the first controller's device ID is '1', because device 1 
		//might have left and now the first controller in the list has a different ID.
		//Never hardcode device IDs!
		int idOfFirstController = AirConsole.instance.GetControllerDeviceIds () [0];

		AirConsole.instance.Message (idOfFirstController, "Hey there, first controller!");

		//Log to on-screen Console
		logWindow.text = logWindow.text.Insert (0, "Sent a message to first Controller \n \n");
	}

	public void BroadcastMessageToAllDevices () {
		AirConsole.instance.Broadcast ("Hey everyone!");
		logWindow.text = logWindow.text.Insert (0, "Broadcast a message. \n \n");
	}

	public void DisplayDeviceID () {
		//Get the device id of this device
		int device_id = AirConsole.instance.GetDeviceId ();

		//Log to on-screen Console		
		logWindow.text = logWindow.text.Insert (0, "This device's id: " + device_id + "\n \n");
	}

	public void DisplayNicknameOfFirstController () {
		//We cannot assume that the first controller's device ID is '1', because device 1 
		//might have left and now the first controller in the list has a different ID.
		//Never hardcode device IDs!		
		int idOfFirstController = AirConsole.instance.GetControllerDeviceIds () [0];

		//To get the controller's name right, we get their nickname by using the device id we just saved
		string nicknameOfFirstController = AirConsole.instance.GetNickname (idOfFirstController);

		//Log to on-screen Console
		logWindow.text = logWindow.text.Insert (0, "The first controller's nickname is: " + nicknameOfFirstController + "\n \n");
		
	}

	private IEnumerator DisplayUrlPicture (string url, Renderer profilePicturePlaneRenderer) {

		Color color = Color.white;
		color.a = 0;
		profilePicturePlaneRenderer.material.color = color;

		// Start a download of the given URL
		WWW www = new WWW (url);

		// Wait for download to complete
		yield return www;

		// assign texture
		profilePicturePlaneRenderer.material.mainTexture = www.texture;
		//Color color = Color.white;
		color.a = 1;
		profilePicturePlaneRenderer.material.color = color;

		//yield return new WaitForSeconds (3.0f);

		//color.a = 0;
		//profilePicturePlaneRenderer.material.color = color;

	}

	public void DisplayAllCustomDataOfFirstController () {
		//We cannot assume that the first controller's device ID is '1', because device 1 
		//might have left and now the first controller in the list has a different ID.
		//Never hardcode device IDs!		
		int idOfFirstController = AirConsole.instance.GetControllerDeviceIds () [0];

		//Get the Custom Device State of the first Controller
		JToken data = AirConsole.instance.GetCustomDeviceState (idOfFirstController);
		
		if (data != null) {
			
			// Check if data has multiple properties
			if (data.HasValues) {
				
				// go through all properties
				foreach (var prop in ((JObject)data).Properties()) {
					logWindow.text = logWindow.text.Insert (0, "Custom Data on first Controller - Key:  " + prop.Name + " / Value:" + prop.Value + "\n \n");
				}

			} else {
				//If there's only one property, log it to on-screen Console
				logWindow.text = logWindow.text.Insert (0, "Custom Data on first Controller: " + data + "\n \n");
			}
		} else {
			logWindow.text = logWindow.text.Insert (0, "No Custom Data on first Controller \n \n");
		}
	}

	public void DisplayCustomPropertyHealthOnFirstController () {
		//We cannot assume that the first controller's device ID is '1', because device 1 
		//might have left and now the first controller in the list has a different ID.
		//Never hardcode device IDs!		
		int idOfFirstController = AirConsole.instance.GetControllerDeviceIds () [0];
		
		//Get the Custom Device State of the first Controller
		JToken data = AirConsole.instance.GetCustomDeviceState (idOfFirstController);

		//If it exists, get the data's health property and cast it as int
		if (data != null && data ["health"] != null) {
			int healthOfFirstController = (int)data ["health"];
			logWindow.text = logWindow.text.Insert (0, "value 'health':" + healthOfFirstController + "\n \n");
		} else {
			logWindow.text = logWindow.text.Insert (0, "No 'health' property set on first Controller \n \n");
		}
	}

	public string DisplayCustomProperty (int ControllerDeviceIds,string ReqData) {
		//We cannot assume that the first controller's device ID is '1', because device 1 
		//might have left and now the first controller in the list has a different ID.
		//Never hardcode device IDs!		
		int idOfController = AirConsole.instance.GetControllerDeviceIds () [ControllerDeviceIds];

		//Get the Custom Device State of the first Controller
		JToken data = AirConsole.instance.GetCustomDeviceState (idOfController);

		//If it exists, get the data's health property and cast it as int
		if (data != null && data [ReqData] != null) {
			string ReqDataContents  = (string)data [ReqData];
			logWindow.text = logWindow.text.Insert (0, "value "+ ReqData + ":" + ReqDataContents + "\n \n");
			return  ReqDataContents;
		} else {

			logWindow.text = logWindow.text.Insert (0, "No "+ReqData +" property set on first Controller \n \n");
			return null;
		}

	}

	public void SetSomeCustomDataOnScreen () {
		//create some data
		var customData = new { 
			players = AirConsole.instance.GetControllerDeviceIds ().Count,
			started = false,
		};

		//Set that Data as this device's Custom Device State (this device is the Screen)
		AirConsole.instance.SetCustomDeviceState (customData);

		//Log url to on-screen Console
		logWindow.text = logWindow.text.Insert (0, "Set new Custom data on Screen: " + customData + " \n \n");
	}

	public void SetLevelPropertyInCustomScreenData () {
		//Set a property 'level' in this devie's custom data (this device is the Screen)
		AirConsole.instance.SetCustomDeviceStateProperty ("level", 1);
	}

	public void DisplayAllCustomDataFromScreen () {
		//The screen always has device id 0. That is the only device id you're allowed to hardcode.
		if (AirConsole.instance.GetCustomDeviceState (0) != null) {

			logWindow.text = logWindow.text.Insert (0, " \n");

			// Show json string of entries
			foreach (JToken key in AirConsole.instance.GetCustomDeviceState(0).Children()) {
				logWindow.text = logWindow.text.Insert (0, "Custom Data on Screen: " + key + " \n");
			}
		}
	}

	public void DisplayNumberOfConnectedControllers () {
		//This does not count devices that have been connected and then left,
		//only devices that are still active
		int numberOfActiveControllers = AirConsole.instance.GetControllerDeviceIds ().Count;
		logWindow.text = logWindow.text.Insert (0, "Number of Active Controllers: " + numberOfActiveControllers + "\n \n");
	}

	public void SetActivePlayers () {
		//Set the currently connected devices as the active players (assigning them a player number)
		AirConsole.instance.SetActivePlayers ();

		//Log to on-screen Console
		logWindow.text = logWindow.text.Insert (0, "Active Players were set \n \n");
	}

	public void DisplayDeviceIDOfPlayerOne () {

		int device_id = AirConsole.instance.ConvertPlayerNumberToDeviceId (0);

		//Log to on-screen Console
		if (device_id != -1) {
			logWindow.text = logWindow.text.Insert (0, "Player #1 has device ID: " + device_id + " \n \n");
		} else {
			logWindow.text = logWindow.text.Insert (0, "There is no active player # 1 - Set Active Players first!\n \n");
		}
	}

	public void DisplayServerTime () {
		//Get the Server Time
		float time = AirConsole.instance.GetServerTime ();
		
		//Log to on-screen Console
		logWindow.text = logWindow.text.Insert (0, "Server Time: " + time + "\n \n");
	}

	public void DisplayIfFirstContrllerIsLoggedIn () {
		//Get the Device Id
		int idOfFirstController = AirConsole.instance.GetControllerDeviceIds () [0];

		bool firstPlayerLoginStatus = AirConsole.instance.IsUserLoggedIn (idOfFirstController);
		
		//Log to on-screen Console
		logWindow.text = logWindow.text.Insert (0, "First Player is logged in: " + firstPlayerLoginStatus + "\n \n");
	}

	public void HideDefaultUI () {
		//Hide the Default UI in the Browser Window
		AirConsole.instance.ShowDefaultUI (false);

		//Log to on-screen Console
		logWindow.text = logWindow.text.Insert (0, "Hid Default UI" + "\n \n");
	}

	public void ShowDefaultUI () {
		//Show the Default UI in the Browser Window
		AirConsole.instance.ShowDefaultUI (true);

		//Log to on-screen Console
		logWindow.text = logWindow.text.Insert (0, "Showed Default UI" + "\n \n");
	}


	void OnDestroy () {

		// unregister events
		if (AirConsole.instance != null) {
			AirConsole.instance.onReady -= OnReady;
			AirConsole.instance.onMessage -= OnMessage;
			AirConsole.instance.onConnect -= OnConnect;
			AirConsole.instance.onDisconnect -= OnDisconnect;
			AirConsole.instance.onDeviceStateChange -= OnDeviceStateChange;
			AirConsole.instance.onCustomDeviceStateChange -= OnCustomDeviceStateChange;
			AirConsole.instance.onGameEnd -= OnGameEnd;
		}
	}
}

