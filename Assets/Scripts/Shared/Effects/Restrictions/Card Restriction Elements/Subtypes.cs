using KompasCore.Cards;
using System.Linq;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
    public class Subtypes : RestrictionElementBase<GameCardBase>
    {
        public string[] subtypes;

        public bool exclude = false; //default to include

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            if (subtypes == null) throw new System.ArgumentNullException("subtypes");
        }

        protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
        //If you're excluding all subtypes (exclude = true) you want all subtypes to not be present on the card (all = false)
        //If you're including all subtypes (exclude = false) you want all subtypes to be present on the card (all = true)
            => exclude != subtypes.All(card.HasSubtype);
    }
}