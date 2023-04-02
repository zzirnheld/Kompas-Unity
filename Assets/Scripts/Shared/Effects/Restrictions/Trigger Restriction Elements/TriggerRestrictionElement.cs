using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Restrictions
{
    public abstract class TriggerRestrictionElement : ContextInitializeableBase, IContextInitializeable
    {
        public bool useDummyResolutionContext = true;

        public bool IsValidContext(TriggeringEventContext context, IResolutionContext secondaryContext = default)
        {
            ComplainIfNotInitialized();
            return AbstractIsValidContext(context, secondaryContext);
        }

        protected abstract bool AbstractIsValidContext(TriggeringEventContext context, IResolutionContext secondaryContext);

        protected IResolutionContext ContextToConsider(TriggeringEventContext triggeringContext, IResolutionContext resolutionContext)
            => useDummyResolutionContext
                ? IResolutionContext.Dummy(triggeringContext)
                : resolutionContext;
    }

    namespace TriggerRestrictionElements
    {
        public class Not : TriggerRestrictionElement
        {
            public TriggerRestrictionElement inverted;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                inverted.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidContext(TriggeringEventContext context, IResolutionContext secondaryContext)
                => !inverted.IsValidContext(context, secondaryContext);
        }

        public class AnyOf : TriggerRestrictionElement
        {
            public TriggerRestrictionElement[] restrictions;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                foreach (var r in restrictions) r.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidContext(TriggeringEventContext context, IResolutionContext secondaryContext)
                => restrictions.Any(r => r.IsValidContext(context, secondaryContext));
        }

        public class ThisCardInPlay : TriggerRestrictionElement
        {
            protected override bool AbstractIsValidContext(TriggeringEventContext context, IResolutionContext secondaryContext)
                => InitializationContext.source.Location == CardLocation.Board;
        }
    }
}