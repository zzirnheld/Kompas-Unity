using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Restrictions
{
    public abstract class TriggerRestrictionElement : ContextInitializeableBase, IContextInitializeable
    {
        public bool IsValidContext(ActivationContext context, ActivationContext secondaryContext = default)
        {
            ComplainIfNotInitialized();
            return AbstractIsValidContext(context, secondaryContext);
        }

        protected abstract bool AbstractIsValidContext(ActivationContext context, ActivationContext secondaryContext);
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

            protected override bool AbstractIsValidContext(ActivationContext context, ActivationContext secondaryContext)
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

            protected override bool AbstractIsValidContext(ActivationContext context, ActivationContext secondaryContext)
                => restrictions.Any(r => r.IsValidContext(context, secondaryContext));
        }

        public class ThisCardInPlay : TriggerRestrictionElement
        {
            protected override bool AbstractIsValidContext(ActivationContext context, ActivationContext secondaryContext)
                => InitializationContext.source.Location == CardLocation.Board;
        }
    }
}