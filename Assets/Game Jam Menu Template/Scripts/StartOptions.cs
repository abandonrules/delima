using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using NDream.AirConsole;
using Newtonsoft.Json.Linq;



public class StartOptions : MonoBehaviour {

	public int sceneToStart = 1;										//Index number in build settings of scene to load if changeScenes is true
	public bool changeScenes;											//If true, load a new scene when Start is pressed, if false, fade out UI and continue in single scene
	public bool changeMusicOnStart;										//Choose whether to continue playing menu music or start a new music clip
	public Renderer profilePicturePlaneRenderer1;
	public Renderer profilePicturePlaneRenderer2;
	public Renderer profilePicturePlaneRenderer3;
	public Renderer profilePicturePlaneRenderer4;
	public Renderer profilePicturePlaneRenderer5;
	public Renderer profilePicturePlaneRenderer6;
	public Text logWindow;
	private bool turnLeft;
	private bool turnRight;




	[HideInInspector] public bool inMainMenu = true;					//If true, pause button disabled in main menu (Cancel in input manager, default escape key)
	[HideInInspector] public Animator animColorFade; 					//Reference to animator which will fade to and from black when starting game.
	[HideInInspector] public Animator animMenuAlpha;					//Reference to animator that will fade out alpha of MenuPanel canvas group
	 public AnimationClip fadeColorAnimationClip;		//Animation clip fading to color (black default) when changing scenes
	[HideInInspector] public AnimationClip fadeAlphaAnimationClip;		//Animation clip fading out UI elements alpha


	private PlayMusic playMusic;										//Reference to PlayMusic script
	private float fastFadeIn = .01f;									//Very short fade time (10 milliseconds) to start playing music immediately without a click/glitch
	private ShowPanels showPanels;										//Reference to ShowPanels script on UI GameObject, to show and hide panels

	void Awake () {
		
		// register events
		AirConsole.instance.onReady += OnReady;
		AirConsole.instance.onMessage += OnMessage;
		AirConsole.instance.onConnect += OnConnect;
		AirConsole.instance.onDisconnect += OnDisconnect;
		AirConsole.instance.onDeviceStateChange += OnDeviceStateChange;
		AirConsole.instance.onCustomDeviceStateChange += OnCustomDeviceStateChange;
		AirConsole.instance.onDeviceProfileChange += OnDeviceProfileChange;
		AirConsole.instance.onAdShow += OnAdShow;
		AirConsole.instance.onAdComplete += OnAdComplete;
		//AirConsole.instance.onGameEnd += OnGameEnd;
		logWindow.text = "Connecting... \n \n";
		//Get a reference to ShowPanels attached to UI object
		showPanels = GetComponent<ShowPanels> ();
		showPanels.ShowMenu ();
		//Get a reference to PlayMusic attached to UI object
		playMusic = GetComponent<PlayMusic> ();
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

		// Rotate the AirConsole Logo to the right
		if ((string)data == "left") {
			turnLeft = true;
			turnRight = false;
		}

		// Rotate the AirConsole Logo to the right
		if ((string)data == "right") {
			turnLeft = false;
			turnRight = true;
		}

		// Stop rotating the AirConsole Logo
		//'stop' is sent when a button on the controller is released
		if ((string)data == "stop") {
			turnLeft = false;
			turnRight = false;
		}
	}

	public void StartButtonClicked()
	{
		//If changeMusicOnStart is true, fade out volume of music group of AudioMixer by calling FadeDown function of PlayMusic, using length of fadeColorAnimationClip as time. 
		//To change fade time, change length of animation "FadeToColor"
		if (changeMusicOnStart) 
		{
			playMusic.FadeDown(fadeColorAnimationClip.length);
		}

		//If changeScenes is true, start fading and change scenes halfway through animation when screen is blocked by FadeImage
		if (changeScenes) 
		{
			//Use invoke to delay calling of LoadDelayed by half the length of fadeColorAnimationClip
			Invoke ("LoadDelayed", fadeColorAnimationClip.length * .5f);

			//Set the trigger of Animator animColorFade to start transition to the FadeToOpaque state.
			animColorFade.SetTrigger ("fade");
		} 

		//If changeScenes is false, call StartGameInScene
		else 
		{
			//Call the StartGameInScene function to start game without loading a new scene.
			StartGameInScene();
		}

	}

