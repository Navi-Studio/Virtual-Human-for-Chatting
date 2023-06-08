using UnityEngine;
using System.Collections;
using OpenCvSharp;
using System;
using System.Runtime.InteropServices;

namespace OpenCvSharp {

	public class Unity {

		/// <summary>
		/// Converts pixel buffer into a raw OpenCV Mat pointer
		/// </summary>
		/// <param name="pixels32">Source buffer, 32-bit RGBA image is expected</param>
		/// <param name="w">Source width</param>
		/// <param name="h">Source height</param>
		/// <param name="flipVetically">True to flip vertically</param>
		/// <param name="flipHorizontally">True to flip horizontally</param>
		/// <param name="rotationAngle">Rotation angle, must be exactly in { 0, 90, 180, 270 } set</param>
		/// <returns>cv::Mat pointer</returns>
		[DllImport(NativeMethods.DllExtern, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr utils_texture_to_mat(IntPtr pixels32, int w, int h, [MarshalAs(UnmanagedType.I1)] bool flipVetically, [MarshalAs(UnmanagedType.I1)] bool flipHorizontally, int rotationAngle);

		/// <summary>
		/// Converts cv::Mat to the pixels buffer
		/// </summary>
		/// <param name="mat">Raw cv::Mat pointer</param>
		/// <returns>Raw cv::Mat pointer thta holds image formatted for Unity</returns>
		[DllImport(NativeMethods.DllExtern, CallingConvention = CallingConvention.Cdecl)]
		private static extern IntPtr utils_mat_to_texture_2(IntPtr mat);
		
		/// <summary>
		/// Data struct to hold raw image data
		/// </summary>
		[StructLayout(LayoutKind.Explicit)]
		private struct Color32Bytes
		{
			[FieldOffset(0)]
			public byte[] byteArray;
			[FieldOffset(0)]
			public Color32[] colors;
		}

		/// <summary>
		/// Aux. class, holds conversion params data
		/// </summary>
		public class TextureConversionParams
		{
			/// <summary>
			/// Vertical flip
			/// </summary>
			public bool FlipVertically { get; set; }

			/// <summary>
			/// Horizontal flip
			/// </summary>
			public bool FlipHorizontally { get; set; }

			/// <summary>
			/// Rotation angle, counter-clock-wise, muts be in { 0, 90, 180, 270 } set
			/// </summary>
			public int RotationAngle { get; set; }

			/// <summary>
			/// Default constructor
			/// </summary>
			public TextureConversionParams()
			{
				FlipVertically = false;
				FlipHorizontally = false;
				RotationAngle = 0;
			}

			/// <summary>
			/// Default value
			/// </summary>
			public static readonly TextureConversionParams Default = new TextureConversionParams() { FlipVertically = false, FlipHorizontally = false, RotationAngle = 0 };
		}

		/// <summary>
		/// Converts Unity Texture2D to OpenCV Mat
		/// </summary>
		/// <returns>New mat</returns>
		/// <param name="texture">Unity texture</param>
		/// <param name="parameters">Conversion parameters</param>
		public static Mat TextureToMat(Texture2D texture, TextureConversionParams parameters = null)
		{
			if (null == parameters)
				parameters = TextureConversionParams.Default;

			Color32[] pixels32 = texture.GetPixels32 ();
			return PixelsToMat(pixels32, texture.width, texture.height, parameters.FlipVertically, parameters.FlipHorizontally, parameters.RotationAngle);
		}

		/// <summary>
		/// Converts Unity Texture2D to OpenCV Mat
		/// </summary>
		/// <returns>New mat</returns>
		/// <param name="texture">Unity texture</param>
		/// <param name="parameters">Conversion parameters</param>
		public static Mat TextureToMat(WebCamTexture texture, TextureConversionParams parameters = null)
		{
			if (null == parameters)
				parameters = TextureConversionParams.Default;

			Color32[] pixels32 = texture.GetPixels32();
			return PixelsToMat(pixels32, texture.width, texture.height, parameters.FlipVertically, parameters.FlipHorizontally, parameters.RotationAngle);
		}

		/// <summary>
		/// Converts Unity Texture2d to OpenCV Mat
		/// </summary>
		/// <param name="pixels32">Source buffer with RGBA texture (8 bits per channel, 32 bits total)</param>
		/// <param name="width">Input buffer width</param>
		/// <param name="height">Input buffer height</param>
		/// <param name="flipVetically">True if OpenCV image should flip source vertically </param>
		/// <param name="flipHorizontally">True if OpenCV image should flip source horizontally</param>
		/// <param name="rotationAngle">Source buffer rotation angle (for camera-sourced images), must be in { 0, 90, 180, 270 } set</param>
		/// <returns>OpenCvSharp.Mat object holding image in OpenCV format</returns>
		public static Mat PixelsToMat(Color32[] pixels32, int width, int height, bool flipVertically, bool flipHorizontally, int rotationAngle) {
			// make sure angle is defined correctly
			if (0 != rotationAngle && 90 != rotationAngle && 180 != rotationAngle && 270 != rotationAngle)
				throw new ArgumentException(string.Format("OpenCvSharp.PixelsToMat: rotationAngle argument = {0}, is not in ( 0, 90, 180, 270 ) set", rotationAngle));

			GCHandle gcHandle = GCHandle.Alloc(pixels32, GCHandleType.Pinned);

			// NO mistake here - we negate flipVertically as it's necessary due to Unity and OpenCV storing images differently and Unity's texture is always
			// vertically flipped for OpenCV. So, this trick allows to avoid user headache about the matter, leaving him thinking about
			// his transforms only
			IntPtr matPtr = utils_texture_to_mat(gcHandle.AddrOfPinnedObject (), width, height, !flipVertically, flipHorizontally, rotationAngle);
			Mat mat = new Mat(matPtr);
			gcHandle.Free ();
			return mat;
		}
		
		/// <summary>
		/// Converts OpenCV Mat to Unity texture
		/// </summary>
		/// <returns>Unity texture</returns>
		/// <param name="mat">OpenCV Mat</param>
		/// <param name="outTexture">Unity texture to set pixels</param>
		public static Texture2D MatToTexture(Mat mat, Texture2D outTexture = null)
		{
			Size size = mat.Size();
			using (Mat unityMat = new Mat(utils_mat_to_texture_2(mat.CvPtr)))
			{
				if (null == outTexture || outTexture.width != size.Width || outTexture.height != size.Height)
					outTexture = new Texture2D(size.Width, size.Height);

				int count = size.Width * size.Height;
				Color32Bytes data = new Color32Bytes();
				data.byteArray = new byte[count * 4];
				data.colors = new Color32[count];
				Marshal.Copy(unityMat.Data, data.byteArray, 0, data.byteArray.Length);
				outTexture.SetPixels32(data.colors);
				outTexture.Apply();

				return outTexture;
			}
		}


	}
}
