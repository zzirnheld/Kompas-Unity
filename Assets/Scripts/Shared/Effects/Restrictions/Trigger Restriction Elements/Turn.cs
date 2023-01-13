using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions.TriggerRestrictionElements
{
    public class Turn : TriggerRestrictionElement
    {
        public IActivationContextIdentity<Player> turnPlayer;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            turnPlayer.Initialize(initializationContext);
        }

        protected override bool AbstractIsValidContext(ActivationContext context, ActivationContext secondaryContext)
            => InitializationContext.game.TurnPlayer == turnPlayer.From(context, secondaryContext);
    }

    public class FriendlyTurn : Turn
    {
        public override void Initialize(EffectInitializationContext initializationContext)
        {
            turnPlayer = new Identities.GamestatePlayerIdentities.FriendlyPlayer();
            base.Initialize(initializationContext);
        }
    }

    public class EnemyTurn : Turn
    {
        public override void Initialize(EffectInitializationContext initializationContext)
        {
            turnPlayer = new Identities.GamestatePlayerIdentities.EnemyPlayer();
            base.Initialize(initializationContext);
        }
    }
}