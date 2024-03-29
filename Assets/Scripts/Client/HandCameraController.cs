using System.Linq;
using KompasCore.GameCore;
using UnityEngine;

namespace KompasClient.UI
{

	public class HandCameraController : CameraRawImageController
	{
		public static HandCameraController Main { get; private set; }

		public HandController handController;

		private const int MaxIterations = 20;
		private const float Step = 0.25f;
		private const int MinHeight = (int)(2f / Step);
		private int handCount = -1;
		private int handDistance = 0;

		//protected override bool DebugControl => true;

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
			if (handController.HandSize != handCount) UpdateCameraHeight();
		}

		private void UpdateCameraHeight()
		{
			if (handController.HandSize == 0) return;
			Debug.Log("Updating height");
			if (handController.HandSize > handCount) PullCameraUp();
			if (handController.HandSize < handCount) PullCameraDown();
			handCount = handController.HandSize;
		}

		private void PullCameraUp()
		{
			Debug.Log("Updating height up");
			while (!IsHandAllVisible())
			{
				if (handDistance++ > MaxIterations) break;
				SetHeight();
				Debug.Log("Moving up");
			}
		}
		private void PullCameraDown()
		{
			Debug.Log("Updating height down");
			while (IsHandAllVisible())
			{
				if (gridCamera.transform.localPosition.y <= MinHeight)
				{
					handDistance = MinHeight;
					return;
				}

				handDistance--;
				SetHeight();
				Debug.Log("Moving down");
			}
			handDistance++;
			SetHeight();
		}

		private void SetHeight() => gridCamera.gameObject.transform.localPosition = handDistance * Step * Vector3.up;

		private bool IsHandAllVisible()
		{
			Plane[] frustrumPlanes = GeometryUtility.CalculateFrustumPlanes(gridCamera);
			//Invert the planes because intersect should not be considered valid
			//Plane[] invertedPlanes = new Plane[6];
			//for (int i = 0; i < frustrumPlanes.Length; i++) invertedPlanes[i] = frustrumPlanes[i].flipped;

			/*var leftCollider = handController.leftDummy.GetComponent<BoxCollider>();
			var rightCollider = handController.leftDummy.GetComponent<BoxCollider>();
			return GeometryUtility.TestPlanesAABB(frustrumPlanes, leftCollider.bounds)
				&& GeometryUtility.TestPlanesAABB(frustrumPlanes, rightCollider.bounds);*/

			return handController.Cards.Select(c => c.CardController.gameCardViewController)
				.SelectMany(gcvc => gcvc.outsideCardBoxColliders)
				.Select(collider => collider.bounds)
				.All(bounds => GeometryUtility.TestPlanesAABB(frustrumPlanes, bounds));
		}

		/*
		public BoxCollider debugCollider;

		private List<GameObject> planes = new List<GameObject>();

		private void VisualizePlanes(Plane[] frustrumPlanes)
		{
			foreach (var plane in planes) Destroy(plane);
			for (int i = 0; i < 6; ++i)
			{
				GameObject p = GameObject.CreatePrimitive(PrimitiveType.Plane);
				p.name = "Plane " + i.ToString();
				p.transform.position = -frustrumPlanes[i].normal * frustrumPlanes[i].distance;
				p.transform.rotation = Quaternion.FromToRotation(Vector3.up, frustrumPlanes[i].normal);
				p.transform.parent = transform;
				planes.Add(p);
			}
		}*/
	}
}