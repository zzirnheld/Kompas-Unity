using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Identities;
using System;

namespace KompasServer.Effects.Identities
{
    public abstract class SubeffectCardIdentity
    {
        private bool initialized;

        protected RestrictionContext RestrictionContext { get; private set; }

        public virtual void Initialize(RestrictionContext restrictionContext)
        {
            RestrictionContext = restrictionContext;

            initialized = true;
        }

        public GameCardBase GetCard() => initialized ? GetCardLogic()
            : throw new NotImplementedException("You forgot to initialize a SubeffectCardIdentity!");


        protected abstract GameCardBase GetCardLogic();
    }

    public class ActivationContextSubeffectCardIdentity : SubeffectCardIdentity
    {
        public ActivationContextCardIdentity contextCardIdentity;

        public override void Initialize(RestrictionContext restrictionContext)
        {
            base.Initialize(restrictionContext);
            contextCardIdentity.Initialize(restrictionContext);
        }

        protected override GameCardBase GetCardLogic() 
            => contextCardIdentity.CardFrom(RestrictionContext.subeffect.Context);
    }

    public class ByIndexSubeffectCardIdentity : SubeffectCardIdentity
    {
        public int index;

        protected override GameCardBase GetCardLogic()
            => RestrictionContext.subeffect.Effect.GetTarget(index);
    }
}