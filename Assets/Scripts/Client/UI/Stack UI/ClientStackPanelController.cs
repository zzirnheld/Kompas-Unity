using System.Collections.Generic;
using UnityEngine;
using KompasClient.Effects;

namespace KompasClient.UI
{
    public class ClientStackPanelController : MonoBehaviour
    {
        public Transform contentParent;
        public GameObject stackPanelElementPrefab;

        private readonly List<ClientStackPanelElementController> stack = new List<ClientStackPanelElementController>();

        public void Add(IClientStackable stackable)
        {
            if (stack.Count == 0) gameObject.SetActive(true);

            var element = Instantiate(stackPanelElementPrefab, parent: contentParent)
                .GetComponent<ClientStackPanelElementController>();
            element.Initialize(stackable);
            stack.Add(element);
        }

        public void Remove(int index)
        {
            if (stack.Count > index)
            {
                Destroy(stack[index].gameObject);
                stack.RemoveAt(index);
                if (stack.Count == 0) gameObject.SetActive(false);
            }
            else Debug.LogError($"Client cannot remove stack panel element at {index}");
        }
    }
}