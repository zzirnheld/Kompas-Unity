using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions
{

    namespace TriggerRestrictionElements
    {
        public class StackablesMatch : TriggerRestrictionElement
        {
            public IActivationContextIdentity<IStackable> firstStackable;
            public IActivationContextIdentity<IStackable> secondStackable;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                firstStackable.Initialize(initializationContext);
                secondStackable.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidContext(ActivationContext context, ActivationContext secondaryContext)
                => firstStackable.From(context, secondaryContext) == secondStackable.From(context, secondaryContext);
        }
    }
}