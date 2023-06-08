namespace OpenCvSharp.Demo
{
	using System;
	using UnityEngine;
	using UnityEngine.UI;
	using OpenCvSharp;

	// Many ideas are taken from http://answers.unity3d.com/questions/773464/webcamtexture-correct-resolution-and-ratio.html#answer-1155328

	/// <summary>
	/// Base WebCamera class that takes care about video capturing.
	/// Is intended to be sub-classed and partially overridden to get
	/// desired behavior in the user Unity script
	/// </summary>
	public abstract class WebCamera: MonoBehaviour
	{
		/// <summary>
		/// Target surface to render WebCam stream
		/// </summary>
		public GameObject Surface;

		private Nullable<WebCamDevice> webCamDevice = null;
		private WebCamTexture webCamTexture = null;
		private Texture2D renderedTexture = null;

		/// <summary>
		/// A kind of workaround for macOS issue: MacBook doesn't state it's webcam as frontal
		/// </summary>
		protected bool forceFrontalCamera = false;

		/// <summary>
		/// WebCam texture parameters to compensate rotations, flips etc.
		/// </summary>
		protected Unity.TextureConversionParams TextureParameters { get; private set; }

		/// <summary>
		/// Camera device name, full list can be taken from WebCamTextures.devices enumerator
		/// </summary>
		public string DeviceName
		{
			get
			{
				return (webCamDevice != null) ? webCamDevice.Value.name : null;
			}
			set
			{
				// quick test
				if (value == DeviceName)
					return;

				if (null != webCamTexture && webCamTexture.isPlaying)
					webCamTexture.Stop();

				// get device index
				int cameraIndex = -1;
				for (int i = 0; i < WebCamTexture.devices.Length && -1 == cameraIndex; i++)
				{
					if (WebCamTexture.devices[i].name == value)
						cameraIndex = i;
				}

				// set device up
				if (-1 != cameraIndex)
				{
					webCamDevice = WebCamTexture.devices[cameraIndex];
					webCamTexture = new WebCamTexture(webCamDevice.Value.name);

					// read device params and make conversion map
					ReadTextureConversionParameters();

					webCamTexture.Play();
				}
				else
				{
					throw new ArgumentException(String.Format("{0}: provided DeviceName is not correct device identifier", this.GetType().Name));
				}
			}
		}

		/// <summary>
		/// This method scans source device params (flip, rotation, front-camera status etc.) and
		/// prepares TextureConversionParameters that will compensate all that stuff for OpenCV
		/// </summary>
		private void ReadTextureConversionParameters()
		{
			Unity.TextureConversionParams parameters = new Unity.TextureConversionParams();

			// frontal camera - we must flip around Y axis to make it mirror-like
			parameters.FlipHorizontally = forceFrontalCamera || webCamDevice.Value.isFrontFacing;
			
			// TODO:
			// actually, code below should work, however, on our devices tests every device except iPad
			// returned "false", iPad said "true" but the texture wasn't actually flipped

			// compensate vertical flip
			//parameters.FlipVertically = webCamTexture.videoVerticallyMirrored;
			
			// deal with rotation
			if (0 != webCamTexture.videoRotationAngle)
				parameters.RotationAngle = webCamTexture.videoRotationAngle; // cw -> ccw

			// apply
			TextureParameters = parameters;

			//UnityEngine.Debug.Log (string.Format("front = {0}, vertMirrored = {1}, angle = {2}", webCamDevice.isFrontFacing, webCamTexture.videoVerticallyMirrored, webCamTexture.videoRotationAngle));
		}

		/// <summary>
		/// Default initializer for MonoBehavior sub-classes
		/// </summary>
		protected virtual void Awake()
		{
			if (WebCamTexture.devices.Length > 0)
				DeviceName = WebCamTexture.devices[WebCamTexture.devices.Length - 1].name;
		}

		void OnDestroy() 
		{
			if (webCamTexture != null)
			{
				if (webCamTexture.isPlaying)
				{
					webCamTexture.Stop();
				}
				webCamTexture = null;
			}

			if (webCamDevice != null) 
			{
				webCamDevice = null;
			}
		}

		/// <summary>
		/// Updates web camera texture
		/// </summary>
		private void Update ()
		{
			if (webCamTexture != null && webCamTexture.didUpdateThisFrame)
			{
				// this must be called continuously
				ReadTextureConversionParameters();

				// process texture with whatever method sub-class might have in mind
				if (ProcessTexture(webCamTexture, ref renderedTexture))
				{
					RenderFrame();
				}
			}
		}

		/// <summary>
		/// Processes current texture
		/// This function is intended to be overridden by sub-classes
		/// </summary>
		/// <param name="input">Input WebCamTexture object</param>
		/// <param name="output">Output Texture2D object</param>
		/// <returns>True if anything has been processed, false if output didn't change</returns>
		protected abstract bool ProcessTexture(WebCamTexture input, ref Texture2D output);

		/// <summary>
		/// Renders frame onto the surface
		/// </summary>
		private void RenderFrame()
		{
			if (renderedTexture != null)
			{
				// apply
				Surface.GetComponent<RawImage>().texture = renderedTexture;

				// Adjust image ration according to the texture sizes 
				Surface.GetComponent<RectTransform>().sizeDelta = new Vector2(renderedTexture.width, renderedTexture.height);
			}
		}
	}
}