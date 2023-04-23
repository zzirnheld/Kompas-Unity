using UnityEngine;

namespace KompasClient.UI
{

	public class HandCameraController : CameraRawImageController
	{
		public static HandCameraController Main { get; private set; }


		private void Awake()
		{
			Main = this;
			Debug.Log($"rawImage {textureRectTransform.rect}");
			var renderTexture = new RenderTexture((int)textureRectTransform.rect.width * 2, (int)textureRectTransform.rect.height * 2, 24);
			gridCamera.targetTexture = renderTexture;
			rawImage.texture = renderTexture;

            gridCamera.gameObject.transform.localPosition = 2f * Vector3.up;
        }
	}
}