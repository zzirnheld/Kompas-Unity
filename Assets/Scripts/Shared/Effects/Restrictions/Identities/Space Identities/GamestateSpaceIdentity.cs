namespace KompasCore.Effects.Identities
{
    public interface INoActivationContextSpaceIdentity : IContextInitializeable
    {
        public Space Space { get; }
    }

    public abstract class GamestateSpaceIdentity : ContextInitializeableBase,
        IActivationContextSpaceIdentity, INoActivationContextSpaceIdentity
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
            public INoActivationContextCardIdentity card;

            public override void Initialize(RestrictionContext restrictionContext)
            {
                base.Initialize(restrictionContext);
                card.Initialize(restrictionContext);
            }

            protected override Space AbstractSpace => card.Card.Position;
        }
    }
}