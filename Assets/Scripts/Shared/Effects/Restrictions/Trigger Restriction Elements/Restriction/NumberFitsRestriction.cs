using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.TriggerRestrictionElements
{
	public class NumberFitsRestriction : TriggerGamestateRestrictionBase
	{
		public IIdentity<int> number;
		public IRestriction<int> restriction;

		public override void Initialize(EffectInitializationContext initializationContext)
		{
			base.Initialize(initializationContext);
			number.Initialize(initializationContext);
			restriction.Initialize(initializationContext);
		}


		protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
			=> restriction.IsValid(number.From(context, secondaryContext), secondaryContext);
	}
}