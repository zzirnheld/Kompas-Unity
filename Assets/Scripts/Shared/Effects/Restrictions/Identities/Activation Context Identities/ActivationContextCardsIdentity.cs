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

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                positions.Initialize(initializationContext);
            }

            protected override ICollection<GameCardBase> AbstractCardsFrom(ActivationContext context)
            {
                var spaces = positions.SpacesFrom(context);
                return spaces.Select(InitializationContext.game.boardCtrl.GetCardAt).Where(s => s != null).ToArray();
            }
        }
    }
}