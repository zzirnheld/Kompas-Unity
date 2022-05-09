using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects.Identities
{
    public abstract class ActivationContextCardsIdentity : ContextInitializeableBase, IContextInitializeable
    {
        protected abstract ICollection<GameCardBase> AbstractCardsFrom(ActivationContext context);

        public ICollection<GameCardBase> CardsFrom(ActivationContext context)
        {
            ComplainIfNotInitialized();
            return AbstractCardsFrom(context);
        }
    }

    namespace ActivationContextCardIdentities
    {
        public class CardsInPositions : ActivationContextCardsIdentity
        {
            public ActivationContextManySpacesIdentity positions;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                positions.Initialize(restrictionContext);
            }

            protected override ICollection<GameCardBase> AbstractCardsFrom(ActivationContext context)
            {
                var spaces = positions.SpacesFrom(context);
                return spaces.Select(RestrictionContext.game.boardCtrl.GetCardAt).Where(s => s != null).ToArray();
            }
        }
    }
}