	//Once the level has loaded, check if we want to call PlayLevelMusic
	void OnLevelWasLoaded()
	{
		//if changeMusicOnStart is true, call the PlayLevelMusic function of playMusic
		if (changeMusicOnStart)
		{
			playMusic.PlayLevelMusic ();
		}	
	}


	public void LoadDelayed()
	{
		//Pause button now works if escape is pressed since we are no longer in Main menu.
		inMainMenu = false;

		//Hide the main menu UI element
		showPanels.HideMenu ();

		//Load the selected scene, by scene index number in build settings
		SceneManager.LoadScene (sceneToStart);
	}

	public void HideDelayed()
	{
		//Hide the main menu UI element after fading out menu for start game in scene
		showPanels.HideMenu();
	}

	public void StartGameInScene()
	{
		//Pause button now works if escape is pressed since we are no longer in Main menu.
		inMainMenu = false;

		//If changeMusicOnStart is true, fade out volume of music group of AudioMixer by calling FadeDown function of PlayMusic, using length of fadeColorAnimationClip as time. 
		//To change fade time, change length of animation "FadeToColor"
		if (changeMusicOnStart) 
		{
			//Wait until game has started, then play new music
			Invoke ("PlayNewMusic", fadeAlphaAnimationClip.length);
		}
		//Set trigger for animator to start animation fading out Menu UI
		animMenuAlpha.SetTrigger ("fade");
		Invoke("HideDelayed", fadeAlphaAnimationClip.length);
		Debug.Log ("Game started in same scene! Put your game starting stuff here.");
		update_profile ();
	}
	public void update_profile()
	{
		DisplayProfilePicture (0,profilePicturePlaneRenderer1);
		DisplayProfilePicture (1,profilePicturePlaneRenderer2);
		DisplayProfilePicture (2,profilePicturePlaneRenderer3);
		DisplayProfilePicture (3,profilePicturePlaneRenderer4);
		DisplayProfilePicture (4,profilePicturePlaneRenderer5);
		DisplayProfilePicture (5,profilePicturePlaneRenderer6);

	}

	public void PlayNewMusic()
	{
		//Fade up music nearly instantly without a click 
		playMusic.FadeUp (fastFadeIn);
		//Play music clip assigned to mainMusic in PlayMusic script
		playMusic.PlaySelectedMusic (1);
	}
	private IEnumerator DisplayUrlPicture (string url, Renderer profilePicturePlaneRenderer) {
		// Start a download of the given URL
		WWW www = new WWW (url);

		// Wait for download to complete
		yield return www;

		// assign texture
		profilePicturePlaneRenderer.material.mainTexture = www.texture;
		Color color = Color.white;
		color.a = 1;
		profilePicturePlaneRenderer.material.color = color;

		//yield return new WaitForSeconds (3.0f);

		//color.a = 0;
		//profilePicturePlaneRenderer.material.color = color;

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
	void OnConnect (int device_id) {
		//Log to on-screen Console
		update_profile();
		logWindow.text = logWindow.text.Insert (0, "Device: " + device_id + " connected. \n \n");
	}

	void OnDisconnect (int device_id) {
		//Log to on-screen Console
		logWindow.text = logWindow.text.Insert (0, "Device: " + device_id + " disconnected. \n \n");
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
	}

	void OnAdShow () {
		//Log to on-screen Console
		logWindow.text = logWindow.text.Insert (0, "On Ad Show \n \n");
	}

	void OnAdComplete (bool adWasShown) {
		//Log to on-screen Console
		logWindow.text = logWindow.text.Insert (0, "Ad Complete. Ad was shown: " + adWasShown + "\n \n");
	}

}
