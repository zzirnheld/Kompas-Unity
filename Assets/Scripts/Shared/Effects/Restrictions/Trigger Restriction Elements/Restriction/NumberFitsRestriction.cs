using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions
{

    namespace TriggerRestrictionElements
    {
        public class NumberFitsRestriction : TriggerRestrictionElement
        {
            public IIdentity<int> number;
            public NumberRestriction restriction;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                number.Initialize(initializationContext);
                restriction.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidContext(TriggeringEventContext context, IResolutionContext secondaryContext)
                => restriction.IsValid(number.From(context, secondaryContext), secondaryContext);
        }
    }
}