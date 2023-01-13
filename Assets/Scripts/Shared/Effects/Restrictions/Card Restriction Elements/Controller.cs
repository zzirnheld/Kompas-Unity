using KompasCore.Cards;
using KompasCore.Effects.Identities;
using KompasCore.Effects.Identities.GamestatePlayerIdentities;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
    public class Controller : CardRestrictionElement
    {
        public INoActivationContextIdentity<Player> playerIdentity;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            playerIdentity.Initialize(initializationContext);
        }

        protected override bool FitsRestrictionLogic(GameCardBase card, ActivationContext context)
            => playerIdentity.Item == card.Controller;
    }

    public class Friendly : Controller
    {
        public override void Initialize(EffectInitializationContext initializationContext)
        {
            playerIdentity = new FriendlyPlayer();
            base.Initialize(initializationContext);
        }
    }

    public class Enemy : Controller
    {
        public override void Initialize(EffectInitializationContext initializationContext)
        {
            playerIdentity = new EnemyPlayer();
            base.Initialize(initializationContext);
        }
    }
}