using KompasCore.Cards;
using KompasCore.Effects.Identities;
using KompasCore.Effects.Identities.Players;

namespace KompasCore.Effects.Restrictions.CardRestrictionElements
{
    public class Controller : CardRestrictionElement
    {
        public IIdentity<Player> playerIdentity;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            playerIdentity.Initialize(initializationContext);
        }

        protected override bool IsValidLogic(GameCardBase card, IResolutionContext context)
            => playerIdentity.From(context, default) == card.Controller;
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