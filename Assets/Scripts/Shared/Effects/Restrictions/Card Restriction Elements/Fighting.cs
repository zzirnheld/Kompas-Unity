using KompasCore.Cards;
using KompasCore.Effects.Identities;
using System.Linq;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
    public class Fighting : CardRestrictionElement
    {
        /// <summary>
        /// Can be null to represent checking whether the card is in any fight at all
        /// </summary>
        public IActivationContextIdentity<GameCardBase> fightingWho;
        public bool defending = false;
        public bool attacking = false;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            fightingWho?.Initialize(initializationContext);

            if (attacking && defending)
                throw new System.ArgumentException("Can't require a card to be attacking and defending using the same Fighting object");
        }

        private bool IsValidFight(GameCardBase card, ActivationContext context, IStackable stackEntry)
        {
            if (!(stackEntry is Attack attack)) return false;

            //If need to be attacking or defending, check those
            if (attacking)
            {
                if (attack.attacker != card) return false;
            }
            else if (defending)
            {
                if (attack.defender != card) return false;
            }
            //Otherwise, just make sure the character is in the fight at all. If it's neither the attacker nor defender, it's not in the fight
            else if (attack.attacker != card && attack.defender != card) return false;

            //And if that card is in the fight, make sure any other card that needs to be in the fight, is in the fight.
            if (fightingWho == null) return true;

            var fightingWhoCard = fightingWho.From(context, default);
            return attack.attacker == fightingWhoCard || attack.defender == fightingWhoCard;
        }

        protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
            => InitializationContext.game.StackEntries.Any(stackEntry => IsValidFight(card, context, stackEntry));
    }
}