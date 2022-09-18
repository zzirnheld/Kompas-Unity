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

        public int LayerMask => 1 >> layer;

        private List<GameObject> objects = null;

        public void Initalize(ICollection<GameObject> objects)
        {
            if (objects != null) throw new System.ArgumentException("Tried to initialized a StackableEntitiesController that was already initialized!");

            this.objects = new List<GameObject>(objects);
            Collapse();
        }

        public void Collapse()
        {
            if (objects == null) throw new System.ArgumentException("Tried to collapse a StackableEntitiesController that wasn't initialized!");
            ShowWithOffset(collapsedOffset);
        }

        public void Expand()
        {
            if (objects == null) throw new System.ArgumentException("Tried to expand a StackableEntitiesController that wasn't initialized!");
            ShowWithOffset(expandedOffset);
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
                foreach (var rend in renderers) bounds.Encapsulate(rend.bounds);
                return bounds;
            }
            else return new Bounds();
        }

        private void UpdateColliders()
        {
            Bounds totalBounds = new Bounds();

            foreach (GameObject go in objects)
            {
                var bounds = GetChildRendererBounds(go);
                totalBounds.Encapsulate(bounds);
            }

            boxCollider.size = totalBounds.size * colliderPadding;
            boxCollider.center = totalBounds.center;
        }

        private void Update()
        {
            if (Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out RaycastHit hit, Mathf.Infinity, LayerMask)
                && hit.collider == boxCollider)
            {

            }
        }
    }
}