namespace KompasCore.Effects.Identities
{
    public abstract class GamestateSpaceIdentity : ContextInitializeableBase, IContextInitializeable
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
    }

    namespace GamestateSpaceIdentities
    {
        public class PositionOf : GamestateSpaceIdentity
        {
            public GamestateCardIdentity cardIdentity;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                cardIdentity.Initialize(restrictionContext);
            }

            protected override Space AbstractSpace => cardIdentity.CardFrom(RestrictionContext.game).Position;
        }
    }
}