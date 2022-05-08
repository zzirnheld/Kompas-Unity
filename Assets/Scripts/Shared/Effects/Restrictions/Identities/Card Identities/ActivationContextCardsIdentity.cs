using KompasCore.Cards;
using KompasCore.Effects;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects.Identities
{
    public abstract class ActivationContextCardsIdentity
    {
        private bool initialized;

        protected RestrictionContext RestrictionContext { get; private set; }

        public virtual void Initialize(RestrictionContext restrictionContext)
        {
            RestrictionContext = restrictionContext;

            initialized = true;
        }

        protected abstract ICollection<GameCardBase> CardsFromLogic(ActivationContext context);

        public ICollection<GameCardBase> CardsFrom(ActivationContext context)
            => initialized ? CardsFromLogic(context)
                : throw new System.NotImplementedException("You forgot to initialize an ActivationContextCardIdentity!");
    }

    public class CardsInPositionsContextCardsIdentity : ActivationContextCardsIdentity
    {
        public ActivationContextSpacesIdentity spacesIdentity;

        public override void Initialize(RestrictionContext restrictionContext)
        {
            base.Initialize(restrictionContext);
            spacesIdentity.Initialize(restrictionContext);
        }

        protected override ICollection<GameCardBase> CardsFromLogic(ActivationContext context)
        {
            var spaces = spacesIdentity.SpacesFrom(context);
            return spaces.Select(RestrictionContext.game.boardCtrl.GetCardAt).Where(s => s != null).ToArray();
        }
    }
}