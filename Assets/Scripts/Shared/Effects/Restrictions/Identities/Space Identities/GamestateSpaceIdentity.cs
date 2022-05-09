using KompasServer.Effects.Identities;

namespace KompasCore.Effects.Identities
{
    public abstract class GamestateSpaceIdentity : ContextInitializeableBase,
        IActivationContextSpaceIdentity, ISubeffectSpaceIdentity
    {
        protected abstract Space AbstractSpace { get; }

        public Space Space
        {
            get
            {
                ComplainIfNotInitialized();
                return AbstractSpace;
            }
        }

        public Space SpaceFrom(ActivationContext context) => Space;
    }

    namespace GamestateSpaceIdentities
    {
        public class PositionOf : GamestateSpaceIdentity
        {
            public GamestateCardIdentity card;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                card.Initialize(restrictionContext);
            }

            protected override Space AbstractSpace => card.CardFrom(RestrictionContext.game).Position;
        }
    }
}