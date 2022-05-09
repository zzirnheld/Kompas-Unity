using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    public interface IActivationContextManyCardsIdentity : IContextInitializeable
    {
        public ICollection<GameCardBase> CardsFrom(ActivationContext context);
    }

    public abstract class ActivationContextManyCardsIdentityBase : ContextInitializeableBase,
        IActivationContextManyCardsIdentity
    {
        protected abstract ICollection<GameCardBase> AbstractCardsFrom(ActivationContext context);

        public ICollection<GameCardBase> CardsFrom(ActivationContext context)
        {
            ComplainIfNotInitialized();
            return AbstractCardsFrom(context);
        }
    }

    namespace ActivationContextManyCardsIdentities
    {
        public class CardsInPositions : ActivationContextManyCardsIdentityBase
        {
            public IActivationContextManySpacesIdentity positions;

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