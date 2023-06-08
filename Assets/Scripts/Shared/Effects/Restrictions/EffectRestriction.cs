using System.Linq;

namespace KompasCore.Effects.Restrictions
{
	public class EffectRestriction : ContextInitializeableBase
	{
		public IEffectRestrictionElement[] restrictionElements = { };

		public bool Evaluate(Effect effect)
		{
			ComplainIfNotInitialized();
			return restrictionElements.All(re => re.IsValidStackable(effect));
		}
	}

	public interface IEffectRestrictionElement
	{
		public bool IsValidStackable(IStackable stackable);
	}

	public abstract class EffectRestrictionElementBase : StackableRestrictionElementBase,
		IEffectRestrictionElement
	{
		protected override bool AbstractIsValidStackable(IStackable stackable)
		{
			return stackable is Effect effect && AbstractIsValidEffect(effect);
		}

		protected abstract bool AbstractIsValidEffect(Effect effect);
	}

	namespace EffectRestrictionElements
	{
		public class Keyword : EffectRestrictionElementBase
		{
			public string keyword;

			protected override bool AbstractIsValidEffect(Effect effect)
				=> effect.Keyword == keyword;
		}
	}
}