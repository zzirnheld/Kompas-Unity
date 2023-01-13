using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions
{

    namespace TriggerRestrictionElements
    {
        public class StackableFitsRestriction : TriggerRestrictionElement
        {
            public StackableRestriction restriction;
            public IActivationContextIdentity<IStackable> stackable;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                restriction.Initialize(initializationContext);
                stackable.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidContext(ActivationContext context, ActivationContext secondaryContext)
                => restriction.Evaluate(stackable.From(context, secondaryContext));
        }
    }
}