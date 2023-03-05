using KompasCore.Cards;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    namespace GamestateCardIdentities
    {

        public class Avatar : ContextualIdentityBase<GameCardBase>
        {
            public IIdentity<Player> player;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                player.Initialize(initializationContext);
            }

            protected override GameCardBase AbstractItemFrom(ActivationContext context, ActivationContext secondaryContext)
                => player.From(context, secondaryContext).Avatar;
        }
    }
}