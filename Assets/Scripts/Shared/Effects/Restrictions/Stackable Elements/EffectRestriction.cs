namespace KompasCore.Effects.Restrictions.StackableRestrictionElements
{
	public abstract class EffectRestrictionElementBase : RestrictionBase<IStackable>, IRestriction<Effect>
	{
		public bool IsValid(Effect item, IResolutionContext context) => base.IsValid(item, context);

		protected override bool IsValidLogic(IStackable item, IResolutionContext context)
		{
			return item is Effect effect && IsValidLogic(effect);
		}

		protected abstract bool IsValidLogic(Effect effect);
	}

	namespace EffectRestrictionElements
	{
		public class Keyword : EffectRestrictionElementBase
		{
			public string keyword;

			protected override bool IsValidLogic(Effect effect)
				=> effect.Keyword == keyword;
		}
	}
}