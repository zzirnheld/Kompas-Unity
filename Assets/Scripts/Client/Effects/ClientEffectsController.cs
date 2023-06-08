using KompasCore.Effects;
using KompasClient.UI;
using UnityEngine;
using System.Collections.Generic;

namespace KompasClient.Effects
{
	public class ClientEffectsController : MonoBehaviour
	{
		public ClientStackPanelController clientStackPanelCtrl;

		private readonly EffectStack<IClientStackable> stack = new EffectStack<IClientStackable>();

		public IEnumerable<IClientStackable> StackEntries => stack.StackEntries;

		public void Add(IClientStackable stackable, ResolutionContext context = default)
		{
			stack.Push((stackable, context));
			clientStackPanelCtrl.Add(stackable);
		}

		public void Remove(int index)
		{
			try
			{
				stack.Cancel(index);
			}
			catch (System.ArgumentOutOfRangeException)
			{
				index = stack.Count - 1;
				stack.Cancel(index);
			}
			clientStackPanelCtrl.Remove(index);
		}
	}
}