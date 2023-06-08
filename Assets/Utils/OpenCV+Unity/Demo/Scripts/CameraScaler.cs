namespace OpenCvSharp.Demo
{
	using UnityEngine;
	using System.Collections;
	
	/// <summary>
	/// This script scale surface with WebCameraTexuture to match the screen size
	/// </summary>
	public class CameraScaler : MonoBehaviour {
		private Vector2 ScreenSize {
			get;
			set;
		}

		private Vector2 ComponentSize {
			get;
			set;
		}

		void Start () {
			ScreenSize = Vector2.zero;
			ComponentSize = Vector2.zero;
			Update ();
		}
		
		void Update () {
			Vector2 compontSize = this.GetComponent<RectTransform>().sizeDelta;
			if (Screen.width != ScreenSize.x || Screen.height != ScreenSize.y || compontSize.x != ComponentSize.x || compontSize.y != ComponentSize.y) {
				ScreenSize = new Vector2 (Screen.width, Screen.height);
				ComponentSize = compontSize;

				Scale();
			}
		}

		void Scale() {
			float width = ComponentSize.x;
			float height = ComponentSize.y;
			if (width <= 0 || height <= 0 || Screen.width <= 0 || Screen.height <= 0) {
				return;
			}
	
			float aspect = Mathf.Min(Screen.height / height, Screen.width / width);
			this.transform.localScale = new Vector3 (aspect, aspect, 1.0f);
		}
	}
}