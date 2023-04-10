using KompasCore.Cards;
using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
    public class Name : RestrictionElementBase<GameCardBase>
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

    public class DistinctName : RestrictionElementBase<GameCardBase>
    {
        public IIdentity<GameCardBase> from = new Identities.Cards.ThisCard();

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            from.Initialize(initializationContext);
        }

        protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
            => from.From(context, default).CardName != card.CardName;
    }
}