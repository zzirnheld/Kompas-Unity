using System.Collections.Generic;
using UnityEngine;

namespace KompasCore.UI
{
    public class StackableEntitiesController : MonoBehaviour
    {
        [Header("Item that is going to be stacked")]
        public Vector3 collapsedOffset;
        public Vector3 expandedOffset;
        public int layer;
        public float colliderPadding = 1.1f;
        public BoxCollider boxCollider;

        public int LayerMask => 1 << layer;

        private List<GameObject> objects = null;

        private bool collapsed = false;

        public void Initalize(ICollection<GameObject> objects)
        {
            if (this.objects != null) throw new System.ArgumentException("Tried to initialized a StackableEntitiesController that was already initialized!");

            this.objects = new List<GameObject>(objects);
            Collapse();
        }

        public void Collapse()
        {
            if (objects == null) throw new System.ArgumentException("Tried to collapse a StackableEntitiesController that wasn't initialized!");
            ShowWithOffset(collapsedOffset);
            collapsed = true;
        }

        public void Expand()
        {
            if (objects == null) throw new System.ArgumentException("Tried to expand a StackableEntitiesController that wasn't initialized!");
            ShowWithOffset(expandedOffset);
            collapsed = false;
        }

        private void ShowWithOffset(Vector3 offset)
        {
            foreach (var (index, obj) in objects.Enumerate())
            {
                obj.transform.localPosition = index * offset;
            }
            UpdateColliders();
        }

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

            foreach (GameObject go in objects)
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

        private void Update()
        {
            bool success = Physics.Raycast(Camera.main.ScreenPointToRay(Input.mousePosition), out RaycastHit hit, Mathf.Infinity, LayerMask)
                && hit.collider == boxCollider;
            if (success && collapsed)
            {
                Debug.Log("EXPANDING");
                Expand();
            }
            else if (!success && !collapsed) Collapse();
        }
    }
}