using KompasCore.Helpers;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.UI
{
	public class StackableEntitiesController : MonoBehaviour
	{
		[Header("Offsets")]
		[Tooltip("Constant offset between objects while collapsed")]
		public Vector3 collapsedOffset;
		[Tooltip("Constant offset between objects while expanded")]
		public Vector3 expandedOffset;
		[Tooltip("Constant offset for the entire from parent while it's expanded (towards the camera)")]
		public float whileExpandedOffset;

		[Header("Collider attributes")]
		public int layer;
		[Tooltip("Multiplier for the boxcollider's bounds. Should be 1/local scale (consider determining automatically...)")]
		public float colliderPadding = 1.1f;
		public BoxCollider boxCollider;

		public Transform behind;
		public float behindBoundsMultiplier;

		private IReadOnlyCollection<GameObject> objects;
		public virtual IEnumerable<GameObject> ShownObjects
		{
			get => objects;
			set {
				if (objects != null)
				{
					bool wasCollapsed = collapsed;
					Collapse();
					collapsed = wasCollapsed;
				}
				objects = new List<GameObject>(value);
				Refresh();
			}
		}

		private bool collapsed = true;
		private Vector3 currOffset = Vector3.zero;

		private int LayerMask => 1 << layer;
		private Vector3 LocalMeToCamera => (Camera.main.transform.position - transform.position).normalized;

		protected virtual bool ForceExpand => false;
		protected virtual bool ForceCollapse => false;

        protected virtual IEnumerable<GameObject> AdditionalGameObjects => Enumerable.Empty<GameObject>();

        protected void Update()
		{
			if (ForceExpand)
			{
				if (collapsed) Expand();
				return;
			}
			else if (ForceCollapse)
			{
				if (!collapsed) Collapse();
				return;
			}

			bool success = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, LayerMask)
				&& hit.collider == boxCollider;

			//if (success) Debug.Log($"Hit {this.GetType()} with raycast");

			if (success && collapsed) Expand();
			else if (!success && !collapsed) Collapse();
		}

		public void Refresh()
		{
			//Debug.Log($"Refreshing {name}");
			if (collapsed) Collapse();
			else Expand();
		}

		public void Collapse()
		{
			//Debug.Log($"Collapsing {name}");
			if (ShownObjects == null) throw new System.ArgumentException("Tried to collapse a StackableEntitiesController that wasn't initialized!");
			OffsetSelf(0f);
			ShowCollapsed();
			UpdateColliders(false);
			collapsed = true;
		}

		public void Expand()
		{
			//Debug.Log($"Expanding {name}");
			if (ShownObjects == null) throw new System.ArgumentException("Tried to expand a StackableEntitiesController that wasn't initialized!");
			OffsetSelf(whileExpandedOffset);
			ShowExpanded();
			UpdateColliders(true);
			collapsed = false;
		}

		private void OffsetSelf(float towardsCameraOffset)
		{
			transform.position -= currOffset;
			currOffset = LocalMeToCamera * towardsCameraOffset;
			if (ShownObjects.Count() > 0 && towardsCameraOffset != 0) currOffset += ShownObjects.Last().transform.position - transform.position;
			transform.position += currOffset;
		}

		protected virtual void ShowCollapsed() => ShowWithOffset(collapsedOffset);

		protected virtual void ShowExpanded() => ShowWithOffset(expandedOffset);

		private void ShowWithOffset(Vector3 offset)
		{
			foreach (var (index, obj) in ShownObjects.Enumerate())
			{
				obj.transform.localPosition = index * offset;
			}
		}

		#region collider
		private Bounds GetChildRendererBounds(GameObject go)
		{
			var renderers = go.GetComponentsInChildren<Renderer>();

			if (renderers.Length > 0)
			{
				Bounds bounds = renderers[0].bounds;
				//Debug.Log($"encapsualting {bounds}");
				foreach (var rend in renderers) bounds.Encapsulate(rend.bounds);
				return bounds;
			}
			else return new Bounds();
		}

		private void UpdateColliders(bool showBehind)
		{
			bool firstLoop = true;
			Bounds totalBounds = default;

			foreach (GameObject go in ShownObjects)
			{
				var bounds = GetChildRendererBounds(go);
				if (firstLoop)
				{
					totalBounds = bounds;
					firstLoop = false;
				}
				totalBounds.Encapsulate(bounds);
			}

			foreach (GameObject go in AdditionalGameObjects)
			{
				if (go == null) continue;
                var bounds = GetChildRendererBounds(go);
				if (firstLoop)
				{
					totalBounds = bounds;
					firstLoop = false;
				}
				totalBounds.Encapsulate(bounds);
			}

			boxCollider.size = totalBounds.size * colliderPadding;
			boxCollider.center = transform.InverseTransformPoint(totalBounds.center) + (0.5f * Vector3.down);

			if (behind != null)
			{
				behind.gameObject.SetActive(showBehind);
				if (showBehind)
				{
					behind.localScale = totalBounds.extents * behindBoundsMultiplier;
					behind.position = totalBounds.center;
					behind.Translate(Vector3.down * 0.15f);
				}
			}
		}
		#endregion collider
	}
}