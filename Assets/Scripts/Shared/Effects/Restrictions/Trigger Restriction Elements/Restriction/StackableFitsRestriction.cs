using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.TriggerRestrictionElements
{
    public class StackableFitsRestriction : TriggerRestrictionBase
    {
        public StackableRestriction restriction;
        public IIdentity<IStackable> stackable;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            restriction.Initialize(initializationContext);
            stackable.Initialize(initializationContext);
        }

        protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
            => restriction.Evaluate(stackable.From(IResolutionContext.Dummy(context), secondaryContext));
    }
}