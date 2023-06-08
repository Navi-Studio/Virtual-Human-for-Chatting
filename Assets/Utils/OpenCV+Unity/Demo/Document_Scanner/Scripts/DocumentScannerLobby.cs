namespace OpenCvSharp.Demo
{
	using UnityEngine;
	using UnityEngine.UI;
	using System.Collections;
	using UnityEngine.SceneManagement;

	public class DocumentScannerLobby : MonoBehaviour {

		// Use this for initialization
		void Awake () {
			SceneManager.LoadScene("DocumentScannerScene", LoadSceneMode.Additive);
		}

		void OnDestroy() {
			SceneManager.UnloadScene ("DocumentScannerScene");
		}

		public void OnButton(string name) {
			NavigateTo (name);
		}

		private void NavigateTo(string name) {
			Scene scene = SceneManager.GetSceneByName ("DocumentScannerScene");
			if (scene.isLoaded) {
				DocumentScannerScript script = Object.FindObjectOfType<DocumentScannerScript>();
				script.Process(name);

//				GameObject gameObject = GameObject.Find("OutputImage");
//				RawImage rawImage = gameObject.GetComponent<RawImage> ();
//				rawImage.texture = (Texture2D)Resources.Load("Letter");

				//GameObject[] gameObjects = scene.GetRootGameObjects ();
				//Debug.Log (gameObjects.Length);
			}
		}

	
	}

}