using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.TriggerRestrictionElements
{
    public class StackablesMatch : TriggerRestrictionBase
    {
        public IIdentity<IStackable> firstStackable;
        public IIdentity<IStackable> secondStackable;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            firstStackable.Initialize(initializationContext);
            secondStackable.Initialize(initializationContext);
        }

        protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
            => firstStackable.From(context, secondaryContext) == secondStackable.From(context, secondaryContext);
    }
}