using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;

public class LobbyScript: MonoBehaviour {
	private string currentScene = null;
	public GameObject mainMenu;
	public GameObject backMenu;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void OnFaceDetectorButton() {
		NavigateTo("FaceDetectorScene");
	}

	public void OnFaceRecognizerButton()
	{
		NavigateTo("FaceRecognizer");
	}

	public void OnGrayscaleButton() {
		NavigateTo("GrayscaleScene");
	}
	
	public void OnContoursByShapeScenerButton() {
		NavigateTo("ContoursByShapeScene");
	}

	public void OnLiveSketchButton() {
		NavigateTo("LiveSketchScene");
	}

	public void OnMarkerDetectorButton() {
		NavigateTo("MarkerDetectorScene");
	}

	public void OnDocumenScannerButton()
	{
		NavigateTo("DocumentScannerLobby");
	}

	public void OnAlphabetOCRButton()
	{
		NavigateTo("AlphabetOCR");
	}

	public void OnTrackingButton()
	{
		NavigateTo("TrackingScene");
	}

	public void OnBackButton() {
		UnloadScene ();
		mainMenu.SetActive (true);
		backMenu.SetActive (false);
	}

	private void UnloadScene() {
		if (currentScene != null) {
			SceneManager.UnloadScene (currentScene);
			currentScene = null;
		}
	}

	private void NavigateTo(string sceneName) {
		UnloadScene ();
		currentScene = sceneName;
		mainMenu.SetActive (false);
		backMenu.SetActive (true);
		SceneManager.LoadScene(sceneName, LoadSceneMode.Additive);
	}
}
