using KompasCore.Effects.Identities;

namespace KompasCore.Effects.Restrictions
{

    namespace TriggerRestrictionElements
    {
        public class PlayersMatch : TriggerRestrictionElement
        {
            public IActivationContextIdentity<Player> firstPlayer;
            public IActivationContextIdentity<Player> secondPlayer;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                firstPlayer.Initialize(initializationContext);
                secondPlayer.Initialize(initializationContext);
            }

            protected override bool AbstractIsValidContext(ActivationContext context, ActivationContext secondaryContext)
                => firstPlayer.From(context, secondaryContext) == secondPlayer.From(context, secondaryContext);
        }
    }
}