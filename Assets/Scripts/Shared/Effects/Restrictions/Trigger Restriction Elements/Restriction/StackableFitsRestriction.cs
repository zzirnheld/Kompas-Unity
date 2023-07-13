using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.TriggerRestrictionElements
{
	public class StackableFitsRestriction : TriggerRestrictionBase
	{
		public IRestriction<IStackable> restriction;
		public IIdentity<IStackable> stackable;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			restriction.Initialize(initializationContext);
			stackable.Initialize(initializationContext);
		}

		protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
		{
			var item = stackable.From(IResolutionContext.Dummy(context), secondaryContext);
			return restriction.IsValid(item, ContextToConsider(context, secondaryContext));
		}
	}
}