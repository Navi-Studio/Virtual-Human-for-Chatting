namespace OpenCvSharp.Demo
{
	using System.Collections.Generic;
	using OpenCvSharp;
	using UnityEngine.UI;

	public class AlphabetOCRScene : UnityEngine.MonoBehaviour
	{
		public UnityEngine.Texture2D texture;
		public UnityEngine.TextAsset model;
		
		void Start()
		{
			// some constants for drawing
			const int textPadding = 2;
			const HersheyFonts textFontFace = HersheyFonts.HersheyPlain;
			double textFontScale = System.Math.Max(this.texture.width, this.texture.height) / 512.0;
			Scalar boxColor = Scalar.DeepPink;
			Scalar textColor = Scalar.White;

			// load alphabet
			AlphabetOCR alphabet = new AlphabetOCR(model.bytes);

			// scan image
			var image = Unity.TextureToMat(this.texture);
			IList<AlphabetOCR.RecognizedLetter> letters = alphabet.ProcessImage(image);
			foreach (var letter in letters)
			{
				int line;
				var bounds = Cv2.BoundingRect(letter.Rect);

				// text box.
				var textData = string.Format("{0}: {1}%", letter.Data, System.Math.Round(letter.Confidence * 100));
				var textSize = Cv2.GetTextSize(textData, textFontFace, textFontScale, 1, out line);
				var textBox = new Rect(
					bounds.X + (bounds.Width - textSize.Width) / 2 - textPadding,
					bounds.Bottom,
					textSize.Width + textPadding * 2,
					textSize.Height + textPadding * 2
				);

				// draw shape
				image.Rectangle(bounds, boxColor, 2);
				image.Rectangle(textBox, boxColor, -1);
				image.PutText(textData, textBox.TopLeft + new Point(textPadding, textPadding + textSize.Height), textFontFace, textFontScale, textColor, (int)(textFontScale + 0.5));
			}

			// result
			UnityEngine.Texture2D texture = Unity.MatToTexture(image);

			// output
			RawImage rawImage = gameObject.GetComponent<RawImage>();
			rawImage.texture = texture;

			var transform = gameObject.GetComponent<UnityEngine.RectTransform>();
			transform.sizeDelta = new UnityEngine.Vector2(image.Width, image.Height);
		}

		// Update is called once per frame
		void Update()
		{
		}
	}
}