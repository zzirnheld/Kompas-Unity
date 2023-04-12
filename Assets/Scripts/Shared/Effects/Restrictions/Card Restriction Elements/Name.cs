using System.Collections.Generic;
using System.Linq;
using KompasCore.Cards;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
    public class Name : CardRestrictionElement
    {
        public string nameIs;
        public string nameIncludes;

        protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
        {
            if (nameIs != null && card.CardName != nameIs) return false;
            if (nameIncludes != null && !card.CardName.Contains(nameIncludes)) return false;

            return true;
        }
    }

    public class DistinctName : CardRestrictionElement
    {
        public IIdentity<GameCardBase> from = new Identities.Cards.ThisCard();
        public IIdentity<IReadOnlyCollection<GameCardBase>> cards;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            from.Initialize(initializationContext);
            cards.Initialize(initializationContext);
        }

        protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
        {
            if (cards == default) return from.From(context, default).CardName != card.CardName;

            return cards.From(context, default)
                .Select(c => c.CardName)
                .All(name => name != card.CardName);
        }
    }
}