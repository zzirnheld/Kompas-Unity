using KompasCore.Cards;
using KompasCore.Effects;
using System;
using System.Collections.Generic;
using System.Linq;

namespace KompasServer.Effects.Identities
{
    public abstract class SubeffectCardsIdentity
    {
        private bool initialized;

        protected RestrictionContext RestrictionContext { get; private set; }

        public virtual void Initialize(RestrictionContext restrictionContext)
        {
            RestrictionContext = restrictionContext;

            initialized = true;
        }

        public ICollection<GameCardBase> GetCards() => initialized ? GetCardsLogic()
            : throw new NotImplementedException("You forgot to initialize a SubeffectCardIdentity!");


        protected abstract ICollection<GameCardBase> GetCardsLogic();
    }

    public class CardsInSpacesSubeffectCardsIdentity : SubeffectCardsIdentity
    {
        public SubeffectSpacesIdentity spacesIdentity;

        public override void Initialize(RestrictionContext restrictionContext)
        {
            base.Initialize(restrictionContext);
            spacesIdentity.Initialize(restrictionContext);
        }

        protected override ICollection<GameCardBase> GetCardsLogic()
        {
            var spaces = spacesIdentity.GetSpaces();
            var cards = spaces.Select(RestrictionContext.game.boardCtrl.GetCardAt).Where(s => s != null).ToArray();
            return cards;
        }
    }

    public class ActivationContextSubeffectCardsIdentity : SubeffectCardsIdentity
    {
        public ActivationContextCardsIdentity cardsIdentity;

        public override void Initialize(RestrictionContext restrictionContext)
        {
            base.Initialize(restrictionContext);
            cardsIdentity.Initialize(restrictionContext);
        }

        protected override ICollection<GameCardBase> GetCardsLogic()
            => cardsIdentity.CardsFrom(RestrictionContext.subeffect.Context);
    }
}