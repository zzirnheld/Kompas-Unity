using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.UI
{
    public class StackableEntitiesController : MonoBehaviour
    {
        [Header("Offsets")]
        [Tooltip("Between objects while collapsed")]
        public Vector3 collapsedOffset;
        [Tooltip("Between objects while expanded")]
        public Vector3 expandedOffset;
        [Tooltip("Offset for the entire from parent while it's expanded")]
        public float whileExpandedOffset;

        [Header("Collider attributes")]
        public int layer;
        public float colliderPadding = 1.1f;
        public BoxCollider boxCollider;

        protected virtual IEnumerable<GameObject> Objects { get; private set; } = null;

        private bool collapsed = true;
        private Vector3 currOffset = Vector3.zero;

        private int LayerMask => 1 << layer;
        private Vector3 LocalMeToCamera => transform.InverseTransformVector(Camera.main.transform.position - transform.position).normalized;

        public void Initalize(ICollection<GameObject> objects)
        {
            if (Objects != null) throw new System.ArgumentException("Tried to initialized a StackableEntitiesController that was already initialized!");

            Objects = new List<GameObject>(objects);
            Collapse();
        }

        public void Refresh()
        {
            if (collapsed) Collapse();
            else Expand();
        }

        public void Collapse()
        {
            if (Objects == null) throw new System.ArgumentException("Tried to collapse a StackableEntitiesController that wasn't initialized!");
            OffsetSelf(0f);
            ShowCollapsed();
            UpdateColliders();
            collapsed = true;
        }

        public void Expand()
        {
            if (Objects == null) throw new System.ArgumentException("Tried to expand a StackableEntitiesController that wasn't initialized!");
            OffsetSelf(whileExpandedOffset);
            ShowExpanded();
            UpdateColliders();
            collapsed = false;
        }

        private void OffsetSelf(float offset)
        {
            transform.localPosition -= currOffset;
            currOffset = LocalMeToCamera * offset;
            transform.localPosition += currOffset;
        }

        protected virtual void ShowCollapsed() => ShowWithOffset(collapsedOffset);

        protected virtual void ShowExpanded() => ShowWithOffset(expandedOffset);

        private void ShowWithOffset(Vector3 offset)
        {
            foreach (var (index, obj) in Objects.Enumerate())
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
                Debug.Log($"encapsualting {bounds}");
                foreach (var rend in renderers) bounds.Encapsulate(rend.bounds);
                return bounds;
            }
            else return new Bounds();
        }

        private void UpdateColliders()
        {
            bool firstLoop = true;
            Bounds totalBounds = default;

            foreach (GameObject go in Objects)
            {
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
        }
        #endregion collider

        private void Update()
        {
            bool success = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, LayerMask)
                && hit.collider == boxCollider;

            if (success && collapsed) Expand();
            else if (!success && !collapsed) Collapse();
        }
    }
}