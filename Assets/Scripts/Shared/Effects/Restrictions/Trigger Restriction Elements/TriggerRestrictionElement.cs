using System;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Restrictions
{
    public abstract class TriggerRestrictionBase : RestrictionElementBase<TriggeringEventContext>, IContextInitializeable
    {
        public bool useDummyResolutionContext = true;

        protected IResolutionContext ContextToConsider(TriggeringEventContext triggeringContext, IResolutionContext resolutionContext)
            => useDummyResolutionContext
                ? IResolutionContext.Dummy(triggeringContext)
                : resolutionContext;
    }

    namespace TriggerRestrictionElements
    {
        public class AllOf : AllOfBase<TriggeringEventContext>
        {
            public static readonly ISet<Type> ReevalationRestrictions
                = new HashSet<Type>(new Type[] { typeof(MaxPerTurn), typeof(MaxPerRound), typeof(MaxPerStack) });

            public static readonly IRestriction<TriggeringEventContext>[] DefaultFallOffRestrictions = {
                new TriggerRestrictionElements.CardsMatch(){
                    card = new Identities.Cards.ThisCard(),
                    other = new Identities.Cards.CardBefore()
                },
                new TriggerRestrictionElements.ThisCardInPlay() };

            /// <summary>
            /// Reevaluates the trigger to check that any restrictions that could change between it being triggered
            /// and it being ordered on the stack, are still true.
            /// (Not relevant to delayed things, since those expire after a given number of uses (if at all), so yeah
            /// </summary>
            /// <returns></returns>
            public bool IsStillValidTriggeringContext(TriggeringEventContext context)
                => elements.Where(elem => ReevalationRestrictions.Contains(elem.GetType()))
                        .All(elem => elem.IsValid(context, default));
        }

        public class AlwaysValid : TriggerRestrictionBase
        {
            protected override bool IsValidLogic(TriggeringEventContext item, IResolutionContext context) => true;
        }

        public class AnyOf : TriggerRestrictionBase
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

        public class Not : TriggerRestrictionBase
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

        public class ThisCardInPlay : TriggerRestrictionBase
        {
            protected override bool IsValidLogic(TriggeringEventContext context, IResolutionContext secondaryContext)
                => InitializationContext.source.Location == CardLocation.Board;
        }
    }
}