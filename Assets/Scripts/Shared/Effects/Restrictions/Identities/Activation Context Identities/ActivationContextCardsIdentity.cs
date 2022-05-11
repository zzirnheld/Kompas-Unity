using KompasCore.Cards;
using System.Collections.Generic;
using System.Linq;

namespace KompasCore.Effects.Identities
{
    public abstract class ActivationContextManyCardsIdentityBase : ContextInitializeableBase,
        IActivationContextIdentity<ICollection<GameCardBase>>
    {
        protected abstract ICollection<GameCardBase> AbstractCardsFrom(ActivationContext context);

        public ICollection<GameCardBase> From(ActivationContext context)
        {
            ComplainIfNotInitialized();
            return AbstractCardsFrom(context);
        }
    }

    namespace ActivationContextManyCardsIdentities
    {
        public class CardsInPositions : ActivationContextManyCardsIdentityBase
        {
            public IActivationContextIdentity<ICollection<Space>> positions;

            public override void Initialize(EffectInitializationContext initializationContext)
            {
                base.Initialize(initializationContext);
                positions.Initialize(initializationContext);
            }

            protected override ICollection<GameCardBase> AbstractCardsFrom(ActivationContext context)
            {
                var spaces = positions.From(context);
                return spaces.Select(InitializationContext.game.boardCtrl.GetCardAt).Where(s => s != null).ToArray();
            }
        }
    }
}