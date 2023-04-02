using KompasCore.Cards;
using System.Linq;

namespace KompasCore.Effects.Identities.Cards
{

    public class Avatar : ContextualParentIdentityBase<GameCardBase>
    {
        public IIdentity<Player> player;

        public override void Initialize(EffectInitializationContext initializationContext)
        {
            base.Initialize(initializationContext);
            player.Initialize(initializationContext);
        }

        protected override GameCardBase AbstractItemFrom(IResolutionContext context, IResolutionContext secondaryContext)
            => player.From(context, secondaryContext).Avatar;
    }
}