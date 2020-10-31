using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KompasCore.Effects;

namespace KompasClient.UI
{
    public class ClientStackPanelController : MonoBehaviour
    {
        public Transform contentParent;
        public GameObject stackPanelElementPrefab;

        private readonly List<ClientStackPanelElementController> stack = new List<ClientStackPanelElementController>();

        public void Add(Sprite image, string cardName, string blurb)
        {
            if (stack.Count == 0) gameObject.SetActive(true);

            var element = Instantiate(stackPanelElementPrefab, parent: contentParent)
                .GetComponent<ClientStackPanelElementController>();
            element.Initialize(image, cardName, blurb);
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