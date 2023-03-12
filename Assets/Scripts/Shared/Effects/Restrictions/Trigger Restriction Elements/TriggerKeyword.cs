using System.Linq;

namespace KompasCore.Effects.Restrictions
{

    namespace TriggerRestrictionElements
    {
        public class TriggerKeyword : TriggerRestrictionElement
        {
            public string keyword;

            private TriggerRestrictionElement[] triggerRestrictionElements;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                triggerRestrictionElements = CardRepository.InstantiateTriggerKeyword(keyword);
            }

            protected override bool AbstractIsValidContext(ActivationContext context, ActivationContext secondaryContext)
                => triggerRestrictionElements.All(tre => tre.IsValidContext(context, secondaryContext));
        }
    }
}