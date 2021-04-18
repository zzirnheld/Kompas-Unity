using KompasCore.Effects;
using KompasClient.UI;
using UnityEngine;

namespace KompasClient.Effects
{
    public class ClientEffectsController : MonoBehaviour
    {
        public ClientStackPanelController clientStackPanelCtrl;

        private readonly EffectStack<IStackable> stack = new EffectStack<IStackable>();

        public void Add(IStackable stackable, ActivationContext context = default)
        {
            stack.Push((stackable, context));
        }

        public void Remove(int index)
        {
            stack.Cancel(index);
        }
    }
}