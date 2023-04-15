using KompasCore.Effects.Identities;
using KompasCore.Effects.Relationships;

namespace KompasCore.Effects.Restrictions
{

    namespace TriggerRestrictionElements
    {
        public class CompareNumbers : TriggerRestrictionElement
        {
            public IIdentity<int> firstNumber;
            public IIdentity<int> secondNumber;
            public INumberRelationship comparison;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                firstNumber.Initialize(initializationContext);
                secondNumber.Initialize(initializationContext);
            }

            protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
            {
                int first = firstNumber.From(context, secondaryContext);
                int second = secondNumber.From(context, secondaryContext);
                return comparison.Compare(first, second);
            }
        }
    }
}