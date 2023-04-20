using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace KompasCore.Effects.Restrictions
{
    public abstract class TriggerRestrictionElement : RestrictionElementBase<TriggeringEventContext>, IContextInitializeable
    {
        public bool useDummyResolutionContext = true;

        protected IResolutionContext ContextToConsider(TriggeringEventContext triggeringContext, IResolutionContext resolutionContext)
            => useDummyResolutionContext
                ? IResolutionContext.Dummy(triggeringContext)
                : resolutionContext;
    }

    namespace TriggerRestrictionElements
    {
        public class Not : TriggerRestrictionElement
        {
            public IRestriction<TriggeringEventContext>  inverted;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                inverted.Initialize(initializationContext);
            }

            protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
                => !inverted.IsValid(context, secondaryContext);
        }

        public class AnyOf : TriggerRestrictionElement
        {
            public IRestriction<TriggeringEventContext> [] restrictions;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                foreach (var r in restrictions) r.Initialize(initializationContext);
            }

            protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
                => restrictions.Any(r => r.IsValid(context, secondaryContext));
        }

        public class ThisCardInPlay : TriggerRestrictionElement
        {
            protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
                => InitializationContext.source.Location == CardLocation.Board;
        }
    }
}