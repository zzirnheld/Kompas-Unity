using KompasCore.Effects;
using KompasClient.UI;
using UnityEngine;

namespace KompasClient.Effects
{
    public class ClientEffectsController : MonoBehaviour
    {
        public ClientStackPanelController clientStackPanelCtrl;

        private readonly EffectStack<IClientStackable> stack = new EffectStack<IClientStackable>();

        public void Add(IClientStackable stackable, ActivationContext context = default)
        {
            stack.Push((stackable, context));
            clientStackPanelCtrl.Add(stackable.PrimarySprite, stackable.SecondarySprite, stackable.StackableBlurb);
        }

        public void Remove(int index)
        {
            stack.Cancel(index);
            clientStackPanelCtrl.Remove(index);
        }
    }
}