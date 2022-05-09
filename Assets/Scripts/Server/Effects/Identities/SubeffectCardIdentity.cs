using KompasCore.Cards;
using KompasCore.Effects;
using KompasCore.Effects.Identities;

namespace KompasServer.Effects.Identities
{
    public abstract class SubeffectCardIdentity : ContextInitializeableBase, IContextInitializeable
    {
        public GameCardBase Card
        {
            get
            {
                ComplainIfNotInitialized();
                return AbstractCard;
            }
        }

        protected abstract GameCardBase AbstractCard { get; }
    }

    namespace SubeffectCardIdentities
    {
        public class FromActivationContext : SubeffectCardIdentity
        {
            public ActivationContextCardIdentity contextCardIdentity;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                contextCardIdentity.Initialize(restrictionContext);
            }

            protected override GameCardBase AbstractCard
                => contextCardIdentity.CardFrom(RestrictionContext.subeffect.Context);
        }

        public class ByIndex : SubeffectCardIdentity
        {
            public int index;

            protected override GameCardBase AbstractCard
                => RestrictionContext.subeffect.Effect.GetTarget(index);
        }
    }
}