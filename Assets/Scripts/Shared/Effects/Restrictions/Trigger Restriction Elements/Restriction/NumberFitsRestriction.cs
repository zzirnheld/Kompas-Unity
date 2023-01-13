using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions
{

    namespace TriggerRestrictionElements
    {
        public class NumberFitsRestriction : TriggerRestrictionElement
        {
            public IActivationContextIdentity<int> number;
            public NumberRestriction restriction;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                number.Initialize(initializationContext);
                restriction.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidContext(ActivationContext context, ActivationContext secondaryContext)
                => restriction.IsValidNumber(number.From(context, secondaryContext));
        }
    }
}