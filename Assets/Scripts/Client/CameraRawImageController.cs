using KompasCore.Cards;
using UnityEngine;
using UnityEngine.UI;

namespace KompasClient.UI
{
	public abstract class CameraRawImageController : MonoBehaviour
	{
		public Camera gridCamera;
		public RawImage rawImage;
		public RectTransform textureRectTransform;

		private void Update()
		{
			DoPointerStuff();
		}

		private void DoPointerStuff()
		{
			var viewportClick = ViewportClick(textureRectTransform);

			//I have a special layer for the objects I want to detect with my ray

			//I cast the ray from the camera which rends the texture
			Ray ray = gridCamera.ViewportPointToRay(new Vector3(viewportClick.x, viewportClick.y, 0));
			//Debug.Log($"Mouse at {position}, net {localClickUnnorm}, Normalized to {localClick}, ray {ray}");

			if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity))
			{
				var cardCtrl = hit.collider.gameObject.GetComponent<CardMouseController>();
				//Debug.Log($"{hit.collider.gameObject.name}, card control? {cardCtrl}");
				if (cardCtrl == null) return;
				if (Input.GetMouseButtonUp(0)) cardCtrl.OnMouseUp();
				else cardCtrl.OnMouseOver();
			}
		}

		public static Vector3 ViewportClick(RectTransform rectTransform)
		{
			var position = Input.mousePosition;
			//I get the point of the RawImage where I click
			RectTransformUtility.ScreenPointToLocalPointInRectangle(rectTransform, position, null, out Vector2 localClick);
			var localClickUnnorm = new Vector2(localClick.x, localClick.y);
			//My RawImage is 700x700 and the click coordinates are in range (-350,350) so I transform it to (0,700) to then normalize
			localClick.x = (rectTransform.rect.xMin * -1) - (localClick.x * -1);
			localClick.y = (rectTransform.rect.yMin * -1) - (localClick.y * -1);

			//I normalize the click coordinates so I get the viewport point to cast a Ray
			Vector2 viewportClick = new Vector2(localClick.x / rectTransform.rect.size.x, localClick.y / rectTransform.rect.size.y);
			return viewportClick;
		}

		public static Vector3 ViewportPosition(Camera camera, RectTransform rectTransform)
		{
			var viewportPos = camera.ViewportToScreenPoint(ViewportClick(rectTransform));
			//Debug.Log($"{localClickUnnorm}, {localClick} / {clientSidebarRectTransform.rect.size}, {viewportClick}, {viewportPos}");
			return viewportPos;
		}
	}
}