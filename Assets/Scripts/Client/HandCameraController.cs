using System.Linq;
using KompasCore.GameCore;
using UnityEngine;

namespace KompasClient.UI
{

	public class HandCameraController : CameraRawImageController
	{
		public static HandCameraController Main { get; private set; }

        public HandController handController;

        private int handCount = -1;

		private void Awake()
		{
			Main = this;
			Debug.Log($"rawImage {textureRectTransform.rect}");
			var renderTexture = new RenderTexture((int)textureRectTransform.rect.width * 2, (int)textureRectTransform.rect.height * 2, 24);
			gridCamera.targetTexture = renderTexture;
			rawImage.texture = renderTexture;

            gridCamera.gameObject.transform.localPosition = 2f * Vector3.up;
        }

		protected override void Update()
		{
            base.Update();
            //if (handController.HandSize != handCount) UpdateCameraHeight();
        }

		private void UpdateCameraHeight()
		{
            Debug.Log("Updating height");
            if (handController.HandSize > handCount) PullCameraUp();
			if (handController.HandSize < handCount) PullCameraDown();
            handCount = handController.HandSize;
        }

        private const int MaxIterations = 10;
        private static readonly Vector3 UpStep = 0.5f * Vector3.up;
        private void PullCameraUp()
		{
            Debug.Log("Updating height up");
            int i = 0;
            while (!IsHandAllVisible())
			{
				if (i++ > MaxIterations) break;
                gridCamera.gameObject.transform.Translate(UpStep);
                Debug.Log("Moving up");
			}
		}

        private static readonly Vector3 DownStep = 0.5f * Vector3.down;
        private const float MinHeight = 2f;
        private void PullCameraDown()
		{
            Debug.Log("Updating height down");
            int i = 0;
			while (IsHandAllVisible())
			{
				if (i++ > MaxIterations) break;
				if (gridCamera.transform.position.y <= MinHeight)
				{
                    gridCamera.transform.position = MinHeight * Vector3.up;
                    return;
                }
                gridCamera.gameObject.transform.Translate(DownStep);
                Debug.Log("Moving down");
            }
            gridCamera.gameObject.transform.Translate(UpStep);
        }

		private bool IsHandAllVisible()
		{
            var frustrumPlanes = GeometryUtility.CalculateFrustumPlanes(gridCamera);
			//Invert the planes because intersect should not be considered valid
            //for (int i = 0; i < frustrumPlanes.Length; i++) frustrumPlanes[i] = frustrumPlanes[i].flipped;

            Bounds bounds = new Bounds();
            foreach(var c in handController.Cards.Select(c => c.CardController.gameCardViewController))
			{
                var boxCollider = c.gameObject.GetComponent<BoxCollider>();
				if (boxCollider == null) continue;
                bounds.Encapsulate(boxCollider.bounds);
			}
            Debug.Log($"Checking if {string.Join(", ", frustrumPlanes)} is within {bounds}");
            return GeometryUtility.TestPlanesAABB(frustrumPlanes, bounds);
        }
    }
}